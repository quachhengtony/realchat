using MediatR;
using Realchat.Application.Dto;

namespace Realchat.Application.Features.ChatbotFeatures.GetChatbots;

public sealed record GetChatbotsRequest() : IRequest<IEnumerable<ChatbotResponse>>;