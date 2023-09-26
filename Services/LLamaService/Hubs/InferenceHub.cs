using Microsoft.AspNetCore.SignalR;
using Realchat.Services.LLamaService.Services;
using System.Text.Json;

namespace Realchat.Services.LLamaService.Hubs
{
    public class InferenceHub : Hub
    {
        private readonly ILLamaService _llamaService;
        
        public InferenceHub(ILLamaService llamaService) 
        {
            _llamaService = llamaService;
        }

        //private static readonly Dictionary<string, string> _connectedUsers = new();

        //public void Register(string username)
        //{
        //    _connectedUsers.TryGetValue(username, out string? connectionId);
        //    if (string.IsNullOrEmpty(connectionId))
        //    {
        //        _connectedUsers.Add(username, Context.ConnectionId);
        //    }
        //    else
        //    {
        //        _connectedUsers[username] = Context.ConnectionId;
        //    }

        //    Console.WriteLine("LLama register");
        //    Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));
        //}

        public void Inference(string username, string message)
        {
            string response = _llamaService.GenerateResponse(message.Split("<MID>")[0], message.Split("<MID>")[1]);
            //_connectedUsers.TryGetValue(username, out string? connectionId);
            Console.WriteLine("LLama inference");
            //Console.WriteLine(JsonSerializer.Serialize(_connectedUsers));
            //if (!string.IsNullOrEmpty(connectionId))
            //{
                Console.WriteLine($"Sending response {response} to {Context.ConnectionId}");
            //Clients.Client(connectionId).SendAsync("Inference", "LLama", response);
            Clients.All.SendAsync("Inference", "LLama", response);
            //}
        }

        public async override Task<Task> OnConnectedAsync()
        {
            //await Clients.Caller.SendAsync("Register");

            Console.WriteLine($"{Context.ConnectionId} connected.");
            return base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"{Context.ConnectionId} disconnected.");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
