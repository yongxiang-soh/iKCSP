using System.Linq;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class MaterialWarehouseCommandRepository : RepositoryBase<TX34_MtrWhsCmd>
    {
        public MaterialWarehouseCommandRepository(IKCSGDbContext context) : base(context)
        {
            
        }

        public TX34_MtrWhsCmd GetMtrWhsCmdByKey(string commandNo, string mdSeqNo)
        {
            return
                GetAll()
                    .FirstOrDefault(
                        i =>
                            i.F34_CommandNo.Trim().Equals(commandNo.Trim()) &&
                            i.F34_CmdSeqNo.Trim().Equals(mdSeqNo.Trim()));
        }
    }
}
