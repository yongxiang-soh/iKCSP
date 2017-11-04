using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;
using KCSG.Core.Models;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IStorageOfExternalPreProductDomain
    {
        IQueryable<TX42_KndCmd> GetKneadingComnmand(string preProductCode, string lotNo);
        IQueryable<TX53_OutSidePrePdtStk> GetOutSidePrePdtStks(string palletNo);
        bool CheckKneadingClassAndStatus(string preProductCode, string lotNo);
        bool CheckPalletNo(string palletNo);
        IQueryable<TX40_PdtShfStk> GetProductShelfStock(string palletNo);
        void DeteteProductShelfStock(string palletNo);
        bool CheckOutSidePreProductStockStatus(string palletNo);

        ResponseResult<string> StoringExternalPreProduct(string lotNo, string preProductCode, double quantity, string palletNo,
            string terminalNo,string deviceCode);

        bool CompletingKneadingCommand(string lotNo, string preProductCode);
        double GetRemainingAmount(string palletNo);

        /// <summary>
        /// Respond message from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="preProductCode"></param>
        /// <returns></returns>
        List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo, string lotNo, string preProductCode,
            string is_kndcmdno);

    }
}
