using KCSG.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;

namespace KCSG.Data.Repositories
{
    public class OutSidePrePdtStkRepository : RepositoryBase<TX53_OutSidePrePdtStk>
    {
        public OutSidePrePdtStkRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
