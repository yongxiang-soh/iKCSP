using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Web.Attributes;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.EnvironmentDataOneDay;
using Newtonsoft.Json;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN011F")]
    public class EnvironmentDataOneDayController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;

        #endregion

        public EnvironmentDataOneDayController(IEnvironmentBaseDomain environmentBaseDomain)
        {
            this._environmentBaseDomain = environmentBaseDomain;
        }

        //
        // GET: /EnvironmentManagement/EnvironmentDataOneDay/
        public ActionResult Index(string environmentDate = "", Constants.EnvMode envMode = Constants.EnvMode.ControlLine, string location ="" )
        {
            ViewBag.ListLocation = _environmentBaseDomain.GetLocationItemByType("1").Select(x => new SelectListItem
            {
                Text = x.F80_Name,
                Selected = x.F80_Name == location,
                Value = x.F80_Id.ToString()
            });
            DateTime dt1;
            if (DateTime.TryParseExact(environmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,out dt1))
            {
                var model = new EnvironmentDataOneDayViewModel()
                {
                    EnvironmentDate = dt1.ToString("dd/MM/yyyy"),
                    EnvMode = envMode
                };
                return View(model);
            }
            else
            {
                var model = new EnvironmentDataOneDayViewModel()
                {
                    EnvironmentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    EnvMode = envMode
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Search(EnvironmentDataOneDayViewModel model)
        {
            var id = Convert.ToInt32(model.Location);
            var dt1 = DateTime.ParseExact(model.EnvironmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt2 = dt1.AddDays(1);
            if (dt1 > DateTime.Now)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,//Reset graph and all fields in the form
                    Message = "This date cannot bigger than current date"
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

                var envMespData = _environmentBaseDomain.GetMespVal(Constants.EnvType.TYPE_RM, int.Parse(model.Location),
                    model.EnvMode);

                if (!envMespData.Item1)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = -1,//Reset graph and all fields in the form
                        Message = envMespData.Item2
                    }, JsonRequestBehavior.AllowGet);
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

                string humid_mean;
                string humid_sigma;
                string humid_hi;
                string humid_lo;
                string humid_range;

                var data = new List<Graphtbl>();
                _environmentBaseDomain.SetValueConfigFromTe80(envMespData.Item3, out t_ucl, out t_lcl, out t_usl,
                    out t_lsl, out t_dis_up, out h_dis_up,
                    out t_dis_lo, out h_dis_lo, out h_ucl, out h_lcl, out h_usl, out h_lsl, out t_cal_up,
                    out t_cal_lo, out h_cal_up, out h_cal_lo);
                Constants.EnvDMent ctrl_ln = Constants.EnvDMent.MODDT;
                var t80Evn =
                    _environmentBaseDomain.GetLocationItemByType("1")
                        .Where(x => x.F80_Id == id).FirstOrDefault();
                var b_DispHumidity = false;
                if (t80Evn.F80_Humidity == "0")
                {
                    b_DispHumidity = false;
                }
                else b_DispHumidity = true;
                var totalRecord = 0;
                var calcData = _environmentBaseDomain.CalcData(Constants.EnvType.TYPE_RM, id, null, dt1, Constants.TypeOfTable.CALC_TE81_TEMP, 1,
                    model.EnvMode, t_ucl, t_lcl, t_usl, t_lsl, t_dis_up, h_dis_up, t_dis_lo, h_dis_lo, h_ucl, h_lcl, h_usl,
                    h_lsl, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, b_DispHumidity, data);
                if (!calcData)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = 0, //Nothing to do
                    }, JsonRequestBehavior.AllowGet);
                }
                var tcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_TEMP, id, dt1, dt2,
                        t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                if (tcalcMean.Item1)
                {
                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TE81_TEMP, dt1, tcalcMean.Item2, Constants.EnvFieldNo.FLD_MEAN,
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
                        Constants.TypeOfTable.CALC_TE81_TEMP, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);

                if (model.EnvMode == Constants.EnvMode.ControlLine)
                {
                    rl_upper = t_ucl;
                    rl_lower = t_lcl;
                }
                else
                {
                    rl_upper = t_usl;
                    rl_lower = t_lsl;
                }
                rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_TEMP, id, dt1, dt2, t_dis_up, h_dis_up, data);
                rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_TEMP, id,
                    dt1, dt2, t_dis_lo, h_dis_lo, data);

                _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1, rl_upper, Constants.EnvFieldNo.FLD_UPPER, data);
                _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1, rl_lower, Constants.EnvFieldNo.FLD_LOWER, data);

                temp_hi = rl_max.ToString("N2");
                temp_lo = rl_min.ToString("N2");
                temp_range = (rl_max - rl_min).ToString("N2");
                temp_mean = tcalcMean.Item2.ToString("N2");
                temp_sigma = tsigma.Item2.ToString("N2");
                if (b_DispHumidity)
                {
                    var hcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TE81_HUMID, id, dt1, dt2,
                        t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                    if (hcalcMean.Item1)
                    {
                        _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                            Constants.TypeOfTable.CALC_TE81_HUMID, dt1, hcalcMean.Item2,
                            Constants.EnvFieldNo.FLD_MEAN,
                            data);
                    }
                    else
                    {
                        var temflds = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TE81_TEMP, data);
                        return Json(new
                        {
                            Success = true,
                            temp_hi = temp_hi,
                            temp_lo = temp_lo,
                            temp_range = temp_range,
                            temp_mean = temp_mean,
                            temp_sigma = temp_sigma,
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
                            humid_range = string.Empty

                        }, JsonRequestBehavior.AllowGet);
                    }
                    var hsigma = _environmentBaseDomain.CalcSigma(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TE81_HUMID, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo,
                        data);

                    if (model.EnvMode == Constants.EnvMode.ControlLine)
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
                        Constants.TypeOfTable.CALC_TE81_HUMID, dt1, rl_upper, Constants.EnvFieldNo.FLD_UPPER,
                        data);
                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TE81_HUMID, dt1, rl_lower, Constants.EnvFieldNo.FLD_LOWER,
                        data);

                    // Update the edit box
                    rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM,
                        Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_HUMID, id,
                        dt1, dt2, t_dis_up, h_dis_up, data);
                    rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM,
                        Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_HUMID, id,
                        dt1, dt2, t_dis_lo, h_dis_lo, data);

                    humid_mean = hcalcMean.Item2.ToString("N2");
                    humid_sigma = hsigma.Item2.ToString("N2");
                    humid_hi = rl_max.ToString("N2");
                    humid_lo = rl_min.ToString("N2");
                    humid_range = (rl_max - rl_min).ToString("N2");
                }
                else
                {
                    humid_mean = string.Empty;
                    humid_sigma = string.Empty;
                    humid_hi = string.Empty;
                    humid_lo = string.Empty;
                    humid_range = string.Empty;
                }
                var temflds1 = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TE81_TEMP, data);
                var hflds = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TE81_HUMID, data);
                return Json(new
                {
                    Success = true,
                    temp_hi = temp_hi,
                    temp_lo = temp_lo,
                    temp_range = temp_range,
                    temp_mean = temp_mean,
                    temp_sigma = temp_sigma,
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