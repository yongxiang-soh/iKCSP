using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IMaterialDomain
    {
        MaterialItem GetById(string id);
        void Create(MaterialItem material);
        void Update(MaterialItem material);
        void Delete(string id);
        bool CheckUnique(string materialCode);
        ResponseResult CreateOrUpdate(MaterialItem model);
        ResponseResult<GridResponse<MaterialItem>> SearchCriteria(string code, GridSettings gridSettings);
        IEnumerable<TM01_Material> GetMaterials(string materialCode);
        
    }
}
