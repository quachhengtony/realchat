using Microsoft.EntityFrameworkCore;
using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class ChatbotRepository : BaseRepository<Chatbot>, IChatbotRepository
{
    public ChatbotRepository(DataContext dataContext) : base(dataContext)
    {

    }

    // public async Task<Organization?> GetOrganizationById(Guid id)
    // {
    //     var chatbot = await _dataContext.Set<Chatbot>().Where(o => o.Id == id).FirstOrDefaultAsync();
    //     if (chatbot == null)
    //     {
    //         return default;
    //     }
    //     return _dataContext.Set<Orga>
    // }

}