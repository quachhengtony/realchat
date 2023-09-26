using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.OrganizationFeatures.CreateOrganization;

public sealed record CreateOrganizationRequest(string DisplayName) : IRequest<OrganizationResponse>;