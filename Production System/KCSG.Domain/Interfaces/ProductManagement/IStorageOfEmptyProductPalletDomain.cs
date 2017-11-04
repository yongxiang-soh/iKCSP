using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IStorageOfEmptyProductPalletDomain
    {
        bool CheckPalletNumber(int palletLoadNumber);
        TM14_Device CheckMaxiumNumberOfIpAddress();
        void StoreTheEmptyPallet(int palletLoadNumber, string terminalNo);

        /// <summary>
        /// Respond message sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo);

        bool CheckProductShelfStatus();
    }
}
