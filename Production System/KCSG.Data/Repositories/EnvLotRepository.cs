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
    public class EnvLotRepository : RepositoryBase<Te84_Env_Lot>
    {
        public EnvLotRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
