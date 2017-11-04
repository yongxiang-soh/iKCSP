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
    public class MtrMsrSndCmdRepository : RepositoryBase<TX52_MtrMsrSndCmd>
    {
        public MtrMsrSndCmdRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void insert(string terminalNo, string mccls, string cmd, string abnorcode, string priority, string scrnno, string code, string sts)
        {
            var tx52 = new TX52_MtrMsrSndCmd()
            {
                F52_TerminalNo = terminalNo,
                F52_AddDate = DateTime.Now,
                F52_MsrMacCls = mccls,
                F52_CommandType = cmd,
                F52_Status = sts,
                F52_Priority = priority,
                F52_MasterCode = code,
                F52_PictureNo = scrnno,
                F52_AbnormalCode = abnorcode,
                F52_UpdateDate = DateTime.Now,
            };
            Add(tx52);
        }
    }
}
