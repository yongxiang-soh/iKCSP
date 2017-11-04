using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface ICalculationOfControlLimitDomain
    {
        ResponseResult<GridResponse<CalculationOfControlLimitItem>> SearchCriteria(GridSettings gridSettings);
    }
}
