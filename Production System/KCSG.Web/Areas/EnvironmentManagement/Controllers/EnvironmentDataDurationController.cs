using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.EnvironmentDataDuration;
using KCSG.Web.Attributes;
using Newtonsoft.Json;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
     [MvcAuthorize("TCEN012F")]
    public class EnvironmentDataDurationController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;

        #endregion

        public EnvironmentDataDurationController(IEnvironmentBaseDomain environmentBaseDomain)
        {
            this._environmentBaseDomain = environmentBaseDomain;
        }
        //
        // GET: /EnvironmentManagement/EnvironmentDataDuration/
       
        public ActionResult Index()
        {
            var today = DateTime.Now;
            var model = new SearchCriteriaModel()
            {
                EndDate = today.AddDays(-1).ToString("dd/MM/yyyy"),
                StartDate = today.AddDays(-89).ToString("dd/MM/yyyy"),
            };
            ViewBag.ListLocation = _environmentBaseDomain.GetLocationItemByType("1").Select(x => new SelectListItem
            {
                Text = x.F80_Name,
                Value = x.F80_Id.ToString()
            });
            return View(model);
        }
        [HttpPost]
        public ActionResult Search(SearchCriteriaModel model)
        {
            var id = Convert.ToInt32(model.Location);
            var dt1 = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt2 = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (dt2 > DateTime.Now)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The End Date is invalid."
                }, JsonRequestBehavior.AllowGet);
            }

            if ((dt2 - dt1).Days >= 90)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The different of dates is more than 90 days!"
                }, JsonRequestBehavior.AllowGet);
            }

            if (dt2 < dt1)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The Start date is larger than the End date!"
                }, JsonRequestBehavior.AllowGet);
            }

            if (ModelState.IsValid)
            {
                // get and set 
                //	Declare some global variables: 
                //	Boolean:
                //	b_DispHumidity; 
                //	b_TEMPUSL_NULL; 
                //	b_HUMIDUSL_NULL
                //	Float:
                //	T_USL, T_LSL, T_UCL, T_LCL, T_Disp UP, T_Disp LO, T_Cal UP, T_Cal LO, H_USL, H_LSL, H_UCL, H_LCL, H_Disp UP, H_Disp LO, H_Cal UP, H_Cal LO
                var localtion =
                        _environmentBaseDomain.GetLocationItemByType("1")
                            .Where(x => x.F80_Id.ToString().Equals(model.Location))
                            .FirstOrDefault();

                var envMespData = _environmentBaseDomain.GetMespVal(Constants.EnvType.TYPE_RM, int.Parse(model.Location),
                    model.Mode);


                if (!envMespData.Item1)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = -1,//Reset graph and all fields in the form
                        Message = envMespData.Item2
                    }, JsonRequestBehavior.AllowGet);
                }
                var b_TEMPUSL_NULL = false;
                var b_HUMIDUSL_NULL = false;
                if (envMespData.Item3.F80_T_Usl == null)
                {
                    envMespData.Item3.F80_T_Usl = envMespData.Item3.F80_T_Ucl;
                    b_TEMPUSL_NULL = true;
                }

                if (envMespData.Item3.F80_H_Usl == null)
                {
                    envMespData.Item3.F80_H_Usl = envMespData.Item3.F80_H_Ucl;
                    b_HUMIDUSL_NULL = true;
                }

                double t_ucl;
                double t_lcl;
                double t_usl;
                double t_lsl;
                double t_dis_up;
                double h_dis_up;
                double t_dis_lo;
                double h_dis_lo;
                double h_ucl;
                double h_lcl;
                double h_usl;
                double h_lsl;
                double t_cal_up;
                double t_cal_lo;
                double h_cal_up;
                double h_cal_lo;
                double rl_upper;
                double rl_lower;
                double rl_max;
                double rl_min;

                string temp_hi;
                string temp_lo;
                string temp_range;
                string temp_mean;
                string temp_sigma;
                string temp_ucl;
                string temp_lcl;
                string temp_cp;
                string temp_cpk;

                string humid_mean;
                string humid_sigma;
                string humid_hi;
                string humid_lo;
                string humid_range;
                string humid_ucl;
                string humid_lcl;
                string humid_cp;
                string humid_cpk;

                _environmentBaseDomain.SetValueConfigFromTe80(envMespData.Item3, out t_ucl, out t_lcl, out t_usl, out t_lsl, out t_dis_up, out h_dis_up,
                out t_dis_lo, out h_dis_lo, out h_ucl, out h_lcl, out h_usl, out h_lsl, out t_cal_up, out t_cal_lo, out h_cal_up, out h_cal_lo);

                var data = new List<Graphtbl>();

                var t80Evn =
                    _environmentBaseDomain.GetLocationItemByType("1")
                        .Where(x => x.F80_Id == id).FirstOrDefault();
                var b_DispHumidity = false;
                if (t80Evn.F80_Humidity == "0")
                {
                    b_DispHumidity = false;
                }
                else b_DispHumidity = true;

                for (DateTime datetime = dt1; datetime <= dt2; datetime = datetime.AddDays(1))
                {
                    var calcData1 = _environmentBaseDomain.CalcData(Constants.EnvType.TYPE_RM, id, null, datetime,
                    Constants.TypeOfTable.CALC_TE81_TEMP90, 1,
                    model.Mode, t_ucl, t_lcl, t_usl, t_lsl, t_dis_up, h_dis_up, t_dis_lo, h_dis_lo, h_ucl, h_lcl, h_usl,
                    h_lsl, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, b_DispHumidity, data);
                    if (!calcData1)
                    {
                        return Json(new
                        {
                            Success = false,
                            ErrorCode = 0, //Nothing to do
                        }, JsonRequestBehavior.AllowGet);
                    }
                }

                var totalRecord = 0;
                var calcData = _environmentBaseDomain.CalcData(Constants.EnvType.TYPE_RM, id, Constants.TypeOfTable.CALC_TE81_TEMP90, dt1,
                    Constants.TypeOfTable.CALC_TETMP_TEMP90, (dt2 - dt1).Days,
                    model.Mode, t_ucl, t_lcl, t_usl, t_lsl, t_dis_up, h_dis_up, t_dis_lo, h_dis_lo, h_ucl, h_lcl, h_usl,
                    h_lsl, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, b_DispHumidity, data);
                if (!calcData)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = 0, //Nothing to do
                    }, JsonRequestBehavior.AllowGet);
                }
                dt1 = dt1.AddHours(8);
                dt2 = dt2.AddHours(8);
                var tcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_BUFFER,
                    Constants.TypeOfTable.CALC_TETMP_TEMP90, id, dt1, dt2,
                        t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                if (tcalcMean.Item1)
                {
                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TETMP_TEMP90, dt1, tcalcMean.Item2, Constants.EnvFieldNo.FLD_MEAN,
                        data);
                }
                else
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = 0, //Nothing to do
                        Message = tcalcMean.Item2
                    }, JsonRequestBehavior.AllowGet);
                }

                var tsigma = _environmentBaseDomain.CalcSigma(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TETMP_TEMP90, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);

                if (model.Mode == Constants.EnvMode.ControlLine)
                {
                    rl_upper = t_ucl;
                    rl_lower = t_lcl;
                }
                else
                {
                    rl_upper = t_usl;
                    rl_lower = t_lsl;
                }
                _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TETMP_TEMP90, dt1, rl_upper, Constants.EnvFieldNo.FLD_UPPER, data);
                _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TETMP_TEMP90, dt1, rl_lower, Constants.EnvFieldNo.FLD_LOWER, data);

                rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TETMP_TEMP90, id,
                    dt1, dt2, t_dis_up, h_dis_up, data);
                rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TETMP_TEMP90, id,
                    dt1, dt2, t_dis_lo, h_dis_lo, data);


                temp_hi = rl_max.ToString("N2");
                temp_lo = rl_min.ToString("N2");
                temp_range = (rl_max - rl_min).ToString("N2");
                temp_mean = tcalcMean.Item2.ToString("N2");
                temp_sigma = tsigma.Item2.ToString("N2");
                temp_ucl = rl_upper.ToString("N2");
                if (localtion.F80_Name.ToUpper().Contains("WAREHOUSE"))
                {
                    temp_lcl = "";
                }
                else
                {
                    temp_lcl = rl_lower.ToString("N2");
                }
                var rl_cp = 0.0;
                var rl_cpk = 0.0;
                _environmentBaseDomain.CalcCp(Constants.TypeOfTable.CALC_TETMP_TEMP90, tcalcMean.Item2, tsigma.Item2, rl_max - rl_min, t_usl, t_lsl,
                 ref rl_cp, ref rl_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, 0.0, 0.0);
                temp_cp = rl_cp.ToString("F");
                temp_cpk = rl_cpk.ToString("F");

                if (b_DispHumidity)
                {
                    var hcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TETMP_HUMID90, id, dt1, dt2,
                        t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                    if (hcalcMean.Item1)
                    {
                        _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                            Constants.TypeOfTable.CALC_TETMP_HUMID90, dt1, hcalcMean.Item2,
                            Constants.EnvFieldNo.FLD_MEAN,
                            data);
                    }
                    else
                    {
                        var temflds = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TETMP_HUMID90, data);
                        return Json(new
                        {
                            Success = true,
                            temp_hi = temp_hi,
                            temp_lo = temp_lo,
                            temp_range = temp_range,
                            temp_mean = temp_mean,
                            temp_sigma = temp_sigma,
                            temp_cp = temp_cp,
                            temp_cpk = temp_cpk,
                            temp_ucl = temp_ucl,
                            temp_lcl = temp_lcl,
                            tem_fld_data = JsonConvert.SerializeObject(temflds.Item1),
                            tem_fld_mean = JsonConvert.SerializeObject(temflds.Item2),
                            tem_fld_upper = JsonConvert.SerializeObject(temflds.Item3),
                            tem_fld_lower = JsonConvert.SerializeObject(temflds.Item4),
                            tem_max = h_dis_up,
                            tem_min = h_dis_lo,

                            humid_mean = string.Empty,
                            humid_sigma = string.Empty,
                            humid_hi = string.Empty,
                            humid_lo = string.Empty,
                            humid_range = string.Empty,
                            humid_ucl = string.Empty,
                            humid_lcl = string.Empty,
                            humid_cp = string.Empty,
                            humid_cpk = string.Empty,

                        }, JsonRequestBehavior.AllowGet);
                    }
                    var hsigma = _environmentBaseDomain.CalcSigma(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TETMP_HUMID90, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo,
                        data);

                    if (model.Mode == Constants.EnvMode.ControlLine)
                    {
                        rl_upper = h_ucl;
                        rl_lower = h_lcl;
                    }
                    else
                    {
                        rl_upper = h_usl;
                        rl_lower = h_lsl;
                    }

                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TETMP_HUMID90, dt1, rl_upper, Constants.EnvFieldNo.FLD_UPPER,
                        data);
                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TETMP_HUMID90, dt1, rl_lower, Constants.EnvFieldNo.FLD_LOWER,
                        data);

                    // Update the edit box
                    rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM,
                        Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TETMP_HUMID90, id,
                        dt1, dt2, t_dis_up, h_dis_up, data);
                    rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM,
                        Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TETMP_HUMID90, id,
                        dt1, dt2, t_dis_lo, h_dis_lo, data);
                    humid_hi = rl_max.ToString("N2");
                    humid_lo = rl_min.ToString("N2");
                    humid_range = (rl_max - rl_min).ToString("N2");
                    humid_mean = hcalcMean.Item2.ToString("N2");
                    humid_sigma = hsigma.Item2.ToString("N2");
                    humid_ucl = rl_upper.ToString("N2");


                    if (localtion.F80_Name.ToUpper().Contains("WAREHOUSE"))
                    {
                        humid_lcl = "";
                    }
                    else
                    {
                        humid_lcl = rl_lower.ToString("N2");
                    }

                    _environmentBaseDomain.CalcCp(Constants.TypeOfTable.CALC_TETMP_HUMID90, hcalcMean.Item2, hsigma.Item2, rl_max - rl_min, t_usl, t_lsl,
                 ref rl_cp, ref rl_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, 0.0, 0.0);
                    humid_cp = rl_cp.ToString("N2");
                    humid_cpk = rl_cpk.ToString("N2");
                }
                else
                {
                    humid_mean = string.Empty;
                    humid_sigma = string.Empty;
                    humid_hi = string.Empty;
                    humid_lo = string.Empty;
                    humid_range = string.Empty;
                    humid_lcl = string.Empty;
                    humid_cp = string.Empty;
                    humid_cpk = string.Empty;
                    humid_ucl = string.Empty;
                }
                var temflds1 = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TETMP_HUMID90, data);
                var hflds = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TETMP_HUMID90, data);
                return Json(new
                {
                    Success = true,
                    temp_hi = temp_hi,
                    temp_lo = temp_lo,
                    temp_range = temp_range,
                    temp_mean = temp_mean,
                    temp_sigma = temp_sigma,
                    temp_ucl = temp_ucl,
                    temp_lcl = temp_lcl,
                    temp_cp = temp_cp,
                    temp_cpk = temp_cpk,

                    tem_fld_data = JsonConvert.SerializeObject(temflds1.Item1),
                    tem_fld_mean = JsonConvert.SerializeObject(temflds1.Item2),
                    tem_fld_upper = JsonConvert.SerializeObject(temflds1.Item3),
                    tem_fld_lower = JsonConvert.SerializeObject(temflds1.Item4),
                    tem_yAxis = JsonConvert.SerializeObject(temflds1.Item5),
                    tem_max = t_dis_up,
                    tem_min = t_dis_lo,

                    humid_mean = humid_mean,
                    humid_sigma = humid_sigma,
                    humid_hi = humid_hi,
                    humid_lo = humid_lo,
                    humid_range = humid_range,
                    humid_lcl = humid_lcl,
                    humid_ucl = humid_ucl,
                    humid_cp = humid_cp,
                    humid_cpk = humid_cpk,
                    h_fld_data = JsonConvert.SerializeObject(hflds.Item1),
                    h_fld_mean = JsonConvert.SerializeObject(hflds.Item2),
                    h_fld_upper = JsonConvert.SerializeObject(hflds.Item3),
                    h_fld_lower = JsonConvert.SerializeObject(hflds.Item4),
                    h_yAxis = JsonConvert.SerializeObject(hflds.Item5),
                    h_max = h_dis_up,
                    h_min = h_dis_lo

                }, JsonRequestBehavior.AllowGet);

            }
            return Json(new
            {
                Success = false,
            }, JsonRequestBehavior.AllowGet);

        }
    }
}