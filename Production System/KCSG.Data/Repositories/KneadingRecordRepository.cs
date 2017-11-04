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
    public class KneadingRecordRepository : RepositoryBase<TX43_KndRcd>
    {
        public KneadingRecordRepository(IKCSGDbContext context)
            : base(context)
        {
        }
    }
}
