using System.Text;
using System.Text.Json;
using Google.Protobuf;
using Grpc.Net.Client;
using IO.Milvus.Grpc;
using Realchat.Application.Storage;
using static IO.Milvus.Grpc.MilvusService;

namespace Realchat.Infrastructure.Storage;

public sealed class MilvusAdapter : IMilvusAdapter
{
    const string CollectionName = @"chatbot_information_chunk";
    const string MilvusUrl = @"http://localhost:9091";
    const string BertUrl = @"http://localhost:9089/vectors";

    public async Task ImportDataToMilvus(string vectorChunk, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId, int i)
    {
        var textVector = vectorChunk.Substring(1, vectorChunk.Length - 2);
        var textFloats = textVector.Split(",").Select(float.Parse).ToArray();
        var payload = new
        {
            collection_name = $"{CollectionName}",
            fields_data = new[]
            {
                new
                {
                    field_name = "organization_id",
                    type = 21,
                    field = new object[] { organizationId }
                },
                new
                {
                    field_name = "chatbot_id",
                    type = 21,
                    field = new object[] { chatbotId }
                },
                new
                {
                    field_name = "knowledge_base_id",
                    type = 21,
                    field = new object[] { knowledgeBaseId }
                },
                new
                {
                    field_name = "chunk_number",
                    type = 5,
                    field = new object[] { i }
                },
                new
                {
                    field_name = "information_chunk_vector",
                    type = 101,
                    field = new object[] { textFloats }
                }
            },
            num_rows = 1
        };
        Console.WriteLine("\nImporting data to Milvus.");
        Console.WriteLine(JsonSerializer.Serialize(payload));

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("accept", "application/json");

        var response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/entities", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to store information chunk {i}.");
        }

        Console.WriteLine($"Information chunk {i} stored successfully.");
        Console.WriteLine(responseBody);
    }

    public async Task ImportScriptToMilvus(string vectorChunk, Guid organizationId, Guid chatbotId, Guid scriptId)
    {
        var textVector = vectorChunk.Substring(1, vectorChunk.Length - 2);
        var textFloats = textVector.Split(",").Select(float.Parse).ToArray();

        var payload = new
        {
            collection_name = $"chatbot_script",
            fields_data = new[]
            {
                new
                {
                    field_name = "organization_id",
                    type = 21,
                    field = new object[] { organizationId }
                },
                new
                {
                    field_name = "chatbot_id",
                    type = 21,
                    field = new object[] { chatbotId }
                },
                new
                {
                    field_name = "script_id",
                    type = 21,
                    field = new object[] { scriptId }
                },
                new
                {
                    field_name = "trigger_text_vector",
                    type = 101,
                    field = new object[] { textFloats }
                }
            },
            num_rows = 1
        };
        Console.WriteLine("\nImporting script data to Milvus.");
        Console.WriteLine(JsonSerializer.Serialize(payload));

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("accept", "application/json");

        var response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/entities", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to store script.");
        }

        Console.WriteLine($"Script stored successfully.");
        Console.WriteLine(responseBody);
    }

    public async Task<(Guid, int)> Search(string chatbotId, string searchString)
    {
        string? searchVector = await EncodeText(searchString.ToLower());
        if (searchVector == null)
        {
            return (Guid.Empty, 0);
        }

        var textVector = searchVector.Substring(1, searchVector.Length - 2);
        var textFloats = textVector.Split(",").Select(float.Parse).ToArray();

        float[] floats;
        floats = textFloats;
        var placeholderGroup = new PlaceholderGroup();
        var placeholderValue = new PlaceholderValue
        {
            Type = PlaceholderType.FloatVector,
            Tag = "$0"
        };

        using (var memoryStream = new MemoryStream(floats.ToList().Count * sizeof(float)))
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            for (int i = 0; i < floats.ToList().Count; i++)
                binaryWriter.Write(floats.ToList()[i]);

            memoryStream.Seek(0, SeekOrigin.Begin);
            placeholderValue.Values.Add(ByteString.FromStream(memoryStream));
        }
        placeholderGroup.Placeholders.Add(placeholderValue);

        Console.ForegroundColor = ConsoleColor.Magenta;
        double magnitude = CalculateMagnitude(floats);
        Console.WriteLine($"Magnitude: {magnitude}");
        Console.ForegroundColor = ConsoleColor.White;

        SearchRequest searchRequest;
        searchRequest = new SearchRequest
        {
            CollectionName = "chatbot_information_chunk",
            PartitionNames = { "_default" },
            Dsl = "",
            DslType = DslType.BoolExprV1,
            PlaceholderGroup = placeholderGroup.ToByteString(),
            OutputFields = { "knowledge_base_id", "chunk_number", "chatbot_id" },
            SearchParams = {
                new IO.Milvus.Grpc.KeyValuePair { Key = "anns_field", Value = "information_chunk_vector" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "topk", Value = "2" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "params", Value = "{\"nprobe\": 32}" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "metric_type", Value = "IP" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "round_decimal", Value = "-1" }
                // new IO.Milvus.Grpc.KeyValuePair { Key = "offset", Value = "0" },
                }
        };

        var milvusClient = new MilvusServiceClient(GrpcChannel.ForAddress("http://localhost:19530"));
        var response = await milvusClient.SearchAsync(searchRequest);
        Console.WriteLine(JsonSerializer.Serialize(response));

        if (response.Results == null || response.Results.Scores == null || response.Results.FieldsData == null)
        {
            Console.WriteLine("Milvus returned null response.");
            return (Guid.Empty, 0);
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Retreiving IP scores.");
        Console.WriteLine(JsonSerializer.Serialize(response.Results?.Scores.ToList()));
        var cosineSimilarities = CalculateCosineSimilarity(response.Results?.Scores.ToList(), magnitude);
        Console.WriteLine("Calculating cosine similarity scores.");
        Console.WriteLine(JsonSerializer.Serialize(cosineSimilarities));
        Console.ForegroundColor = ConsoleColor.White;

        if (cosineSimilarities.FirstOrDefault() <= 0.3)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Cannot find high-quality context for the question.");
            Console.ForegroundColor = ConsoleColor.White;
            return (Guid.Empty, 0);
        }

        var chatbots = response.Results.FieldsData.Where((field) => field.FieldName == "chatbot_id").FirstOrDefault().Scalars.StringData.Data;

        var chatbotArray = chatbots.ToArray();
        int order = 0;
        for (int i = 0; i < chatbotArray.Length; i++)
        {
            if (chatbotArray[i] == chatbotId)
            {
                order = i;
                break;
            }
        }

        string knowledgeBaseId = response.Results.FieldsData.Where((field) => field.FieldName == "knowledge_base_id").FirstOrDefault().Scalars.StringData.Data.ToArray()[order];

        long chunkNumber = response.Results.FieldsData.Where((field) => field.FieldName == "chunk_number").FirstOrDefault().Scalars.LongData.Data.ToArray()[order];
        return (Guid.Parse(knowledgeBaseId), Convert.ToInt32(chunkNumber));
    }

    public async Task<Guid> SearchScript(string chatbotId, string searchString)
    {
        string? searchVector = await EncodeText(searchString.ToLower());
        if (searchVector == null)
        {
            return Guid.Empty;
        }

        var textVector = searchVector.Substring(1, searchVector.Length - 2);
        var textFloats = textVector.Split(",").Select(float.Parse).ToArray();

        float[] floats;
        floats = textFloats;
        var placeholderGroup = new PlaceholderGroup();
        var placeholderValue = new PlaceholderValue
        {
            Type = PlaceholderType.FloatVector,
            Tag = "$0"
        };

        using (var memoryStream = new MemoryStream(floats.ToList().Count * sizeof(float)))
        using (var binaryWriter = new BinaryWriter(memoryStream))
        {
            for (int i = 0; i < floats.ToList().Count; i++)
                binaryWriter.Write(floats.ToList()[i]);

            memoryStream.Seek(0, SeekOrigin.Begin);
            placeholderValue.Values.Add(ByteString.FromStream(memoryStream));
        }
        placeholderGroup.Placeholders.Add(placeholderValue);

        Console.ForegroundColor = ConsoleColor.Magenta;
        double magnitude = CalculateMagnitude(floats);
        Console.WriteLine($"Magnitude: {magnitude}");
        Console.ForegroundColor = ConsoleColor.White;

        SearchRequest searchRequest;
        searchRequest = new SearchRequest
        {
            CollectionName = "chatbot_script",
            PartitionNames = { "_default" },
            Dsl = "",
            DslType = DslType.BoolExprV1,
            PlaceholderGroup = placeholderGroup.ToByteString(),
            OutputFields = { "script_id", "chatbot_id" },
            SearchParams = {
                new IO.Milvus.Grpc.KeyValuePair { Key = "anns_field", Value = "trigger_text_vector" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "topk", Value = "2" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "params", Value = "{\"nprobe\": 32}" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "metric_type", Value = "IP" },
                new IO.Milvus.Grpc.KeyValuePair { Key = "round_decimal", Value = "-1" }
                // new IO.Milvus.Grpc.KeyValuePair { Key = "offset", Value = "0" },
                }
        };

        var milvusClient = new MilvusServiceClient(GrpcChannel.ForAddress("http://localhost:19530"));
        var response = await milvusClient.SearchAsync(searchRequest);
        Console.WriteLine(JsonSerializer.Serialize(response));

        if (response.Results == null || response.Results.Scores == null || response.Results.FieldsData == null)
        {
            Console.WriteLine("Milvus returned null response.");
            return Guid.Empty;
        }

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Retreiving IP scores.");
        Console.WriteLine(JsonSerializer.Serialize(response.Results?.Scores.ToList()));
        var cosineSimilarities = CalculateCosineSimilarity(response.Results?.Scores.ToList(), magnitude);
        Console.WriteLine("Calculating cosine similarity scores.");
        Console.WriteLine(JsonSerializer.Serialize(cosineSimilarities));
        Console.ForegroundColor = ConsoleColor.White;

        if (cosineSimilarities.FirstOrDefault() <= 0.1)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Cannot find high-quality context for the question.");
            Console.ForegroundColor = ConsoleColor.White;
            return Guid.Empty;
        }

        var chatbots = response.Results.FieldsData.Where((field) => field.FieldName == "chatbot_id").FirstOrDefault().Scalars.StringData.Data;

        var chatbotArray = chatbots.ToArray();

        Console.WriteLine("chatbot array");
        Console.WriteLine(JsonSerializer.Serialize(chatbotArray));
        int order = 0;
        for (int i = 0; i < chatbotArray.Length; i++)
        {
            if (chatbotArray[i] == chatbotId)
            {
                order = i;
                break;
            }
        }

        Console.WriteLine("order ");
        Console.WriteLine(order);

        Console.WriteLine(JsonSerializer.Serialize(response.Results.FieldsData.Where((field) => field.FieldName == "script_id").FirstOrDefault().Scalars.StringData.Data.ToArray()));
        string scriptId = response.Results.FieldsData.Where((field) => field.FieldName == "script_id").FirstOrDefault().Scalars.StringData.Data.ToArray()[order];
        // string scriptId = response.Results.FieldsData.Where((field) => field.FieldName == "script_id").FirstOrDefault().Scalars.StringData.Data.ToArray().Where(d => d )
        return Guid.Parse(scriptId);
    }

    private static async Task<string?> EncodeText(string text)
    {
        using HttpClient httpClient = new();
        var payload = new
        {
            text = text
        };
        var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(BertUrl, contentData);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Text {text} sent successfully.");
            JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (json.RootElement.TryGetProperty("vector", out var value))
            {
                return value.ToString();
            }
        }
        else
        {
            Console.WriteLine($"Failed to send text {text}. Status code: {response.StatusCode}");
        }
        return null;
    }

    // Calculate the magnitude (Euclidean norm) of a vector
    private static double CalculateMagnitude(float[] vector)
    {
        double sumOfSquares = 0;
        foreach (var value in vector)
        {
            sumOfSquares += value * value;
        }

        return Math.Sqrt(sumOfSquares);
    }

    private static List<double> CalculateCosineSimilarity(List<float> ipScores, double magnitude)
    {
        List<double> cosineSimilarities = new();
        for (int i = 0; i < ipScores.Count; i++)
        {
            double cosineSimilarity = ipScores[i] / (magnitude * magnitude);
            cosineSimilarities.Add(cosineSimilarity);
        }

        return cosineSimilarities;
    }
}