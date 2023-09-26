using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.ChatbotFeatures.CreateChatbot;

public sealed record CreateChatbotRequest(string DisplayName, Guid OrganizationId) : IRequest<ChatbotResponse>;