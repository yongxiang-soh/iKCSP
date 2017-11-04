using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class ProductWarehouseCommandRepository : RepositoryBase<TX47_PdtWhsCmd>
    {
        public ProductWarehouseCommandRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public TX47_PdtWhsCmd InsertProductWarehouseCommand(string commandNo, string commandSeqNo, string commandType,
            string strRtrType, string status, string palletNo, string from, string to, string terminalNo,
            string pictureNo)
        {
            var productWarehouseCommandItem = GetByCommondNoAndSeqNo(commandSeqNo, commandNo);
            if (productWarehouseCommandItem == null)
            {
                productWarehouseCommandItem = new TX47_PdtWhsCmd();
                productWarehouseCommandItem.F47_CommandNo = commandNo;
                productWarehouseCommandItem.F47_CmdSeqNo = commandSeqNo;
                productWarehouseCommandItem.F47_CommandType = commandType;
                productWarehouseCommandItem.F47_StrRtrType = strRtrType;
                productWarehouseCommandItem.F47_Status = status;
                productWarehouseCommandItem.F47_Priority = 0;
                productWarehouseCommandItem.F47_PalletNo = palletNo;
                productWarehouseCommandItem.F47_From = from;
                productWarehouseCommandItem.F47_To = to;
                productWarehouseCommandItem.F47_TerminalNo = terminalNo;
                productWarehouseCommandItem.F47_PictureNo = pictureNo;
                productWarehouseCommandItem.F47_AbnormalCode = "";
                productWarehouseCommandItem.F47_RetryCount = 0;
                productWarehouseCommandItem.F47_AddDate = DateTime.Now;
                productWarehouseCommandItem.F47_UpdateDate = DateTime.Now;
                productWarehouseCommandItem.F47_UpdateCount = 0;

                Add(productWarehouseCommandItem);
            }

            return productWarehouseCommandItem;
        }

        public void UpdateProductWarehouseCommandStatus(TX47_PdtWhsCmd productWarehouseCommand, string status)
        {
            productWarehouseCommand.F47_Status = status;
            productWarehouseCommand.F47_UpdateDate = DateTime.Now;
            Update(productWarehouseCommand);
        }

        public void UpdateCommandNoAndCommandSeqNo(TX47_PdtWhsCmd productWarehouseCommand, string commandNo,
            string commandSeqNo)
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
                            i.F47_CommandNo.Trim().Equals(commandNo.Trim()) &&
                            i.F47_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                    .FirstOrDefault();
        }

        public IQueryable<TX47_PdtWhsCmd> GetByTerminalNoAndPictureNoAndStatus679(string terminalNo, string pictureNo)
        {
            return
                GetMany(
                    i =>
                        i.F47_TerminalNo.Trim().Equals(terminalNo.Trim()) &&
                        i.F47_PictureNo.Trim().Equals(pictureNo.Trim())
                        && i.F47_Status == "6" || i.F47_Status == "7" || i.F47_Status == "9"
                ).OrderBy(i => i.F47_AddDate);
        }

        public IQueryable<TX47_PdtWhsCmd> GetByTerminalNoAndPictureNoAndStatus(string terminalNo, string pictureNo,
            List<string> lstStatus)
        {
            return
                GetMany(
                    i =>
                        i.F47_TerminalNo.Trim().Equals(terminalNo.Trim()) &&
                        i.F47_PictureNo.Trim().Equals(pictureNo.Trim())
                        && lstStatus.Contains(i.F47_Status)
                ).OrderBy(i => i.F47_AddDate);
        }
    }
}