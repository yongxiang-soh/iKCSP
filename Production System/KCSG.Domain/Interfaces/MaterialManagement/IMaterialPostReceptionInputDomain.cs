using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IMaterialPostReceptionInputDomain
    {
        MaterialItem GetById(string id);
        ResponseResult<GridResponse<MaterialPostReceptionInputItem>> SearchCriteria(GridSettings gridSettings);
        ResponseResult SavePostReception(MaterialPostReceptionInputItem model);
        bool CheckTotalQuantity(MaterialPostReceptionInputItem item);

        /// <summary>
        /// Find material shelf stocks by using material code 
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        Task<IList<TX33_MtrShfStk>> FindMaterialShelfStocks(string materialCode, string palletNo);
    }
}
