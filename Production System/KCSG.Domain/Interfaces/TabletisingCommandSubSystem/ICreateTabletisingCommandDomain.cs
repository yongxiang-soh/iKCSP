using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Enumerations;
using KCSG.Domain.Models.Tabletising;
using KCSG.Domain.Models.TabletisingCommondSubSystem;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.TabletisingCommandSubSystem
{
    public interface ICreateTabletisingCommandDomain
    {
        /// <summary>
        /// Find neading command by using specific conditions.
        /// </summary>
        Task<FindTabletisingKneadingCommandItem> RetrieveTabletisingKneadingCommand(int page, int records);

        /// <summary>
        /// Search kneading commands and gridize it.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<object>>> SearchTabletisingKneadingCommands(GridSettings gridSettings);

        /// <summary>
        /// Check whether the item is valid for delete or not.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        Task<bool> IsValidDeleteItem(TabletisingKneadingCommandItem tabletisingKneadingCommandItem);

        /// <summary>
        /// This function is for deleting kneading command item.
        /// </summary>
        /// <param name="tabletisingKneadingCommandItem"></param>
        /// <returns></returns>
        void DeleteTabletisingKneadingCommand(TabletisingKneadingCommandItem tabletisingKneadingCommandItem);

        /// <summary>
        /// This function is for searching tablet command
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="preproductCode"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<SearchProductInformationItem>>> SearchProductInformation(GridSettings gridSettings,
            string preproductCode);


        /// <summary>
        /// Search product details and return a grid back.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="commandNo"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="preProductCode"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<object>>> SearchProductDetails(GridSettings gridSettings, string commandNo, string prePdtLotNo, string preProductCode);

        /// <summary>
        /// Save production planning.
        /// </summary>
        /// <param name="item"></param>
        Task<InitiateTabletisingCommandResult> InitiateTabletisingCommand(InitiateTabletisingCommandViewModel item);

        /// <summary>
        /// This function is for validating product planning item.
        /// </summary>
        /// <param name="kneadingNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<string> ValidateProductPlanning(string kneadingNo, string lotNo, double quantity);

        /// <summary>
        /// Update product quantity. (Refer BR8 - Create tabletising)
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="quantity"></param>
        void Update(string productCode, double quantity);

        /// <summary>
        /// Search product shelf statuses.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<object>>> SearchProductShelfStatuses(GridSettings gridSettings,string productCode,string lotNo);

        /// <summary>
        /// Search pre product shelf statuses
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<object>>> SearchPreProductShelfStatuses(
            GridSettings gridSettings, string productCode, string lotNo);

        /// <summary>
        /// Get list Product Details
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        IList<ProductLowerTable> GetProductDetails(string productCode, string productName);
    }
}