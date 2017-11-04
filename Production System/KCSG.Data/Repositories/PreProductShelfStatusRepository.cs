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
    public class PreProductShelfStatusRepository : RepositoryBase<TX37_PrePdtShfSts>
    {
        public PreProductShelfStatusRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public TX37_PrePdtShfSts GetByRowBayLevel(string row, string bay, string level)
        {
            return
                GetAll()
                    .FirstOrDefault(i => i.F37_ShelfRow == row && i.F37_ShelfBay == bay && i.F37_ShelfLevel == level);
        }
    }
}
