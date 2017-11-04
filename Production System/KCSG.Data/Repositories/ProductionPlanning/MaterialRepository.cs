using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class MaterialRepository : RepositoryBase<TM01_Material>
    {
        public MaterialRepository(IKCSGDbContext context) : base(context)
        {
        }
    }
}
