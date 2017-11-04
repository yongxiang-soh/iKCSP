using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class ProductWarehouseCommand:RepositoryBase<TX47_PdtWhsCmd>
    {
        public ProductWarehouseCommand(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertProductWarehouseCommand(string commandNo, string commandSeqNo, string commandType, string strRtrType, string status, string palletNo, string from, string to, string terminalNo, string pictureNo)
        {
            var productWarehouseCommand = new TX47_PdtWhsCmd();
            productWarehouseCommand.F47_CommandNo = commandNo;
            productWarehouseCommand.F47_CmdSeqNo = commandSeqNo;
            productWarehouseCommand.F47_CommandType = commandType;
            productWarehouseCommand.F47_StrRtrType = strRtrType;
            productWarehouseCommand.F47_Status = status;
            productWarehouseCommand.F47_Priority = 0;
            productWarehouseCommand.F47_PalletNo = palletNo;
            productWarehouseCommand.F47_From = from;
            productWarehouseCommand.F47_To = to;
            productWarehouseCommand.F47_TerminalNo = terminalNo;
            productWarehouseCommand.F47_PictureNo = pictureNo;
            productWarehouseCommand.F47_RetryCount = 0;
            productWarehouseCommand.F47_AddDate = DateTime.Now;
            productWarehouseCommand.F47_UpdateDate = DateTime.Now;

            Add(productWarehouseCommand);
        }

        public void UpdateProductWarehouseCommandStatus(TX47_PdtWhsCmd productWarehouseCommand, string status)
        {
          
            productWarehouseCommand.F47_Status = status;
            productWarehouseCommand.F47_UpdateDate = DateTime.Now;
            Update(productWarehouseCommand);    
        }

        public void UpdateCommandNoAndCommandSeqNo(TX47_PdtWhsCmd productWarehouseCommand, string commandNo, string commandSeqNo)
        {
            
            productWarehouseCommand.F47_CommandNo = commandNo;
            productWarehouseCommand.F47_CmdSeqNo = commandSeqNo;
            Update(productWarehouseCommand);  
        }

        public TX47_PdtWhsCmd GetByCommondNoAndSeqNo(string commandNo, string cmdSeqNo)
        {
            return
                GetMany(
                    i =>
                        i.F47_CommandNo.Trim().Equals(commandNo.Trim()) && i.F47_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                    .FirstOrDefault();
        }
    }
}
