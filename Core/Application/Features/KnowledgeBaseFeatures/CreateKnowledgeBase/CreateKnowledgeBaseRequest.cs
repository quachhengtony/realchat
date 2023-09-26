using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.KnowledgeBaseFeatures.CreateKnowledgeBase;

// public sealed record CreateKnowledgeBaseRequest(string Name, Guid ChatbotId, Guid OrganizationId) : IRequest<KnowledgeBaseResponse>;
public sealed record CreateKnowledgeBaseRequest(string Name, Guid ChatbotId) : IRequest<KnowledgeBaseResponse>;