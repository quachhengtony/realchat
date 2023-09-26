using MediatR;
using Microsoft.AspNetCore.Mvc;
using Realchat.Application.Features.KnowledgeBaseFeatures.CreateKnowledgeBase;
using Realchat.Application.Features.KnowledgeBaseFeatures.GetKnowledgeBases;
using Realchat.Application.Features.KnowledgeBaseFeatures.UploadFile;

namespace Realchat.WebApi.Controllers;

[Route("api/knowledgebases")]
[ApiController]
public class KnowledgeBasesController : ControllerBase
{
    private readonly IMediator _mediator;
    public KnowledgeBasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateKnowledgeBaseRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetKnowledgeBasesRequest(), cancellationToken);
        return Ok(response);
    }

    [HttpPost("{knowledgeBaseId}/upload-file")]
    public async Task<ActionResult> UploadFile([FromForm] IFormFile file, [FromRoute] Guid knowledgeBaseId, CancellationToken cancellationToken)
    {
        using Stream fileStream = file.OpenReadStream();
        var response = await _mediator.Send(new UploadFileRequest(knowledgeBaseId, file.FileName, fileStream), cancellationToken);
        return Ok(response);
    }
}