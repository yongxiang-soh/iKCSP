using System.Collections.Generic;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IRetrievalOfProductPalletDomain
    {
        /// <summary>
        ///     Find possible retrieval quantity.
        /// </summary>
        /// <returns></returns>
        double GetPossibleRetrievalQuantity();

        /// <summary>
        ///     Retrieve empty pallet.
        /// </summary>
        /// <param name="terminalNo"></param>
        void Retrieval(string terminalNo);

        /// <summary>
        /// Handle result after messages sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        IList<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo);
    }
}