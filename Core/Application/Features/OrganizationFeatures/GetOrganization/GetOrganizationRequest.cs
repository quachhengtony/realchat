using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.OrganizationFeatures.GetOrganization;

public sealed record GetOrganizationRequest(Guid Id) : IRequest<OrganizationResponse>;