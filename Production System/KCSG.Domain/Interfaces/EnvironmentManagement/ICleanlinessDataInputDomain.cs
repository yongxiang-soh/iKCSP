using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface ICleanlinessDataInputDomain
    {
        ResponseResult<GridResponse<object>> SearchDataWindow1(DateTime inputDate, DateTime dateData, string location,
            GridSettings gridSettings);
        ResponseResult<GridResponse<object>> SearchDataWindow2(DateTime inputDate, DateTime dateData, string location,
            GridSettings gridSettings);

        int CalculaAll(int refVal, ref double dMean, ref double dSigma, ref double dCp, double?[][] val1M05,
            IReadOnlyList<double?[]> val1M5,
            double?[][] val5Cm03, double?[][] val5Cm05, double?[][] val5Cm1, double?[][] val5Cm5);

    }
}
