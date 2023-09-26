using Realchat.Domain.Entities;

namespace Realchat.Application.Storage;

public interface IMilvusAdapter
{
    public Task ImportDataToMilvus(string vectorChunk, Guid organizationId, Guid chatbotId, Guid knowledgeBaseId, int i);
    public Task ImportScriptToMilvus(string vectorChunk, Guid organizationId, Guid chatbotId, Guid scriptId);
    public Task<(Guid, int)> Search(string chatbotId, string searchString);
    public Task<Guid> SearchScript(string chatbotId, string searchString);
}