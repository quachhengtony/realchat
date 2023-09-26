using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.OrganizationFeatures.GetOrganizations;

public sealed record GetOrganizationsRequest() : IRequest<IEnumerable<OrganizationResponse>>;