using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class RealtimeRepository : RepositoryBase<RealtimeConnection>
    {
        public RealtimeRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}