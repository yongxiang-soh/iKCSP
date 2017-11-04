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
    public class ProductShelfStatusRepository : RepositoryBase<TX51_PdtShfSts>
    {
        public ProductShelfStatusRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void UpdateProductShelfStatus(TX51_PdtShfSts productShelfStatus, string shelfStatus, string stockTalkingFlag, string terminalNo)
        {
            productShelfStatus.F51_ShelfStatus = shelfStatus;
            productShelfStatus.F51_StockTakingFlag = stockTalkingFlag;
            productShelfStatus.F51_TerminalNo = terminalNo;
            productShelfStatus.F51_StorageDate = DateTime.Now;
            productShelfStatus.F51_UpdateDate = DateTime.Now;
            Update(productShelfStatus);
        }

        public void UpdateProductShelfStatus(TX51_PdtShfSts productShelfStatus, string shelfStatus, string terminalNo)
        {
            productShelfStatus.F51_ShelfStatus = shelfStatus;
            productShelfStatus.F51_TerminalNo = terminalNo;
            productShelfStatus.F51_UpdateDate = DateTime.Now;
            Update(productShelfStatus);
        }
        public void UpdateStockTalkingFlagInProductShelfStatus(TX51_PdtShfSts productShelfStatus, string stockTalkingFlag, string terminalNo)
        {
            productShelfStatus.F51_StockTakingFlag = stockTalkingFlag;
            productShelfStatus.F51_TerminalNo = terminalNo;
            productShelfStatus.F51_UpdateDate = DateTime.Now;
            Update(productShelfStatus);
        }


        public TX51_PdtShfSts GetByRowBayLevel(string ls_shelfrow, string ls_shelfbay, string ls_shelflevel)
        {
            return
                GetAll()
                    .FirstOrDefault(
                        i =>
                            i.F51_ShelfRow == ls_shelfrow && i.F51_ShelfBay == ls_shelfbay &&
                            i.F51_ShelfLevel == ls_shelflevel);
        }

        public IQueryable<TX51_PdtShfSts> GetByShelfStatusAndShelfType(string shelfStatus, string shelfType)
        {
            return GetMany(i => i.F51_ShelfStatus == shelfStatus && i.F51_ShelfType == shelfType);
        }
    }
}
