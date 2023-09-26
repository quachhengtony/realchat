using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;

namespace Realchat.Application.Features.OrganizationFeatures.CreateOrganization;

public sealed class CreateOrganizationHandler : BaseHandler, IRequestHandler<CreateOrganizationRequest, OrganizationResponse>
{
    private readonly IOrganizationRepository _organizationRepository;

    public CreateOrganizationHandler(IUnitOfWork unitOfWork, IOrganizationRepository organizationRepository, IMapper mapper) : base(unitOfWork, mapper)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<OrganizationResponse> Handle(CreateOrganizationRequest request, CancellationToken cancellationToken)
    {
        var organization = Mapper.Map<Organization>(request);
        organization.Id = Guid.NewGuid();
        organization.CreatedTime = DateTime.UtcNow;

        _organizationRepository.Create(organization);
        await UnitOfWork.Save(cancellationToken);

        return Mapper.Map<OrganizationResponse>(organization);
    }
}