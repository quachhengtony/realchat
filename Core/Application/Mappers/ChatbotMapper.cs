using AutoMapper;
using Realchat.Application.Dto;
using Realchat.Application.Features.ChatbotFeatures.CreateChatbot;
using Realchat.Domain.Entities;

namespace Realchat.Application.Mappers;

public sealed class ChatbotMapper : Profile
{
    public ChatbotMapper()
    {
        CreateMap<CreateChatbotRequest, Chatbot>();
        CreateMap<Chatbot, ChatbotResponse>();
    }
}