using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
    public interface IPreProductPlanDomain
    {
        PreProductPlanItem GetById(string date, string code);
        void Delete(DateTime date,string code);
        ResponseResult<GridResponse<PreProductPlanItem>> SearchCriteria(string date, GridSettings gridSettings);
        bool CheckUnique(string productCode,DateTime yearMonth);
        ResponseResult CreateOrUpdate(PreProductPlanItem model);
    }
}
