using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IRetrievalOfExternalProductDomain
    {
        /// <summary>
        /// Find external pre-products asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<ExternalPreProductItem>>> FindExternalPreProductsAsync(
            GridSettings gridSettings);

        /// <summary>
        /// Find table listing line.
        /// </summary>
        /// <returns></returns>
        Task<TM14_Device> FindDeviceTableListingLine(string tableListingLine);
        
        /// <summary>
        /// Retrieval external pre-product asynchronously.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="commandNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="tableListingLine"></param>
        /// <returns></returns>
        Task RetrievalExternalPreProductAsync(string terminalNo, string commandNo, string preProductCode, string prePdtLotNo, string tableListingLine);

        /// <summary>
        /// Reply messages sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="kneadingCommandNo"></param>
        /// <param name="preProductLotNo"></param>
        /// <returns></returns>
        IList<ThirdCommunicationResponse> Reply(string terminalNo, string preProductCode, string kneadingCommandNo,
            string preProductLotNo);

        string GetTabletingLine(string Line);
    }
}