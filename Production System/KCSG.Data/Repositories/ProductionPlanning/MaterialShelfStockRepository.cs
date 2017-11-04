using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories.ProductionPlanning
{
    public class MaterialShelfStockRepository : RepositoryBase<TX33_MtrShfStk>
    {
        public MaterialShelfStockRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
