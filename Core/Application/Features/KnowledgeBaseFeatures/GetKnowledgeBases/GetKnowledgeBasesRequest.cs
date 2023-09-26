using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.KnowledgeBaseFeatures.GetKnowledgeBases;

public sealed record GetKnowledgeBasesRequest : IRequest<IEnumerable<KnowledgeBaseResponse>>;