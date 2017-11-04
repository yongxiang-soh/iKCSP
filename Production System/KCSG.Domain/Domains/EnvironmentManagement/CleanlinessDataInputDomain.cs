using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
  public  class CleanlinessDataInputDomain:EnvironmentBaseDomain,ICleanlinessDataInputDomain
    {
        #region 
        public CleanlinessDataInputDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        #endregion
        public ResponseResult<GridResponse<object>> SearchDataWindow1(DateTime inputDate, DateTime dateData, string location, GridSettings gridSettings)
        {
            var lstTe80 = _unitOfWork.EnvMespRepository.GetMany(i => DbFunctions.AddDays(i.F80_D_From, -1) >= inputDate && DbFunctions.AddDays(i.F80_D_To, -1) <= dateData);
            var itemCount = lstTe80.Count();
            var resultModel = new GridResponse<object>(lstTe80, itemCount);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public ResponseResult<GridResponse<object>> SearchDataWindow2(DateTime inputDate, DateTime dateData, string location, GridSettings gridSettings)
        {
            var lstTe80 = _unitOfWork.EnvMespRepository.GetMany(i => DbFunctions.AddDays(i.F80_D_From, -1) >= inputDate && DbFunctions.AddDays(i.F80_D_To, -1) <= dateData);
            var itemCount = lstTe80.Count();
            var resultModel = new GridResponse<object>(lstTe80, itemCount);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }
        
        public int CalculaAll(int refVal, ref double dMean, ref double dSigma, ref double dCp, double?[][] val1M05, IReadOnlyList<double?[]> val1M5,
            double?[][] val5Cm03, double?[][] val5Cm05, double?[][] val5Cm1, double?[][] val5Cm5)
        {
            var calc_val = new List<double>();
            int j;

            j = 0;

            for (int i = 0; i < 10; i++)
            {
                switch (refVal)
                {
                    case 1:
                        if (val1M05[0][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val1M05[0][i].Value);
                        }

                        break;
                    case 2:
                        if (val1M5[0][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val1M5[0][ i].Value);
                        }
                        break;
                    case 3:
                        if (val1M05[1][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val1M05[1][ i].Value);
                        }

                        break;
                    case 4:
                        if (val1M5[1][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val1M5[1][i].Value);
                        }

                        break;
                    case 5:
                        if (val5Cm03[0][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm03[0][i].Value);
                        }

                        break;
                    case 6:
                        if (val5Cm05[0][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm05[0][i].Value);
                        }

                        break;
                    case 7:
                        if (val5Cm1[0][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm1[0][i].Value);
                        }

                        break;
                    case 8:
                        if (val5Cm5[0][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm5[0][i].Value);
                        }

                        break;
                    case 9:
                        if (val5Cm03[1][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm03[1][i].Value);
                        }

                        break;
                    case 10:
                        if (val5Cm05[1][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm05[1][i].Value);
                        }

                        break;
                    case 11:
                        if (val5Cm1[1][ i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add( val5Cm1[1][i].Value);
                        }

                        break;
                    case 12:

                        if (val5Cm5[1][i].HasValue)
                        {
                            j = j + 1;
                            calc_val.Add(val5Cm5[1][i].Value);
                        }
                        break;
                }
            }
            if (j > 0)
            {
                //Calc max
                var mx = calc_val.Max();
               
                var mi = calc_val.Min();
               
                // Calc Range
                var rg = mx - mi;
                // Calc Mean
                var total = calc_val.Sum();
                var tlsqr = calc_val.Sum(i=>i*i);
                var mean = total/j;
                total = total*total;
                // Calc Sigma
                // Modified by Sum (30/10) -- Divide by Zero
                var sigma = 0.0;
                if (j > 1)
                {
                    sigma = Math.Sqrt((tlsqr - (total/j))/(j - 1));
                }
                // Calc Cpk
                var cpk = 0.0;
                if (sigma > 0)
                {
                    cpk = rg/(6*sigma);
                }

                dMean =Math.Round(mean,2);
                dSigma = Math.Round(sigma,2);
                dCp = Math.Round(cpk,2);
            }
            return 0;
        }
    }
}
