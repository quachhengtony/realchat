using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(DataContext dataContext) : base(dataContext)
    {

    }

    public Task<User> GetByEmail(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}