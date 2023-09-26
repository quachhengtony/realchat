using System.Text.Json;
using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.ScriptFeatures.CreateScript;

public sealed record ScriptAction
{
    public string? SerializedSay 
    {
        get
        {
            if (Say == null)
            {
                return null;
            }
            else
            {
                return JsonSerializer.Serialize(Say);
            }
        }
    }
    public string[]? Say { get; set; }
}

public sealed record CreateScriptRequest(string TriggerText, ScriptAction Action, Guid ChatbotId) : IRequest<Response>;