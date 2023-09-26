using AutoMapper;
using Realchat.Application.Dto;
using Realchat.Application.Features.KnowledgeBaseFeatures.CreateKnowledgeBase;
using Realchat.Domain.Entities;

namespace Realchat.Application.Mappers;

public sealed class KnowledgeBaseMapper : Profile
{
    public KnowledgeBaseMapper()
    {
        CreateMap<CreateKnowledgeBaseRequest, KnowledgeBase>();
        CreateMap<KnowledgeBase, KnowledgeBaseResponse>();
    }
}