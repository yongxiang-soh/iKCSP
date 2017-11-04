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
    public class PreProductWarehouseCommandRepository : RepositoryBase<TX50_PrePdtWhsCmd>
    {
        public PreProductWarehouseCommandRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public TX50_PrePdtWhsCmd GetByCmdNoAndSeqNo(string commandNo, string cmdSeqNo)
        {
            return
                GetAll()
                    .FirstOrDefault(
                        i =>
                            i.F50_CommandNo.Trim().Equals(commandNo.Trim()) &&
                            i.F50_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()));
        }

        public void AddCommand(string to, int cmdSeqNo, string containerNo, string containerCode, string from,
            string terminalNo, string pictureNo, string commandNo, Constants.F50_StrRtrType strRtrType,
            string commmType)
        {
            var commandSeqNo = cmdSeqNo.ToString("D4");
            var tx50prepdtprepdtwhscmd =
                Get(i => i.F50_CommandNo.Trim().Equals(commandNo.Trim()) && i.F50_CmdSeqNo.Trim().Equals(commandSeqNo));
            if (tx50prepdtprepdtwhscmd == null)
            {
                // Find the current time on the server.
                var systemTime = DateTime.Now;
                // Please refer to SQL16.
                var prepdtprepdtwhscmd = new TX50_PrePdtWhsCmd();
                prepdtprepdtwhscmd.F50_CommandNo = commandNo;
                prepdtprepdtwhscmd.F50_CmdSeqNo = cmdSeqNo.ToString("D4");
                prepdtprepdtwhscmd.F50_CommandType = commmType;
                prepdtprepdtwhscmd.F50_StrRtrType = strRtrType.ToString("D");
                prepdtprepdtwhscmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                prepdtprepdtwhscmd.F50_ContainerNo = containerNo;
                prepdtprepdtwhscmd.F50_ContainerCode = containerCode;
                prepdtprepdtwhscmd.F50_Priority = 0;
                prepdtprepdtwhscmd.F50_From = from;
                prepdtprepdtwhscmd.F50_To = to;
                prepdtprepdtwhscmd.F50_CommandSendDate = systemTime;
                prepdtprepdtwhscmd.F50_CommandEndDate = systemTime;
                prepdtprepdtwhscmd.F50_TerminalNo = terminalNo;
                prepdtprepdtwhscmd.F50_PictureNo = pictureNo;
                prepdtprepdtwhscmd.F50_AddDate = systemTime;
                prepdtprepdtwhscmd.F50_UpdateDate = systemTime;
                prepdtprepdtwhscmd.F50_UpdateCount = 0;
                prepdtprepdtwhscmd.F50_RetryCount = 0;

                Add(prepdtprepdtwhscmd);
            }
        }
    }
}