using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Minio.DataModel;
using Realchat.Application.Storage;
using Realchat.Application.Workers;
using Realchat.Infrastructure.Workers;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace Realchat.Infrastructure.Storage;

public sealed class MinioAdapter : IMinioAdapter
{
    const string endpoint = "localhost:9000";
    const string accessKey = "minioadmin";
    const string secretKey = "minioadmin";
    const bool secure = false;

    private readonly MinioClient minio;
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMilvusAdapter _milvusAdapter;

    private Guid _organizationId, _chatbotId, _knowledgeBaseId;

    public MinioAdapter(IBackgroundTaskQueue backgroundTaskQueue, IServiceScopeFactory serviceScopeFactory, IMilvusAdapter milvusAdapter)
    {
        _milvusAdapter = milvusAdapter;
        _backgroundTaskQueue = backgroundTaskQueue;
        _serviceScopeFactory = serviceScopeFactory;
        minio = new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(secure)
                    .Build();
    }

    public async Task ListBuckets()
    {
        var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

        foreach (var bucket in getListBucketsTask.Buckets)
        {
            Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
        }
    }

    public async Task UploadAndPreprocessFile(string objectPath, Stream fileStream, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId)
    {
        List<string> informationChunks = new();
        var bucketName = "realchat";
        var contentType = "application/octet-stream";

        // Make a bucket on the server, if not already present.
        var beArgs = new BucketExistsArgs()
                        .WithBucket(bucketName);
        bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                            .WithBucket(bucketName);
            await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
                                .WithBucket(bucketName)
                                .WithObject(objectPath)
                                .WithStreamData(fileStream)
                                .WithObjectSize(fileStream.Length)
                                .WithContentType(contentType);
        await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        // Confirm upload
        StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                .WithBucket(bucketName)
                                                .WithObject(objectPath);
        ObjectStat objectStat = await minio.StatObjectAsync(statObjectArgs);
        Console.WriteLine("Successfully uploaded " + objectStat.ObjectName);

        _backgroundTaskQueue.QueueBackgroundWorkItem(async (token) =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedServices = scope.ServiceProvider;
            await PreprocessFile(objectPath, await GetFile("realchat", objectPath), organizationId, chatbotId, knowledgeBaseId);
        });
    }

    private async Task<Stream> GetFile(string bucketName, string objectPath)
    {
        var memoryStream = new MemoryStream();
        StatObjectArgs statObjectArgs = new StatObjectArgs().WithBucket(bucketName).WithObject(objectPath);
        ObjectStat objectStat = await minio.StatObjectAsync(statObjectArgs);
        Console.WriteLine("File exists " + objectStat.ObjectName);

        var getObjectArgs = new GetObjectArgs()
                                    .WithBucket(bucketName)
                                    .WithObject(objectPath)
                                    .WithCallbackStream(async (stream) =>
        {
            await stream.CopyToAsync(memoryStream);
        });
        await minio.GetObjectAsync(getObjectArgs);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private async Task PreprocessFile(string objectPath, Stream fileStream, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId)
    {
        using HttpClient httpClient = new();

        Console.WriteLine("Preprocessing file.");
        WordprocessingDocument doc = WordprocessingDocument.Open(fileStream, false);

        XmlDocument xmlDocument = new();
        xmlDocument.Load(doc.MainDocumentPart.GetStream());

        var words = GetWordsFromXmlDocument(xmlDocument);

        int chunkSize = 100;
        int chunkCount = (int)Math.Ceiling((double)words.Count / chunkSize);

        for (int i = 0; i < chunkCount; i++)
        {
            int startIndex = i * chunkSize;
            int endIndex = Math.Min(startIndex + chunkSize, words.Count);
            List<string> chunk = words.GetRange(startIndex, endIndex - startIndex);

            string chunkText = string.Join(" ", chunk); // Join words with spaces

            Console.WriteLine($"Uploading chunk {i + 1}");
            await UploadFile($"{objectPath}_{i + 1}.txt", StringToStream(chunkText));
            await EncodeText(httpClient, chunkText, objectPath, i, organizationId, chatbotId, knowledgeBaseId);
        }
    }

    private async Task EncodeText(HttpClient httpClient, string content, string objectPath, int i, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId)
    {
        string apiUrl = "http://localhost:9089/vectors";
        try
        {
            var payload = new
            {
                text = content.ToLower()
            };
            var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, contentData);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"File sent {objectPath} successfully.");
                JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                if (json.RootElement.TryGetProperty("vector", out var value))
                {
                    await UploadFile($"{objectPath}_{i + 1}_encoded.txt", StringToStream(value.ToString()));
                    await _milvusAdapter.ImportDataToMilvus(value.ToString(), organizationId, chatbotId, knowledgeBaseId, i + 1);
                }
            }
            else
            {
                Console.WriteLine($"Failed to send file {objectPath}. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {objectPath}: {ex.Message}");
        }

        Console.WriteLine("Processing complete.");
    }

    private async Task<bool> UploadFile(string objectPath, Stream fileStream)
    {
        var bucketName = "realchat";
        var contentType = "application/octet-stream";

        // Make a bucket on the server, if not already present.
        var beArgs = new BucketExistsArgs()
                        .WithBucket(bucketName);
        bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs()
                            .WithBucket(bucketName);
            await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        // Upload a file to bucket.
        var putObjectArgs = new PutObjectArgs()
                                .WithBucket(bucketName)
                                .WithObject(objectPath)
                                .WithStreamData(fileStream)
                                .WithObjectSize(fileStream.Length)
                                .WithContentType(contentType);
        await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        // Confirm upload
        StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                .WithBucket(bucketName)
                                                .WithObject(objectPath);
        ObjectStat objectStat = await minio.StatObjectAsync(statObjectArgs);
        Console.WriteLine("Successfully uploaded " + objectStat.ObjectName);

        return true;
    }

    private static Stream StringToStream(string str)
    {
        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(str);
        return new MemoryStream(byteArray);
    }

    private List<string> GetWordsFromXmlDocument(XmlDocument xmlDocument)
    {
        List<string> words = new();

        // Recursively traverse the XML tree and extract text from XmlText nodes
        void ExtractWords(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Text)
            {
                string[] rawWords = node.Value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                words.AddRange(rawWords);
            }
            else
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    ExtractWords(childNode);
                }
            }
        }

        ExtractWords(xmlDocument);

        return words;
    }
}