using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;

namespace Realchat.Application.Features.OrganizationFeatures.GetOrganization;

public sealed class GetOrganizationHandler : BaseHandler, IRequestHandler<GetOrganizationRequest, OrganizationResponse>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationHandler(IOrganizationRepository organizationRepository, IMapper mapper) : base(mapper)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<OrganizationResponse> Handle(GetOrganizationRequest request, CancellationToken cancellationToken)
    {
        var organization = await _organizationRepository.GetById(request.Id, cancellationToken);
        return Mapper.Map<OrganizationResponse>(organization);
    }
}