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
    public class ConveyorRepository :RepositoryBase<TM05_Conveyor>
    {
        public ConveyorRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public TM05_Conveyor GetbyTerminerNo(string terminerNo)
        {
          return  GetAll().FirstOrDefault(i => i.F05_TerminalNo.Trim().Equals(terminerNo.Trim()));
        }
        
    }
}
