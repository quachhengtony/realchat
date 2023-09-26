using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class KnowledgeBaseRepository : BaseRepository<KnowledgeBase>, IKnowledgeBaseRepository
{
    public KnowledgeBaseRepository(DataContext dataContext) : base(dataContext)
    {

    }
}