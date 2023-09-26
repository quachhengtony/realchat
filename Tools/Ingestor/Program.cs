using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Realchat.Tools.Ingestor.Models;

const string BertUrl = @"http://localhost:9089";
const string ClipUrl = @"http://localhost:9090";
const string MilvusUrl = @"http://localhost:9091";
const string CollectionName = @"chatbot_information_chunk";
const string CollectionDesciption = @"Chatbot information chunk";
const string inputDirectory = @"D:\Projects\Realchat.Data\Encoded"; // Replace with your directory path

// await EncodeText();
await UpsertMilvusCollection();
// await ImportDataToMilvus();

static async Task EncodeText()
{
    string directoryPath = @"D:\Projects\Realchat.Data\Processed"; // Replace with your directory path
    string outputDirectory = @"D:\Projects\Realchat.Data\Encoded";
    string apiUrl = "http://localhost:9089/vectors"; // Replace with your API URL

    string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");

    using HttpClient httpClient = new HttpClient();

    foreach (string txtFile in txtFiles)
    {
        try
        {
            string content = File.ReadAllText(txtFile);

            var payload = new
            {
                text = content
            };

            var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, contentData);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"File {txtFile} sent successfully.");
                JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                if (json.RootElement.TryGetProperty("vector", out var value))
                {
                    string outputFilePath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(txtFile)}_encoded.txt");
                    File.WriteAllText(outputFilePath, value.ToString());
                }
            }
            else
            {
                Console.WriteLine($"Failed to send file {txtFile}. Status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing file {txtFile}: {ex.Message}");
        }
    }

    Console.WriteLine("Processing complete.");
}

static async Task UpsertMilvusCollection()
{
    var chatbotInformationChunkSchema = new CreateCollectionRequest
    {
        collection_name = CollectionName,
        schema = new CollectionSchema
        {
            autoID = false,
            description = $"{CollectionName}",
            fields = new List<CollectionField>
            {
                new CollectionField
                {
                    name = "id",
                    description = "Id",
                    is_primary_key = true,
                    autoID = true,
                    data_type = 5
                },
                new CollectionField
                {
                    name = "organization_id",
                    description = "Organization id",
                    is_primary_key = false,
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "chatbot_id",
                    description = "Chatbot id",
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "knowledge_base_id",
                    description = "Knowledge base id",
                    is_primary_key = false,
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "chunk_number",
                    description = "Chunk number",
                    is_primary_key = false,
                    data_type = 5
                },
                new CollectionField
                {
                    name = "information_chunk_vector",
                    description = "Information chunk vector",
                    is_primary_key = false,
                    data_type = 101,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "dim", value = "384" }
                    }
                }
            },
            name = $"{CollectionName}"
        }
    };

    var chatbotScriptSchema = new CreateCollectionRequest
    {
        collection_name = "chatbot_script",
        schema = new CollectionSchema
        {
            autoID = false,
            description = $"{CollectionName}",
            fields = new List<CollectionField>
            {
                new CollectionField
                {
                    name = "id",
                    description = "Id",
                    is_primary_key = true,
                    autoID = true,
                    data_type = 5
                },
                new CollectionField
                {
                    name = "organization_id",
                    description = "Organization id",
                    is_primary_key = false,
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "chatbot_id",
                    description = "Chatbot id",
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "script_id",
                    description = "Script id",
                    data_type = 21,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "max_length", value = "36" }
                    }
                },
                new CollectionField
                {
                    name = "trigger_text_vector",
                    description = "Trigger text vector",
                    is_primary_key = false,
                    data_type = 101,
                    type_params = new List<TypeParams>
                    {
                        new TypeParams { key = "dim", value = "384" }
                    }
                }
            },
            name = $"chatbot_script"
        }
    };

    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Add("accept", "application/json");

    var checkCollectionRequest = new HttpRequestMessage(HttpMethod.Get, $"{MilvusUrl}/api/v1/collection/existence")
    {
        Content = new StringContent(JsonSerializer.Serialize(new { collection_name = CollectionName }))
    };
    var response = await httpClient.SendAsync(checkCollectionRequest);
    if (!response.IsSuccessStatusCode)
        throw new HttpRequestException($"Failed to check collection {CollectionName} existence.");

    string responseBody = await response.Content.ReadAsStringAsync();
    JsonDocument json = JsonDocument.Parse(responseBody);
    if (json.RootElement.TryGetProperty("value", out _))
    {
        Console.WriteLine($"Collection {CollectionName} already exists. Skipping the creation of collection {CollectionName}.");
        return;
    }

    var jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    var productSchemaJson = JsonSerializer.Serialize(chatbotInformationChunkSchema, jsonSerializerOptions);
    var productImageSchemaJson = JsonSerializer.Serialize(chatbotScriptSchema, jsonSerializerOptions);
    // var userProductPreferenceSchemaJson = JsonSerializer.Serialize(userProductPreferenceSchema, jsonSerializerOptions);

    response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/collection", new StringContent(productSchemaJson, Encoding.UTF8, "application/json"));
    responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode || responseBody != "{}")
        throw new HttpRequestException($"Failed to create collection {CollectionName}.");

    Console.WriteLine($"Collection {CollectionName} created successfully.");
    response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/collection", new StringContent(productImageSchemaJson, Encoding.UTF8, "application/json"));
    responseBody = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode || responseBody != "{}")
        throw new HttpRequestException($"Failed to create collection chatbot_script.");

    Console.WriteLine($"Collection chatbot_script created successfully.");
    // response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/collection", new StringContent(userProductPreferenceSchemaJson, Encoding.UTF8, "application/json"));
    // responseBody = await response.Content.ReadAsStringAsync();

    // if (!response.IsSuccessStatusCode || responseBody != "{}")
    //     throw new HttpRequestException($"Failed to create collection user_product_preference.");

    // Console.WriteLine($"Collection {CollectionName} created successfully.");
}

