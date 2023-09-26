using MediatR;

namespace Realchat.Application.Features.ChatbotFeatures.Chat;

public sealed record ChatRequest(string ChatbotId, string Text, string Mode) : IRequest<ChatResponse>;