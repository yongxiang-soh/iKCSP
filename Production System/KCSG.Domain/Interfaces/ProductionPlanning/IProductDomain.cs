using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IProductDomain
    {
        ProductItem GetById(string id);
        void Create(ProductItem product);
        void Update(ProductItem product);
        void Delete(string lstCode);
        bool CheckUnique(string productCode);
        ResponseResult CreateOrUpdate(ProductItem model);
        ResponseResult<GridResponse<ProductItem>> SearchPrint(string code, GridSettings gridSettings);
        ResponseResult<GridResponse<TM09_Product>> SearchCriteria(string code, GridSettings gridSettings);
        ProductItem GetByCode(string code);
    }
}
