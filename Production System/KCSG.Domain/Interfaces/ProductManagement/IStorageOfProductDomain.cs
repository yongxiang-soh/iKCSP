using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IStorageOfProductDomain
    {
        ResponseResult<GridResponse<OutOfPlanProductItem>> ShowData(GridSettings gridSettings);
        ResponseResult<GridResponse<StorageOfProductItem>> SearchCriteria(GridSettings gridSettings);

        List<SelectingItem> GetSelected(string lstValue, int status);
        bool CheckedPalletNo(string palletNo);
        double GetTotalAmountOfTX40(string palletNo);
        bool CheckOutSidePrePdtStk(string palletNo);

        ResponseResult UpdaDateCreateAndDelete(StoreProductItem item, string terminalNo);

        /// <summary>
        ///     Responding Reply From C3 Rules:
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="item"></param>
        List<ThirdCommunicationResponse> ProcessDataReceiveMessageForC3(string terminalNo, StoreProductItem item);
    }
}