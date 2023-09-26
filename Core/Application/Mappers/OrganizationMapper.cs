using AutoMapper;
using Realchat.Application.Dto;
using Realchat.Application.Features.OrganizationFeatures.CreateOrganization;
using Realchat.Domain.Entities;

namespace Realchat.Application.Mappers;

public sealed class OrganizationMapper : Profile
{
    public OrganizationMapper()
    {
        CreateMap<CreateOrganizationRequest, Organization>();
        CreateMap<Organization, OrganizationResponse>();
    }
}