using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class KndCmdMsrSndRepository : RepositoryBase<TX55_KndCmdMsrSnd>
    {
        public KndCmdMsrSndRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
