using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;

namespace Realchat.Application.Features.ChatbotFeatures.GetChatbots;

public sealed class GetChatbotsHandler : BaseHandler, IRequestHandler<GetChatbotsRequest, IEnumerable<ChatbotResponse>>
{
    private readonly IChatbotRepository _chatbotRepository;
    public GetChatbotsHandler(IUnitOfWork unitOfWork, IMapper mapper, IChatbotRepository chatbotRepository) : base(unitOfWork, mapper)
    {
        _chatbotRepository = chatbotRepository;
    }

    public async Task<IEnumerable<ChatbotResponse>> Handle(GetChatbotsRequest request, CancellationToken cancellationToken)
    {
        var chatbots = await _chatbotRepository.GetAll(cancellationToken);
        IEnumerable<ChatbotResponse> responses = new List<ChatbotResponse>();
        List<ChatbotResponse> chatbotList = responses.ToList();

        foreach (var chatbot in chatbots)
        {
            chatbotList.Add(Mapper.Map<ChatbotResponse>(chatbot));
        }

        responses = chatbotList;
        return responses;
    }
}