using AutoMapper;
using MediatR;
using Realchat.Application.Common.Handlers;
using Realchat.Application.Dto;
using Realchat.Application.Features.KnowledgeBaseFeatures.GetKnowledgeBases;
using Realchat.Application.Repositories;

public sealed class GetKnowledgeBasesHandler : BaseHandler, IRequestHandler<GetKnowledgeBasesRequest, IEnumerable<KnowledgeBaseResponse>>
{
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;

    public GetKnowledgeBasesHandler(IUnitOfWork unitOfWork, IMapper mapper, IKnowledgeBaseRepository knowledgeBaseRepository) : base(unitOfWork, mapper)
    {
        _knowledgeBaseRepository = knowledgeBaseRepository;
    }
    public async Task<IEnumerable<KnowledgeBaseResponse>> Handle(GetKnowledgeBasesRequest request, CancellationToken cancellationToken)
    {
        var knowledgeBases = await _knowledgeBaseRepository.GetAll(cancellationToken);
        IEnumerable<KnowledgeBaseResponse> responses = new List<KnowledgeBaseResponse>();
        List<KnowledgeBaseResponse> knowledgeBaseList = responses.ToList();

        foreach (var knowledgeBase in knowledgeBases)
        {
            knowledgeBaseList.Add(Mapper.Map<KnowledgeBaseResponse>(knowledgeBase));
        }

        responses = knowledgeBaseList;
        return responses;
    }
}