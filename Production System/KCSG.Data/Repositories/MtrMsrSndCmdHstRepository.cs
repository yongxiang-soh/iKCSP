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
    public class MtrMsrSndCmdHstRepository : RepositoryBase<TH68_MtrMsrSndCmdHst>
    {
        public MtrMsrSndCmdHstRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
