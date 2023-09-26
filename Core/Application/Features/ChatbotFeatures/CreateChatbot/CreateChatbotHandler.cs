using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;

namespace Realchat.Application.Features.ChatbotFeatures.CreateChatbot;

public sealed class CreateChatbotHandler : BaseHandler, IRequestHandler<CreateChatbotRequest, ChatbotResponse>
{
    private readonly IChatbotRepository _chatbotRepository;

    public CreateChatbotHandler(IUnitOfWork unitOfWork, IChatbotRepository chatbotRepository, IMapper mapper) : base(unitOfWork, mapper)
    {
        _chatbotRepository = chatbotRepository;
    }

    public async Task<ChatbotResponse> Handle(CreateChatbotRequest request, CancellationToken cancellationToken)
    {
        var chatbot = Mapper.Map<Chatbot>(request);
        chatbot.Id = Guid.NewGuid();
        chatbot.CreatedTime = DateTime.UtcNow;

        _chatbotRepository.Create(chatbot);
        await UnitOfWork.Save(cancellationToken);

        return Mapper.Map<ChatbotResponse>(chatbot);
    }
}