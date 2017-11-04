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
    public class TermStatusRepository : RepositoryBase<TM17_TermStatus>
    {
        public TermStatusRepository(IKCSGDbContext context)
            : base(context)
        {
            
        }

        public TM17_TermStatus GetId(string terminalNo)
        {
            return GetAll().FirstOrDefault(i => i.F17_TermNo.Trim().Equals(terminalNo.Trim()));
        }
    }
}
