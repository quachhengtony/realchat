using Realchat.Domain.Entities;

namespace Realchat.Application.Repositories;

public interface IInformationChunkRepository : IBaseRepository<InformationChunk>
{
    public Task<InformationChunk?> GetByKnowledgeBaseIdAndChunkNumber(Guid knowledgeBaseId, int chunkNumber);
}