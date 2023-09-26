using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;

namespace Realchat.Application.Features.KnowledgeBaseFeatures.CreateKnowledgeBase;

public sealed class CreateKnowledgeBaseHandler : BaseHandler, IRequestHandler<CreateKnowledgeBaseRequest, KnowledgeBaseResponse>
{
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
    private readonly IChatbotRepository _chatbotRepository;
    public CreateKnowledgeBaseHandler(IUnitOfWork unitOfWork, IMapper mapper, IKnowledgeBaseRepository knowledgeBaseRepository, IChatbotRepository chatbotRepository) : base(unitOfWork, mapper)
    {
        _knowledgeBaseRepository = knowledgeBaseRepository;
        _chatbotRepository = chatbotRepository;
    }

    public async Task<KnowledgeBaseResponse> Handle(CreateKnowledgeBaseRequest request, CancellationToken cancellationToken)
    {
        var knowledgeBase = Mapper.Map<KnowledgeBase>(request);
        knowledgeBase.Id = Guid.NewGuid();
        knowledgeBase.CreatedTime = DateTime.UtcNow;

        var chatbot = await _chatbotRepository.GetById(request.ChatbotId, cancellationToken);
        if (chatbot != null)
        {
            knowledgeBase.OrganizationId = chatbot.OrganizationId;
        }

        _knowledgeBaseRepository.Create(knowledgeBase);
        await UnitOfWork.Save(cancellationToken);

        return Mapper.Map<KnowledgeBaseResponse>(knowledgeBase);
    }
}