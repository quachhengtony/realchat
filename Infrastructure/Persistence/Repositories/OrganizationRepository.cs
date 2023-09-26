using Realchat.Application.Repositories;
using Realchat.Domain.Entities;
using Realchat.Infrastructure.Persistence.Contexts;

namespace Realchat.Infrastructure.Persistence.Repositories;

public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(DataContext dataContext) : base(dataContext)
    {

    }
}