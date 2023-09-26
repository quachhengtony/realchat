using Microsoft.EntityFrameworkCore;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class ScriptRepository : BaseRepository<Script>, IScriptRepository
{
    public ScriptRepository(DataContext dataContext) : base(dataContext)
    {

    }

    public async Task<Script?> GetByTriggerTextAndChatbotId(string triggerText, Guid chatbotId)
    {
        return await _dataContext.Set<Script>().Where(s => s.ChatbotId == chatbotId && s.TriggerText.ToLower().Contains(triggerText.ToLower().Trim())).FirstOrDefaultAsync();
    }
}