static async Task ImportDataToMilvus()
{
    if (!Directory.Exists(inputDirectory))
    {
        Console.WriteLine($"Directory {inputDirectory} does not exist.");
        return;
    }

    string[] txtFiles = Directory.GetFiles(inputDirectory, "*.txt");

    foreach (string txtFile in txtFiles)
    {
        var productId = new Random().Next();

        string content = File.ReadAllText(txtFile);
        Console.WriteLine($"Content of {Path.GetFileName(txtFile)}:");
        Console.WriteLine(content);
        Console.WriteLine(); // Empty line to separate contents

        var imagePayload = new
        {
            collection_name = $"{CollectionName}_image",
            fields_data = new[]
    {
                new
                {
                    field_name = "id",
                    type = 5,
                    field = new object[] { productId }
                },
                new
                {
                    field_name = "information_chunk_vector",
                    type = 101,
                    field = new object[] { content }
                }
            },
            num_rows = 1
        };
    }

    Console.WriteLine("Reading and printing complete.");

    // using (var httpClient = new HttpClient())
    // {
    //     httpClient.DefaultRequestHeaders.Add("accept", "application/json");

    //     var response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/entities", new StringContent(JsonSerializer.Serialize(productPayload), Encoding.UTF8, "application/json"));
    //     var responseBody = await response.Content.ReadAsStringAsync();

    //     if (!response.IsSuccessStatusCode)
    //         throw new HttpRequestException($"Failed to store product {csvProduct.DisplayName}.");

    //     Console.WriteLine($"Product {csvProduct.DisplayName} with id {productId} stored successfully.");
    //     Console.WriteLine(responseBody);
    //     response = await httpClient.PostAsync($"{MilvusUrl}/api/v1/entities", new StringContent(JsonSerializer.Serialize(imagePayload), Encoding.UTF8, "application/json"));
    //     responseBody = await response.Content.ReadAsStringAsync();

    //     if (!response.IsSuccessStatusCode)
    //         throw new HttpRequestException($"Failed to store image of product {csvProduct.DisplayName}.");

    //     Console.WriteLine($"Image of product {csvProduct.DisplayName} with id {productId} stored successfully.");
    //     Console.WriteLine(responseBody);
    // }
}