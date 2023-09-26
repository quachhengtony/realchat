using MediatR;
using Microsoft.AspNetCore.Mvc;
using Realchat.Application.Dto;
using Realchat.Application.Features.ScriptFeatures.CreateScript;

namespace Realchat.WebApi.Controllers;

[Route("api/scripts")]
[ApiController]
public class ScriptsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScriptsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Response>> Create([FromBody] CreateScriptRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }
}