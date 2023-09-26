using System.Text;
using System.Text.Json;
using Google.Cloud.Translation.V2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Realchat.Application.LLM;

namespace Realchat.Infrastructure.LLM;

public interface ILLamaPub
{
    public event EventHandler<LLamaEventArgs> OnChange;
    public void Raise(string value);
}

public class LLamaEventArgs : EventArgs
{
    public string Value { get; set; }
    public LLamaEventArgs(string value)
    {
        Value = value;
    }
}

public class LLamaPub : ILLamaPub
{
    // public event Action OnChange = delegate { };
    public event EventHandler<LLamaEventArgs> OnChange = delegate { };

    public void Raise(string value)
    {
        OnChange(this, new LLamaEventArgs(value));
    }
}

public sealed class LLamaAdapter : ILLamaAdapter
{
    private readonly string llamaServiceUrl = "https://localhost:7091/api/llama/inference";
    // private readonly string llamaServiceUrl = "https://0bvg466j-7091.asse.devtunnels.ms/api/llama/inference";
    // private readonly HubConnection _hubConnection;
    private readonly TranslationClient _translationClient;
    private readonly ILLamaPub _llamaPub;
    public LLamaAdapter(ILLamaPub lLamaPub)
    {
        _translationClient = TranslationClient.CreateFromApiKey("");
        // _hubConnection = new HubConnectionBuilder()
        // //   .WithUrl("https://0rxbgslc-7091.asse.devtunnels.ms/inferencehub")
        //   .WithUrl("https://0bvg466j-7091.asse.devtunnels.ms/inferencehub")
        //   .WithAutomaticReconnect()
        //   .Build();
        // _hubConnection.On<string, string>("Inference", Inference);
        // StartLLamaConnection();
        _llamaPub = lLamaPub;
    }

    // private async Task Register()
    // {
    //     await _hubConnection.SendAsync("Register", "Realchat");
    // }

    public void PrintLLamaConnectionStatus()
    {
        // Console.WriteLine("Connection status with LLama is: " + _hubConnection.State.ToString());
    }

    private void Inference(string username, string message)
    {
        _llamaPub.Raise(message.Replace("Context:", "").Replace("DMSBot:", "").Trim());
    }

    public void StartLLamaConnection()
    {
        // Console.WriteLine("Connection status with LLama is: " + _hubConnection.State.ToString());
        // if (_hubConnection.State == HubConnectionState.Disconnected)
        // {
        //     new Action(async () => await _hubConnection.StartAsync())();
        //     Console.WriteLine("Connection with LLama has been successfully started.");
        // }
        // else if (_hubConnection.State == HubConnectionState.Connecting || _hubConnection.State == HubConnectionState.Reconnecting)
        // {
        //     Console.WriteLine("Establishing connection with LLama.");
        // }
        // else
        // {
        //     Console.WriteLine("Connection with LLama is currenly running.");
        // }
    }

    public async Task<string> GetResponse()
    {
        using HttpClient httpClient = new();
        httpClient.Timeout = Timeout.InfiniteTimeSpan;
        bool continueGet = true;
        string translatedResponse = "";

        do
        {
            Console.WriteLine("Getting response...");
            var response = await httpClient.GetAsync("https://0bvg466j-7091.asse.devtunnels.ms/api/llama/get-response");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"LLama inferenced successfully.");
                JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                Console.WriteLine(JsonSerializer.Serialize(json));

                if (json.RootElement.TryGetProperty("text", out var value))
                {
                    Console.WriteLine(value.ToString());
                    if (value.ToString() != "Processing")
                    {
                        continueGet = false;
                    }
                    else
                    {
                        translatedResponse = (await _translationClient.TranslateTextAsync(value.ToString().Replace("Context:", "").Replace("DMSBot:", "").Trim(), LanguageCodes.Vietnamese)).TranslatedText;
                    }
                }
            }
        } while (continueGet == true);
        return translatedResponse;
    }

    public async Task<string> GenerateResponse(string context, string inquiry)
    {
        var translatedContext = (await _translationClient.TranslateTextAsync(context, LanguageCodes.English)).TranslatedText;
        var translatedInquiry = (await _translationClient.TranslateTextAsync(inquiry, LanguageCodes.English)).TranslatedText;

        using HttpClient httpClient = new();
        httpClient.Timeout = Timeout.InfiniteTimeSpan;

        var payload = new
        {
            Context = translatedContext,
            Inquiry = translatedInquiry
        };

        var contentData = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(llamaServiceUrl, contentData);
        // response.EnsureSuccessStatusCode();

        // var responseStream = await response.Content.ReadAsStreamAsync();
        // var reader = new StreamReader(responseStream);

        // while (!reader.EndOfStream)
        // {
        //     var line = await reader.ReadLineAsync();
        //     if (string.IsNullOrEmpty(line))
        //     {
        //         Console.WriteLine($"Failed to inference LLama. Status code: {response.StatusCode}");
        //         return string.Empty;
        //     }
        //     Console.WriteLine($"LLama inferenced successfully.");
        //     Console.WriteLine(line);
        //     return line;
        // }

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"LLama inferenced successfully.");
            JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Console.WriteLine(JsonSerializer.Serialize(json));

            if (json.RootElement.TryGetProperty("text", out var value))
            {
                Console.WriteLine(value.ToString());
                var translatedResponse = (await _translationClient.TranslateTextAsync(value.ToString().Replace("Context:", "").Replace("DMSBot:", "").Trim(), LanguageCodes.Vietnamese)).TranslatedText;
                return translatedResponse;
            }
        }
        // else if ((int)response.StatusCode == 504)
        // {
        //     Console.WriteLine("Getting response...");
        //     response = await httpClient.GetAsync("https://0bvg466j-7091.asse.devtunnels.ms/api/llama/get-response");
        //     if (response.IsSuccessStatusCode)
        //     {
        //         Console.WriteLine($"LLama inferenced successfully.");
        //         JsonDocument json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        //         Console.WriteLine(JsonSerializer.Serialize(json));

        //         if (json.RootElement.TryGetProperty("text", out var value))
        //         {
        //             Console.WriteLine(value.ToString());
        //             var translatedResponse = (await _translationClient.TranslateTextAsync(value.ToString().Replace("Context:", "").Replace("DMSBot:", "").Trim(), LanguageCodes.Vietnamese)).TranslatedText;
        //             return translatedResponse;
        //         }
        //     }
        // }
        else
        {
            Console.WriteLine($"Failed to inference LLama. Status code: {response.StatusCode}");
        }
        return string.Empty;

        // await _hubConnection.SendAsync("Inference", "Realchat", $"{translatedContext}<MID>{translatedInquiry}");
    }
}