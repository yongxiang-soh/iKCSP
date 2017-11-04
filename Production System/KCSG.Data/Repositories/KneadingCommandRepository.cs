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
    public class KneadingCommandRepository : RepositoryBase<TX42_KndCmd>
    {
        public KneadingCommandRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertKneadingCommand(string kneadingCommandNo, string preProductLotNo, string preProductCode, string outSiteClass, string status, double throwAmount)
        {
            var kneadingCommand = new TX42_KndCmd();
            kneadingCommand.F42_KndCmdNo = kneadingCommandNo;
            kneadingCommand.F42_PrePdtLotNo = preProductLotNo;
            kneadingCommand.F42_PreProductCode = preProductCode;
            kneadingCommand.F42_KndEptBgnDate = DateTime.Now.Date;
            kneadingCommand.F42_OutSideClass = outSiteClass;
            kneadingCommand.F42_Status = status;
            kneadingCommand.F42_ThrowAmount = throwAmount;
            kneadingCommand.F42_StgCtnAmt = 1;
            kneadingCommand.F42_BatchEndAmount = 0;
            kneadingCommand.F42_KndCmdBookNo = 0;
            kneadingCommand.F42_LotSeqNo = 0;
            kneadingCommand.F42_CommandSeqNo = 0;
            kneadingCommand.F42_MtrRtrFlg = "0";
            kneadingCommand.F42_AddDate = DateTime.Now;
            kneadingCommand.F42_UpdateDate = DateTime.Now;

            Add(kneadingCommand);
        }

        public void UpdateKneadingCommand(TX42_KndCmd kneadingCommand, double throwAmount, int stgCtnAmt)
        {
           
            kneadingCommand.F42_ThrowAmount = throwAmount;
            kneadingCommand.F42_StgCtnAmt = stgCtnAmt;
            kneadingCommand.F42_UpdateDate = DateTime.Now;

            Update(kneadingCommand);
        }
    }
}
