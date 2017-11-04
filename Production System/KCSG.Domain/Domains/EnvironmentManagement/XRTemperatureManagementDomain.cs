using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using DocumentFormat.OpenXml.Drawing;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class XRTemperatureManagementDomain:EnvironmentBaseDomain,IXRTemperatureManagement
    {
        

        public XRTemperatureManagementDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }

        public ResponseResult<XRTemperatureItem> Search(string productCode, Constants.EnvMode envMode,
            DateTime startDate, DateTime endDate)
        {
            var th85 =
                _unitOfWork.EnvProdRepository.GetMany(i => i.F85_Code.Trim().Equals(productCode.Trim()))
                    .FirstOrDefault();
            var dt1 = DateTime.Now;

            var dt2 = new DateTime();
            var rl_avg = 0.0;
            var total_rec_cnt = 0;
            var rl_cp = 0.0;
            var rl_cpk = 0.0;
            var lstTable = new List<Graphtbl>();

            var res = CalcProdlot(productCode, Constants.TypeOfTable.CALC_LOT_AVG, ref dt1, ref dt2,
                ref lstTable);
            if (res == -1)
            {
                return new ResponseResult<XRTemperatureItem>(null, true);
            }
            var ttlt = res;
          rl_avg =   CalcMean(Constants.TypeOfTable.CALC_LOT_AVG, Constants.TypeOfTable.CALC_LOT_AVG,
                (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0, 0.0, 0.0, lstTable).Item2;
            var resSigma = CalcSigma(Constants.TypeOfTable.CALC_LOT_AVG, Constants.TypeOfTable.CALC_LOT_AVG,
                (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0, 0.0, 0.0, lstTable);
            var rl_sigma = resSigma.Item2;
            var max = GetMax(Constants.EnvType.TYPE_LT, Constants.TypeOfTable.CALC_LOT_AVG,
                Constants.TypeOfTable.CALC_LOT_AVG, (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0,
                lstTable);
            var min = GetMin(Constants.EnvType.TYPE_LT, Constants.TypeOfTable.CALC_LOT_AVG,
                Constants.TypeOfTable.CALC_LOT_AVG, (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0,
                lstTable);

            CalcCp(Constants.TypeOfTable.CALC_LOT_AVG, rl_avg, rl_sigma, max - min, th85.F85_M_Usl??0, th85.F85_M_Lsl??0,
               ref rl_cp,ref rl_cpk, false, false, 0.0, 0.0);
            var repone = new XRTemperatureItem();
            var data = GrapData(Constants.TypeOfTable.CALC_LOT_AVG, lstTable);
            repone.tblTemp = data.Item1;
            repone.TimeTemp = data.Item5;
            repone.HighTemp = max.ToString("f");
            repone.LowTemp = min.ToString("f");
            repone.MeanTemp = rl_avg.ToString("f");
            repone.SigmaTemp = rl_sigma.ToString("f");
            repone.RangeTemp = (max - min).ToString("f");
            repone.UCLTemp = (rl_avg + 3*rl_sigma).ToString("f");
            repone.LCLTemp = (rl_avg - 3*rl_sigma).ToString("f");
            repone.CpTemp = rl_cp.ToString("f");
            repone.CpkTemp = rl_cpk.ToString("f");
            var lbl = 0;
            if (ttlt > 150)
            {
                lbl = 20;
            }
            if (ttlt > 100)
            {
                lbl = 10;
            }
            if (ttlt > 50)
            {
                lbl = 5;
            }
            if (ttlt > 10)
            {
                lbl = 2;
            }
            else
            {
                lbl = 1;
            }

           var d_min = min -1;
            if (d_min < 0)
            {
                d_min = 0;
            }
           

// Update Values
    var lt_range =Math.Round(max - min, 3);
var a_ucl = Math.Round(rl_avg + 3*rl_sigma, 3);
var a_lcl = Math.Round(rl_avg - 3*rl_sigma, 3);
            var te85 = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code == productCode);
            if (te85!=null)
            {
                te85.F85_T_High = max;
                te85.F85_T_Low = min;
                te85.F85_T_Ucl = a_ucl;
                te85.F85_T_Lcl = a_lcl;
                te85.F85_T_Cpk = rl_cpk;
                te85.F85_T_Cp = rl_cp;
                te85.F85_T_Range = lt_range;
                te85.F85_T_Mean = rl_avg;
                te85.F85_T_Sigma = rl_sigma;
                te85.F85_No_Lot = ConvertHelper.ToInteger(ttlt);
                _unitOfWork.EnvProdRepository.Update(te85);
            }
            
            rl_avg =   CalcMean(Constants.TypeOfTable.CALC_LOT_RANGE, Constants.TypeOfTable.CALC_LOT_AVG,
                (int) Constants.TypeOfTable.CALC_LOT_RANGE, dt1, dt2,0.0, 0.0, 0.0, 0.0, lstTable).Item2;
                 resSigma = CalcSigma(Constants.TypeOfTable.CALC_LOT_RANGE, Constants.TypeOfTable.CALC_LOT_AVG,
                (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0, 0.0, 0.0, lstTable);
                 rl_sigma = resSigma.Item2;

// Draw for Range

    max = GetMax(Constants.EnvType.TYPE_LT, Constants.TypeOfTable.CALC_LOT_RANGE,
                Constants.TypeOfTable.CALC_LOT_AVG, (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0,
                lstTable);
             min = GetMin(Constants.EnvType.TYPE_LT, Constants.TypeOfTable.CALC_LOT_RANGE,
                Constants.TypeOfTable.CALC_LOT_AVG, (int) Constants.TypeOfTable.CALC_LOT_AVG, dt1, dt2, 0.0, 0.0,
                lstTable);
            CalcCp(Constants.TypeOfTable.CALC_LOT_RANGE, rl_avg, rl_sigma, max - min, th85.F85_M_Usl??0, th85.F85_M_Lsl??0,
               ref rl_cp,ref rl_cpk, false, false, 0.0, 0.0);
            foreach (var item in lstTable)
            {
                item.buf_type = Constants.TypeOfTable.CALC_LOT_RANGE;
            }
            data = GrapData(Constants.TypeOfTable.CALC_LOT_RANGE, lstTable);
            repone.HighHumid = max.ToString("F");
            repone.LowHumid = min.ToString("f");
            repone.MeanHumid = rl_avg.ToString("F");
            repone.SigmaHumid = rl_sigma.ToString("f");
            repone.RangeHumid = (max - min).ToString("f");
            repone.UCLHumid = (rl_avg + 3*rl_sigma).ToString("f");
            repone.LCLHumid = (rl_avg - 3*rl_sigma).ToString("F");
            repone.CpHumid = rl_cp.ToString("F");
            repone.CpkHumid = rl_cpk.ToString("F");
            repone.tblHumid = data.Item1;
            repone.TimeHumid = data.Item5;
// Draw graph for average temperature
            d_min = min - 1;
            if (d_min < 0)
            {
                d_min = 0;
            }
            lt_range = Math.Round(max - min, 3);
            a_ucl = Math.Round(rl_avg + 3*rl_sigma, 3);
            a_lcl = Math.Round(rl_avg - 3*rl_sigma, 3);
             te85 = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code == productCode);
            if (te85!=null)
            {
                te85.F85_R_High = max;
                te85.F85_R_Low = min;
                te85.F85_R_Ucl = a_ucl;
                te85.F85_R_Lcl = a_lcl;
                te85.F85_R_Cpk = rl_cpk;
                te85.F85_R_Cp = rl_cp;
                te85.F85_R_Range = lt_range;
                te85.F85_R_Mean = rl_avg;
                te85.F85_R_Sigma = rl_sigma;
                _unitOfWork.EnvProdRepository.Update(te85);
            }
            _unitOfWork.Commit();
            return new ResponseResult<XRTemperatureItem>(repone, true);
        }

        public SelectList GetProduct()
        {
            return new SelectList(_unitOfWork.ProductRepository.GetAll(), "F09_ProductCode", "F09_ProductDesp");
        }
        
    }
}
