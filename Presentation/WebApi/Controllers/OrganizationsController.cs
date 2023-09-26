using MediatR;
using Microsoft.AspNetCore.Mvc;
using Realchat.Application.Dto;
using Realchat.Application.Features.OrganizationFeatures.CreateOrganization;
using Realchat.Application.Features.OrganizationFeatures.GetOrganization;
using Realchat.Application.Features.OrganizationFeatures.GetOrganizations;
using Realchat.Application.Storage;
using Realchat.Infrastructure.Storage;

namespace Realchat.WebApi.Controllers;

[Route("api/organizations")]
[ApiController]
public class OrganizationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMinioAdapter _minioAdapter;

    public OrganizationsController(IMediator mediator, IMinioAdapter minioAdapter)
    {
        _mediator = mediator;
        _minioAdapter = minioAdapter;
    }

    [HttpPost]
    public async Task<ActionResult<OrganizationResponse>> Create([FromBody] CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOrganizationsRequest(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrganizationResponse>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetOrganizationRequest(id), cancellationToken);
        return Ok(response);
    }
}