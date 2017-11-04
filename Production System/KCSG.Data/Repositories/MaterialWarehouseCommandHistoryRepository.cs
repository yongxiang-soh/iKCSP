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
    public class MaterialWarehouseCommandHistoryRepository : RepositoryBase<TH60_MtrWhsCmdHst>
    {
        public MaterialWarehouseCommandHistoryRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public TH60_MtrWhsCmdHst AddByMaterialCmd(TX34_MtrWhsCmd tx34)
        {
            var materialHouseHistory = new TH60_MtrWhsCmdHst()
            {
                F60_AbnormalCode = tx34.F34_AbnormalCode,
                F60_AddDate = tx34.F34_AddDate,
                F60_CmdSeqNo = tx34.F34_CmdSeqNo,
                F60_CommandEndDate = tx34.F34_CommandEndDate,
                F60_CommandNo = tx34.F34_CommandNo,
                F60_CommandSendDate = tx34.F34_CommandEndDate,
                F60_CommandType = tx34.F34_CommandType,
                F60_From = tx34.F34_From,
                F60_PalletNo = tx34.F34_PalletNo,
                F60_PictureNo = tx34.F34_PictureNo,
                F60_Priority = tx34.F34_Priority,
                F60_RetryCount = tx34.F34_RetryCount,
                F60_Status = tx34.F34_Status,
                F60_StrRtrType = tx34.F34_StrRtrType,
                F60_TerminalNo = tx34.F34_TerminalNo,
                F60_To = tx34.F34_To,
                F60_UpdateCount = tx34.F34_UpdateCount,
                F60_UpdateDate = tx34.F34_UpdateDate
            };
            Add(materialHouseHistory);
            return materialHouseHistory;
        }
    }
}
