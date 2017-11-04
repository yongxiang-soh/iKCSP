using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
   public interface IMaterialRequirementListDomain
    {
       ResponseResult<GridResponse<object>> SearchCriteria(DateTime? yearMonthTime, GridSettings gridSettings);

       ResponseResult<GridResponse<object>> Search(GridSettings gridSettings);
    }

}
