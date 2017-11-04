using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Web.Attributes;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.CreepingAndRollSpeedDataOneDay;
using Newtonsoft.Json;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN023F")]
    public class CreepingAndRollSpeedDataOneDayController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;

        #endregion

        public CreepingAndRollSpeedDataOneDayController(IEnvironmentBaseDomain environmentBaseDomain)
        {
            this._environmentBaseDomain = environmentBaseDomain;
        }

        //
        // GET: /EnvironmentManagement/CreepingAndRollSpeedDataOneDay/
        public ActionResult Index(string environmentDate = "", Constants.EnvMode envMode = Constants.EnvMode.ControlLine,string machine="")
        {
            DateTime dt1;
            var ListName = new List<SelectListItem>();
            ListName.Add(new SelectListItem
            {
                Text = "12 Inch Mixing Roll Machine",
                Selected = machine.Equals("Roll_MC_12"),
                Value = "Roll_MC_12"
            });
            ListName.Add(new SelectListItem
            {
                Text = "18 Inch Mixing Roll Machine",
                Selected = machine.Equals("Roll_MC_18"),
                Value = "Roll_MC_18"
            } );
            ViewBag.ListName = ListName;
            if (DateTime.TryParseExact(environmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt1))
            {
                var model = new CreepingAndRollSpeedDataOneDayViewModel()
                {
                    EnvironmentDate = dt1.ToString("dd/MM/yyyy"),
                    EnvMode = envMode
                };
                return View(model);
            }
            else
            {
                var model = new CreepingAndRollSpeedDataOneDayViewModel()
                {
                    EnvironmentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    EnvMode = envMode
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Search(CreepingAndRollSpeedDataOneDayViewModel model)
        {
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
            
            string left_high = string.Empty;
            string left_low = string.Empty;
            string left_range = string.Empty;
            string left_mean = string.Empty;
            string left_sigma = string.Empty;
            string left_yAxis = string.Empty;
            string left_fld_data = string.Empty;
            string left_fld_mean = string.Empty;
            string left_fld_upper = string.Empty;
            string left_fld_lower = string.Empty;

            string right_high = string.Empty;
            string right_low = string.Empty;
            string right_range = string.Empty;
            string right_mean = string.Empty;
            string right_sigma = string.Empty;
            string right_yAxis = string.Empty;
            string right_fld_data = string.Empty;
            string right_fld_mean = string.Empty;
            string right_fld_upper = string.Empty;
            string right_fld_lower = string.Empty;

            string spd_high = string.Empty;
            string spd_low = string.Empty;
            string spd_range = string.Empty;
            string spd_mean = string.Empty;
            string spd_sigma = string.Empty;
            string spd_yAxis = string.Empty;
            string spd_fld_data = string.Empty;
            string spd_fld_mean = string.Empty;
            string spd_fld_upper = string.Empty;
            string spd_fld_lower = string.Empty;

            if (ModelState.IsValid)
            {
                for (int i = 1; i <= 3; i++)
                {
                    var id = 6;
                    if (model.Machine.Equals("Roll_MC_18") && (i != 3))
                    {
                        id = i;
                    }
                    else if (model.Machine.Equals("Roll_MC_12") && (i != 3))
                    {
                        id = i + 2;
                    }
                    else if (model.Machine.Equals("Roll_MC_18") && (i == 3))
                    {
                        id = 5;
                    }

                    var envMespData = _environmentBaseDomain.GetMespVal(Constants.EnvType.TYPE_RS, id,
                        model.EnvMode);
                    if (!envMespData.Item1)
                    {
                        return Json(new
                        {
                            Success = false,
                            ErrorCode = -1, //Reset graph and all fields in the form
                            Message = envMespData.Item2
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var data = new List<Graphtbl>();
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
                    
                    _environmentBaseDomain.SetValueConfigFromTe80(envMespData.Item3, out t_ucl, out t_lcl, out t_usl,
                        out t_lsl, out t_dis_up, out h_dis_up,
                        out t_dis_lo, out h_dis_lo, out h_ucl, out h_lcl, out h_usl, out h_lsl, out t_cal_up,
                        out t_cal_lo, out h_cal_up, out h_cal_lo);
                    var totalRecord = 0;
                    var calcData = _environmentBaseDomain.CalcData(Constants.EnvType.TYPE_RS, id, null, dt1,
                        Constants.TypeOfTable.CALC_TE83, 1,
                        model.EnvMode, t_ucl, t_lcl, t_usl, t_lsl, t_dis_up, h_dis_up, t_dis_lo, h_dis_lo, h_ucl, h_lcl,
                        h_usl,
                        h_lsl, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, true, data);
                    if (!calcData)
                    {
                        return Json(new
                        {
                            Success = false,
                            ErrorCode = 0, //Nothing to do
                        }, JsonRequestBehavior.AllowGet);
                    }
                    var tcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TE83, id, dt1, dt2,
                        t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);

                    if (tcalcMean.Item1)
                    {
                        _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                            Constants.TypeOfTable.CALC_TE83, dt1, tcalcMean.Item2, Constants.EnvFieldNo.FLD_MEAN,
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
                        Constants.TypeOfTable.CALC_TE83, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                    rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TE83, id, dt1, dt2, t_dis_up, h_dis_up, data);
                    rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_BUFFER,
                        Constants.TypeOfTable.CALC_TE83, id, dt1, dt2, t_dis_lo, h_dis_lo, data);

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

                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TE83, dt1, rl_upper, Constants.EnvFieldNo.FLD_UPPER, data);
                    _environmentBaseDomain.UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB,
                        Constants.TypeOfTable.CALC_TE83, dt1, rl_lower, Constants.EnvFieldNo.FLD_LOWER, data);
                    var flds = _environmentBaseDomain.GrapData(Constants.TypeOfTable.CALC_TE83, data);
                    switch (id)
                    {
                        case 1:
                        case 3:
                            left_high = rl_max.ToString("F");
                            left_low = rl_min.ToString("F");
                            left_range = (rl_max - rl_min).ToString("F");
                            left_mean = tcalcMean.Item2.ToString("F");
                            left_sigma = tsigma.Item2.ToString("F");
                            left_yAxis = JsonConvert.SerializeObject(flds.Item5);
                            left_fld_data = JsonConvert.SerializeObject(flds.Item1);
                            left_fld_mean = JsonConvert.SerializeObject(flds.Item2);
                            left_fld_upper = JsonConvert.SerializeObject(flds.Item3);
                            left_fld_lower = JsonConvert.SerializeObject(flds.Item4);

                            break;
                        case 2:
                        case 4:
                            right_high = rl_max.ToString("F");
                            right_low = rl_min.ToString("F");
                            right_range = (rl_max - rl_min).ToString("F");
                            right_mean = tcalcMean.Item2.ToString("F");
                            right_sigma = tsigma.Item2.ToString("F");
                            right_yAxis = JsonConvert.SerializeObject(flds.Item5);
                            right_fld_data = JsonConvert.SerializeObject(flds.Item1);
                            right_fld_mean = JsonConvert.SerializeObject(flds.Item2);
                            right_fld_upper = JsonConvert.SerializeObject(flds.Item3);
                            right_fld_lower = JsonConvert.SerializeObject(flds.Item4);
                            break;
                        case 5:
                        case 6:
                            spd_high = rl_max.ToString("F");
                            spd_low = rl_min.ToString("F");
                            spd_range = (rl_max - rl_min).ToString("F");
                            spd_mean = tcalcMean.Item2.ToString("F");
                            spd_sigma = tsigma.Item2.ToString("F");
                            spd_yAxis = JsonConvert.SerializeObject(flds.Item5);
                            spd_fld_data = JsonConvert.SerializeObject(flds.Item1);
                            spd_fld_mean = JsonConvert.SerializeObject(flds.Item2);
                            spd_fld_upper = JsonConvert.SerializeObject(flds.Item3);
                            spd_fld_lower = JsonConvert.SerializeObject(flds.Item4);
                            break;
                    }
                }
            }
            return Json(new
                {
                    Success = true,
                    left_high = left_high,
                    left_low = left_low,
                    left_range = left_range,
                    left_mean = left_mean,
                    left_sigma = left_sigma,
                    left_yAxis = left_yAxis,
                    left_fld_data = left_fld_data,
                    left_fld_mean = left_fld_mean,
                    left_fld_lower = left_fld_lower,
                    left_fld_upper = left_fld_upper,

                    right_high = right_high,
                    right_low = right_low,
                    right_range = right_range,
                    right_mean = right_mean,
                    right_sigma = right_sigma,
                    right_yAxis = right_yAxis,
                    right_fld_data = right_fld_data,
                    right_fld_mean = right_fld_mean,
                    right_fld_lower = right_fld_lower,
                    right_fld_upper = right_fld_upper,

                    spd_high = spd_high,
                    spd_low = spd_low,
                    spd_range = spd_range,
                    spd_mean = spd_mean,
                    spd_sigma = spd_sigma,
                    spd_yAxis = spd_yAxis,
                    spd_fld_data = spd_fld_data,
                    spd_fld_mean = spd_fld_mean,
                    spd_fld_lower = spd_fld_lower,
                    spd_fld_upper = spd_fld_upper,
            }, JsonRequestBehavior.AllowGet);

        }
    }
}