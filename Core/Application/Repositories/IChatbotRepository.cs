using Realchat.Application.Repositories;
using Realchat.Domain.Entities;

namespace Realchat.Application.Repositories;

public interface IChatbotRepository : IBaseRepository<Chatbot>
{
    // public Task<Organization?> GetOrganizationById(Guid id);
}