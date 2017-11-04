using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface IControlLimitEditDomain
    {
        ResponseResult<GridResponse<CalculationOfControlLimitItem>> SearchCriteria(string location,GridSettings gridSettings);
        void Update(ControlLimitEditItem item);
    }
}
