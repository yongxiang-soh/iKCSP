using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class PreProductRepository : RepositoryBase<TM03_PreProduct>
    {
        public PreProductRepository(IKCSGDbContext context) : base(context)
        {
        }
    }
}
