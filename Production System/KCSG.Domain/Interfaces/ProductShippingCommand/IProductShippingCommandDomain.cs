using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductShippingCommand
{
    public interface IProductShippingCommandDomain
    {
        /// <summar>
        /// Find list of product shipping commands.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<ProductShippingCommandItem>>> FindProductShippingCommandsAsync(
            string shippingNo, GridSettings gridSettings);

        /// <summary>
        /// Get list of product shipping command details.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="palletNo"></param>
        /// <param name="productLotNo"></param>
        /// <returns></returns>
        ResponseResult<GridResponse<object>> FindProductShippingCommandDetailsAsync(GridSettings gridSettings, string palletNo, string productLotNo, string productNo,double? RequestAmount,
            string shelfNo);


        /// <summary>
        /// Unassign a specific product shipping command.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        Task UnassignProduct(string shippingNo, string terminalNo);

        /// <summary>
        /// Assign a specific product shipping command.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="shippingQuantity"></param>
        /// <param name="shippedAmount"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<AssignProductShippingCommandResult> AssignProductAsync(string shippingNo, string productCode,
            string productLotNo, double shippingQuantity, double shippedAmount, string terminalNo);

        /// <summary>
        /// Retrieve  a specific product shipping command.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="terminalNo"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        ResponseResult RetrieveProductAsync(string shippingNo, string productCode, string productLotNo, string row, string bay, string level, string terminalNo,string palletNo);

        /// <summary>
        /// Find product shipping commands list for printing.
        /// </summary>
        /// <param name="shippingNo"></param>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<object> FindProductShippingCommandsForPrinting(string shippingNo, string productCode, string lstPalletNo, string productNo, string productLotNo,
            GridSettings gridSettings,string shelfNo);

       
        /// <summary>
        /// Responding Reply FromC3
        /// </summary>
        /// <param name="terminalNo"></param>
        List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo,string productCode);
    }
}