using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IStockTakingOfProductDomain
    {
        /// <summary>
        /// Find list of stock taking of product asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<StockTakingOfProductItem>>> FindStockTakingOfProductAsync(
            string startShelfRow, string startShelfBay, string startShelfLevel,
            string endShelfRow, string endShelfBay, string endShelfLevel,
            GridSettings gridSettings);

        /// <summary>
        /// Find product details asynchronously.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        Task<IList<StockTakingOfProductDetailItem>> FindProductDetailAsync(string palletNo);

        /// <summary>
        /// Retrieve product details asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<ResponseResult<RetrievalItem>> RetrieveProductDetailsAsync(string palletNo, string row, string bay, string level, string terminalNo);

        /// <summary>
        /// Find products stock details.
        /// </summary>
        /// <param name="shelfRow"></param>
        /// <param name="shelfBay"></param>
        /// <param name="shelfLevel"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        Task<object> FindProductConfirmDetails(string shelfRow, string shelfBay, string shelfLevel, string palletNo);

        /// <summary>
        /// Respond data after retrieve button is clicked.
        /// </summary>
        /// <param name="terminalNo"></param>
        IList<ThirdCommunicationResponse> RespondingReplyFromC3RetrieveProduct(string terminalNo);

        /// <summary>
        /// Respond 
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        IList<ThirdCommunicationResponse> RespondingReplyFromC3RestoreProduct(string terminalNo, string row, string bay, string level);

        /// <summary>
        /// Restorage product asynchronously.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="palletNo"></param>
        /// <param name="items"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        Task RestoreProductsAsync(string terminalNo, string palletNo,
            IList<StockTakingOfProductConfirmItem> items,
            string row, string bay, string level);
    }
}