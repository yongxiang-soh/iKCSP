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
    public class PreProductWarehouseCommandHistoryRepository : RepositoryBase<TH63_PrePdtWhsCmdHst>
    {
        public PreProductWarehouseCommandHistoryRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void AddByPreproductCmd(TX50_PrePdtWhsCmd tx50PrePdtWhsCmd)
        {
            var th63 = new TH63_PrePdtWhsCmdHst()
            {
                F63_AbnormalCode = tx50PrePdtWhsCmd.F50_AbnormalCode,
                F63_AddDate = tx50PrePdtWhsCmd.F50_AddDate,
                F63_CmdSeqNo = tx50PrePdtWhsCmd.F50_CmdSeqNo,
                F63_CommandEndDate = tx50PrePdtWhsCmd.F50_CommandEndDate,
                F63_CommandNo = tx50PrePdtWhsCmd.F50_CommandNo,
                F63_CommandSendDate = tx50PrePdtWhsCmd.F50_CommandEndDate,
                F63_CommandType = tx50PrePdtWhsCmd.F50_CommandType,
                F63_From = tx50PrePdtWhsCmd.F50_From,
                F63_ContainerNo = tx50PrePdtWhsCmd.F50_ContainerNo,
                F63_ContainerCode = tx50PrePdtWhsCmd.F50_ContainerCode,
                F63_PictureNo = tx50PrePdtWhsCmd.F50_PictureNo,
                F63_Priority = tx50PrePdtWhsCmd.F50_Priority,
                F63_RetryCount = tx50PrePdtWhsCmd.F50_RetryCount,
                F63_Status = tx50PrePdtWhsCmd.F50_Status,
                F63_StrRtrType = tx50PrePdtWhsCmd.F50_StrRtrType,
                F63_TerminalNo = tx50PrePdtWhsCmd.F50_TerminalNo,
                F63_To = tx50PrePdtWhsCmd.F50_To,
                F63_UpdateCount = tx50PrePdtWhsCmd.F50_UpdateCount,
                F63_UpdateDate = tx50PrePdtWhsCmd.F50_UpdateDate

            };
            Add(th63);

        }
    }
}
