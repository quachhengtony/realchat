using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;

namespace Realchat.Application.Features.OrganizationFeatures.GetOrganizations;

public sealed class GetOrganizationsHandler : BaseHandler, IRequestHandler<GetOrganizationsRequest, IEnumerable<OrganizationResponse>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationsHandler(IOrganizationRepository organizationRepository, IMapper mapper) : base(mapper)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<IEnumerable<OrganizationResponse>> Handle(GetOrganizationsRequest request, CancellationToken cancellationToken)
    {
        var organizations = await _organizationRepository.GetAll(cancellationToken);
        IEnumerable<OrganizationResponse> responses = new List<OrganizationResponse>();
        List<OrganizationResponse> organizationList = responses.ToList();

        foreach (var organization in organizations)
        {
            organizationList.Add(Mapper.Map<OrganizationResponse>(organization));
        }

        responses = organizationList;
        return responses;
    }
}