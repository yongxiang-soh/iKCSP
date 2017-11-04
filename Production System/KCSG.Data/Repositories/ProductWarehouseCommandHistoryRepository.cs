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
    public class ProductWarehouseCommandHistoryRepository : RepositoryBase<TH66_PdtWhsCmdHst>
    {
        public ProductWarehouseCommandHistoryRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertWithTx47(TX47_PdtWhsCmd tx47PdtWhsCmd)
        {
            var preProductHouseHistory = new TH66_PdtWhsCmdHst()
            {
                F66_AbnormalCode = tx47PdtWhsCmd.F47_AbnormalCode,
                F66_AddDate = tx47PdtWhsCmd.F47_AddDate,
                F66_CmdSeqNo = tx47PdtWhsCmd.F47_CmdSeqNo,
                F66_CommandEndDate = tx47PdtWhsCmd.F47_CommandEndDate,
                F66_CommandNo = tx47PdtWhsCmd.F47_CommandNo,
                F66_CommandSendDate = tx47PdtWhsCmd.F47_CommandEndDate,
                F66_CommandType = tx47PdtWhsCmd.F47_CommandType,
                F66_From = tx47PdtWhsCmd.F47_From,
                F66_PalletNo = tx47PdtWhsCmd.F47_PalletNo,
                F66_PictureNo = tx47PdtWhsCmd.F47_PictureNo,
                F66_Priority = tx47PdtWhsCmd.F47_Priority,
                F66_RetryCount = tx47PdtWhsCmd.F47_RetryCount,
                F66_Status = tx47PdtWhsCmd.F47_Status,
                F66_StrRtrType = tx47PdtWhsCmd.F47_StrRtrType,
                F66_TerminalNo = tx47PdtWhsCmd.F47_TerminalNo,
                F66_To = tx47PdtWhsCmd.F47_To,
                F66_UpdateCount = tx47PdtWhsCmd.F47_UpdateCount,
                F66_UpdateDate = tx47PdtWhsCmd.F47_UpdateDate
            };
            Add(preProductHouseHistory);
        }
    }
}
