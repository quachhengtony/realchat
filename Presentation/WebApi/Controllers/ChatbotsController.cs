using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Realchat.Application.Dto;
using Realchat.Application.Features.ChatbotFeatures.Chat;
using Realchat.Application.Features.ChatbotFeatures.CreateChatbot;
using Realchat.Application.Features.ChatbotFeatures.GetChatbots;
using Realchat.Application.LLM;
using Realchat.WebApi.Hubs;

namespace Realchat.WebApi.Controllers;

[Route("api/chatbots")]
[ApiController]
public class ChatbotsController : ControllerBase
{
    private readonly IHubContext<ChatHub> _chatHub;
    private readonly IMediator _mediator;

    public ChatbotsController(IHubContext<ChatHub> chatHub, IMediator mediator)
    {
        _chatHub = chatHub;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ChatbotResponse>> Create([FromBody] CreateChatbotRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatbotResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetChatbotsRequest(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("chat")]
    public async Task<ActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}