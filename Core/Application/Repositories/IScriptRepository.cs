using Realchat.Domain.Entities;

namespace Realchat.Application.Repositories;

public interface IScriptRepository : IBaseRepository<Script>
{
    public Task<Script?> GetByTriggerTextAndChatbotId(string triggerText, Guid chatbotId);
}