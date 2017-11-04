using KCSG.Core.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface ISubMaterialDomain
    {
        SubMaterialItem GetById(string id);
        void Delete(string code);
      
         ResponseResult<GridResponse<SubMaterialItem>> SearchCriteria(string code, GridSettings gridSettings);
         ResponseResult CreateOrUpdate(SubMaterialItem model);

         bool CheckUnique(string f01_MaterialCode);
    }
}
