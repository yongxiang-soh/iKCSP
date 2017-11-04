using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
namespace KCSG.Data.Repositories.ProductCertification
{
    public class StorageOfProductRepository : RepositoryBase<TX56_TbtPdt>
    {
        public StorageOfProductRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
