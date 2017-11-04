using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IPckMtrDomain
    {
        PckMtrItem GetById(string id);
        IEnumerable<PckMtrItem> GetAll();
        void Create(PckMtrItem pckMtr);
        void Update(PckMtrItem pckMtr);
        bool Delete(string productCode, string subMaterialCode);
        TM11_PckMtr GetPckMtr(string id, string subMaterialCode);
        ResponseResult<GridResponse<TM11_PckMtr>> SearchCriteria(string productCode, GridSettings gridSettings);
        ResponseResult<PckMtrItem> CreateOrUpdate(PckMtrItem model);
       

        bool CheckUnique(string f11_SubMaterialCode, string f11_ProductCode);
    }
}
