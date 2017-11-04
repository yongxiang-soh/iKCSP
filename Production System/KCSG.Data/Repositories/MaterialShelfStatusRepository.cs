using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;


namespace KCSG.Data.Repositories
{
    public class MaterialShelfStatusRepository : RepositoryBase<TX31_MtrShfSts>
    {
        public MaterialShelfStatusRepository(IKCSGDbContext context)
            : base(context)
        {

        }
        public List<TX31_MtrShfSts> GetShelfStorageMaterial(string liquidFlag)
        {
            var emptyShelf = ((int)Constants.F31_ShelfStatus.EmptyShelf).ToString();
            List<TX31_MtrShfSts> result = null;
            if (liquidFlag.Equals(((int)Constants.TX31_MtrShfSts_LiquidFlag.NonLiquid).ToString()))
            {
                result = GetAll().Where(m => (
                    m.F31_ShelfStatus.Equals(emptyShelf) && m.F31_CmnShfAgnOrd.HasValue
                )).ToList();
            }
            else
            {
                result = GetAll().Where(m => (
                    m.F31_ShelfStatus.Equals(emptyShelf) && m.F31_LqdShfAgnOrd.HasValue
                )).ToList();
            }
            return result;
        }
        public TX31_MtrShfSts Search(string shelfRow, string shelfBay, string shelfLevel)
        {
           return  GetAll().FirstOrDefault(m => (m.F31_ShelfRow.Equals(shelfRow) && m.F31_ShelfBay.Equals(shelfBay) && m.F31_ShelfLevel.Equals(shelfLevel)));

        }
    }
}
