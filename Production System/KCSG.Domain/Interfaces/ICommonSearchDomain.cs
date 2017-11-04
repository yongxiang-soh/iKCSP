using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces
{
    public interface ICommonSearchDomain
    {
        ResponseResult<GridResponse<CommonSearchItem>> GetSupplierCodes(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetProductCode(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetMaterialCode(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetPreproductCode(string code, GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> GetSupMatCode(string code,
            GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> GetEndUserCode(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetShippingNo(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetProductLotNo(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetProductLotNoWithProductCode(string code, GridSettings gridSettings, string productCode = null);
        ResponseResult<GridResponse<CommonSearchItem>> GetPONo(string materialCode,string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetContainerType(string code, GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> GetMaterialPalletNo(string code,
            GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> GetTabletingLine(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetShelfNo(string code, GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> GetProductPalletNo(string code,
            GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetOutOfPlanProduct(string code,
            GridSettings gridSettings);
        ResponseResult<GridResponse<CommonSearchItem>> GetPalletNoWithStockFlag(string code,GridSettings gridSettings);

        ResponseResult<GridResponse<CommonSearchItem>> FindProductLabelList(string code, GridSettings gridSettings);
    }
}
