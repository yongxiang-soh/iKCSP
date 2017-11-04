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
    public class EnvAvalRepository : RepositoryBase<Te82_Env_Aval>
    {
        public EnvAvalRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
