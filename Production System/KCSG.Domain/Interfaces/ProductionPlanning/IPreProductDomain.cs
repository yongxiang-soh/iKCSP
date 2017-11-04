using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IPreProductDomain
    {
        PreProductItem GetById(string id);
        TM03_PreProduct GetPreProduct(string id);
     
        void Create(PreProductItem material);
        void Update(PreProductItem material);
        void Delete(string preProductCode);
        bool CheckUnique(string preProductCode);
        ResponseResult CreateOrUpdate(PreProductItem model);
        ResponseResult<GridResponse<PrintPreProductItem>> SearchMaterialList(string request, GridSettings gridSettings);
        ResponseResult<GridResponse<PreProductItem>> Search(string request, GridSettings gridSettings);
        TM03_PreProduct SetColorClass(TM03_PreProduct model);
        TM03_PreProduct SetKneadingLine(TM03_PreProduct model);
        ResponseResult<GridResponse<PrintPreProductItem>> MaterialListPrint(string request);
    }
}
