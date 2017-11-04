using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IStorageOfSupplementaryMaterialDomain
    {
        StorageOfSupplementaryMaterialItem GetById(string id);
        void Create(SubMaterialItem supMaterial);
        void Update(SubMaterialItem supMaterial);
        void Delete(string id);
        bool CheckUnique(string supMaterialCode);
        ResponseResult CreateOrUpdate(StorageOfSupplementaryMaterialItem model);
        ResponseResult<GridResponse<SubMaterialItem>> SearchCriteria(string code, GridSettings gridSettings);
        IEnumerable<TM15_SubMaterial> GetSupMaterials(string supMaterialCode);
    }
}
