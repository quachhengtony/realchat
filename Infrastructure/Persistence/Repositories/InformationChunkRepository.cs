using Microsoft.EntityFrameworkCore;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class InformationChunkRepository : BaseRepository<InformationChunk>, IInformationChunkRepository
{
    public InformationChunkRepository(DataContext dataContext) : base(dataContext)
    {

    }

    public async Task<InformationChunk?> GetByKnowledgeBaseIdAndChunkNumber(Guid knowledgeBaseId, int chunkNumber)
    {
        var informationChunk = await _dataContext.Set<InformationChunk>().Where(x => x.KnowledgeBaseId == knowledgeBaseId && x.ChunkNumber == chunkNumber).FirstOrDefaultAsync();
        return informationChunk;
    }
}