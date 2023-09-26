using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Realchat.Application.Features.ChatbotFeatures.Chat;
using Realchat.Application.LLM;
using Realchat.Infrastructure.LLM;

namespace Realchat.WebApi.Hubs;

public sealed record Category
{

    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public sealed record Product
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}

public sealed record ProductDto
{
    [JsonPropertyName("productDto")]
    public Product? Product { get; set; }
}

public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private static readonly Dictionary<string, string> _connectedUsers = new();
    private readonly ILLamaPub _llamaPub;
    private static readonly List<string> responseList = new();
    private ILLamaAdapter _llamaAdapter;
    private static readonly List<string> chatLog = new();
    public ChatHub(IMediator mediator, ILLamaPub lLamaPub, ILLamaAdapter lLamaAdapter)
    {
        _mediator = mediator;
        _llamaPub = lLamaPub;
        _llamaAdapter = lLamaAdapter;
    }

    public void Register(string username)
    {
        _connectedUsers.TryGetValue(username, out string? connectionId);

        if (string.IsNullOrEmpty(connectionId))
        {
            _connectedUsers.Add(username, Context.ConnectionId);
            Clients.Client(Context.ConnectionId).SendAsync("Chat", "DMSBot", "Tôi có thể giúp gì cho bạn?");
        }
        else
        {
            _connectedUsers[username] = Context.ConnectionId;
        }

        Console.WriteLine("Register");
        Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));
    }

    // private static async Task<string?> EncodeText(string text)
    // {
    //     using HttpClient httpClient = new();
    //     var payload = new
    //     {
    //         text = text
    //     };
    //     var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    //     HttpResponseMessage response = await httpClient.PostAsync("http://localhost:9089/vectors", contentData);

    //     if (response.IsSuccessStatusCode)
    //     {
    //         Console.WriteLine($"Text {text} sent successfully.");
    //         JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
    //         if (json.RootElement.TryGetProperty("vector", out var value))
    //         {
    //             return value.ToString();
    //         }
    //     }
    //     else
    //     {
    //         Console.WriteLine($"Failed to send text {text}. Status code: {response.StatusCode}");
    //     }
    //     return null;
    // }

    public async Task Chat(string username, string message, string chatbotId)
    {
        Console.WriteLine("Chat");
        Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));

        // string? searchVector = await EncodeText(message.ToLower());

        // if (searchVector == null)
        // {
        //     return;
        // }

        // var textVector = searchVector.Substring(1, searchVector.Length - 2);
        // var textFloats = textVector.Split(",").Select(float.Parse).ToArray();

        // float[] floats;
        // floats = textFloats;

        await Clients.Caller.SendAsync("Chat", username, message);

        if (message.StartsWith("!"))
        {
            Console.WriteLine($"Getting product with id.");
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 3b0d6f78-e54c-4223-b54b-49a501a4f2dd");

            HttpResponseMessage response = await httpClient.GetAsync($"http://dms4.ext.gpmn.net:8735/api/v1/product-service/product/id?productId={message.Split(":")[1].Replace("'", "")}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get product with id successfully.");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    Console.WriteLine("Empty response.");
                    return;
                }

                Console.WriteLine(JsonSerializer.Serialize(responseContent));
                JsonDocument json = JsonDocument.Parse(responseContent);
                Console.WriteLine(JsonSerializer.Serialize(json));
                if (json.RootElement.TryGetProperty("productDto", out var content))
                {
                    // Console.WriteLine("value");
                    // Console.WriteLine(productDto.ToString());
                    Product? product = new();
                    string temp = "";

                    if (!string.IsNullOrEmpty(content.ToString()))
                    {
                        product = JsonSerializer.Deserialize<Product>(content.ToString());
                        Console.WriteLine("product dto");
                        Console.WriteLine(product);

                        if (product == null)
                        {
                            temp = "[]";
                        }
                        else
                        {
                            // for (int i = 0; i < productDtos?.Length; i++)
                            // {
                            temp += product.Id + ";" + product.Code + ";" + product.Name + ";" + product.Description + "<PRODUCT>";
                            // }
                        }
                    }

                    Console.WriteLine($"Sending {temp} via PROD.");
                    await Clients.Caller.SendAsync("Chat", "PROD", temp);
                    await Clients.Caller.SendAsync("Chat", "LINK", $"http://dms4.ext.gpmn.net:8735/api/v1/product-service/product/id?productId={message.Split(":")[1].Replace("'", "")}<PRODUCT>");
                }
            }
            return;
        }

        if (message.StartsWith("'"))
        {
            Console.WriteLine($"Getting products with the category.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 3b0d6f78-e54c-4223-b54b-49a501a4f2dd");

            HttpResponseMessage response = await httpClient.GetAsync($"http://dms4.ext.gpmn.net:8735/api/v1/product-service/product?page=0&size=10&status=ACT,+PEN&productCategoryIds={message.Replace("'", "")}&isVariant=SIM&isNewProduct=false&searchText");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get products with the category successfully.");
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseContent))
                {
                    Console.WriteLine("Empty response.");
                    return;
                }

                Console.WriteLine(JsonSerializer.Serialize(responseContent));
                JsonDocument json = JsonDocument.Parse(responseContent);
                Console.WriteLine(JsonSerializer.Serialize(json));
                if (json.RootElement.TryGetProperty("content", out var content))
                {
                    Console.WriteLine("value");
                    Console.WriteLine(content.ToString());
                    ProductDto[]? productDtos = new ProductDto[10];
                    string temp = "";

                    if (!string.IsNullOrEmpty(content.ToString()))
                    {
                        productDtos = JsonSerializer.Deserialize<ProductDto[]>(content.ToString());
                        Console.WriteLine("product dto");
                        Console.WriteLine(productDtos);

                        if (productDtos?.Length <= 0)
                        {
                            temp = "[]";
                        }
                        else
                        {
                            for (int i = 0; i < productDtos?.Length; i++)
                            {
                                temp = productDtos[i]?.Product?.Id + ";" + productDtos[i]?.Product?.Code + ";" + productDtos[i]?.Product?.Name + ";" + productDtos[i]?.Product?.Description + "<PRODUCT>";
                            }
                        }
                    }

                    Console.WriteLine($"Sending {temp} via PRODS_IN_CAT.");
                    await Clients.Caller.SendAsync("Chat", "PRODS_IN_CAT", temp);
                }
            }
            return;
        }

        if (message.ToLower().Contains("categories"))
        {
            Console.WriteLine($"Getting product categories.");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 3b0d6f78-e54c-4223-b54b-49a501a4f2dd");
            HttpResponseMessage response = await httpClient.GetAsync("http://dms4.ext.gpmn.net:8735/api/v1/product-service/productCategory?size=10&code=&name=");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get product categories successfully.");
                JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine(JsonSerializer.Serialize(json));
                if (json.RootElement.TryGetProperty("content", out var value))
                {
                    Console.WriteLine("value");
                    Console.WriteLine(value.ToString());
                    Category[]? categories = new Category[10];
                    string temp = "";

                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        categories = JsonSerializer.Deserialize<Category[]>(value.ToString());
                        for (int i = 0; i < categories?.Length; i++)
                        {
                            temp += categories[i].Id + ";" + categories[i].Code + ";" + categories[i].Name + ";" + categories[i].Description + "<CATEGORY>";
                        }
                    }
                    Console.WriteLine($"Sending {temp} via CATS.");
                    await Clients.Caller.SendAsync("Chat", "CATS", temp);
                }
            }
            return;
        }

        if (chatLog.LastOrDefault() == "search")
        {
            Console.WriteLine("GetProductsByCondition");
            Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer 3b0d6f78-e54c-4223-b54b-49a501a4f2dd");

            HttpResponseMessage response = await httpClient.GetAsync($"http://dms4.ext.gpmn.net:8735/api/v1/product-service/product?page=0&size=10&status=ACT,+PEN&productCategoryIds=&isVariant=SIM&isNewProduct=false&searchText={message}");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"GetProductsByCondition with the category successfully.");
                JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine(JsonSerializer.Serialize(json));
                if (json.RootElement.TryGetProperty("content", out var content))
                {
                    Console.WriteLine("value");
                    Console.WriteLine(content.ToString());
                    ProductDto[]? productDtos = new ProductDto[10];
                    string temp = "";

                    if (!string.IsNullOrEmpty(content.ToString()))
                    {
                        productDtos = JsonSerializer.Deserialize<ProductDto[]>(content.ToString());
                        Console.WriteLine("product dto");
                        Console.WriteLine(productDtos);

                        if (productDtos?.Length <= 0)
                        {
                            temp = "[]";
                        }
                        else
                        {
                            for (int i = 0; i < productDtos?.Length; i++)
                            {
                                temp += productDtos[i]?.Product?.Id + ";" + productDtos[i]?.Product?.Code + ";" + productDtos[i]?.Product?.Name + ";" + productDtos[i]?.Product?.Description + "<PRODUCT>";
                            }
                        }
                    }

                    Console.WriteLine($"Sending {temp} via PRODS.");
                    await Clients.Caller.SendAsync("Chat", "PRODS", temp);
                }
            }

            chatLog.Clear();
            return;
        }

        if (message.ToLower().Contains("search"))
        {
            chatLog.Add("search");
            await Clients.Caller.SendAsync("Chat", "DMSBot", "Vui lòng nhập từ khóa cần tìm kiếm.");
            return;
        }

        if (message.StartsWith("/AI "))
        {
            await Clients.Caller.SendAsync("Chat", "DMSBot", "Tôi đang tìm kiếm tài liệu liên quan để trả lời câu truy vấn của bạn (30 - 60s). Trong lúc đợi, bạn có thể thực hiện các truy vấn khác.");
            await LateSend(username, chatbotId, message.Replace("/AI ", ""));
            return;
        }

        var scriptResponse = await _mediator.Send(new ChatRequest(chatbotId, message, "Script"), new CancellationToken());
        if (scriptResponse != null && scriptResponse.Text != null)
        {
            string[] items = scriptResponse.Text.Split("<ITEM>");
            for (int i = 0; i < items.Length - 1; i++)
            {
                await Clients.Caller.SendAsync("Chat", "DMSBot", items[i]);
            }
        }
    }

    public async Task LateSend(string username, string chatbotId, string message)
    {
        Console.WriteLine("LateSend");
        Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));

        var response = await _mediator.Send(new ChatRequest(chatbotId, message, "AI"), new CancellationToken());
        _connectedUsers.TryGetValue(username, out string? connectionId);
        if (!string.IsNullOrEmpty(connectionId))
        {
            Console.WriteLine($"Sending response {response.Text} to {connectionId}.");
            if (response.Text.Contains("ORIGIN:"))
            {
                var texts = response.Text.Split("ORIGIN:");
                if (!string.IsNullOrWhiteSpace(texts[1]))
                {
                    await Clients.Client(connectionId).SendAsync("Chat", "DMSBot", texts[0]);
                    await Clients.Client(connectionId).SendAsync("Chat", "CTX", texts[1]);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync("Chat", "DMSBot", texts[0]);
                }
            }
        }
    }

    public async override Task<Task> OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Register");
        Console.WriteLine($"{Context.ConnectionId} connected");
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception ex)
    {
        Console.WriteLine($"{Context.ConnectionId} disconnected.");
        await base.OnDisconnectedAsync(ex);
    }
}