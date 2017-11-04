using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.CalculationOfControlLimit;
using KCSG.Web.Attributes;
using Newtonsoft.Json;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
     [MvcAuthorize("TCEN013F")]
    public class CalculationOfControlLimitController : KCSG.Web.Controllers.BaseController
    {
        #region properties

        private readonly  ICalculationOfControlLimitDomain _calculationOfControlLimitDomain;
        private readonly IEnvironmentBaseDomain _environmentBaseDomain;
        #endregion

        #region Constructor

        public CalculationOfControlLimitController(ICalculationOfControlLimitDomain calculationOfControlLimitDomain, IEnvironmentBaseDomain environmentBaseDomain)
        {
            _calculationOfControlLimitDomain = calculationOfControlLimitDomain;
            _environmentBaseDomain = environmentBaseDomain;
        }
        #endregion
        //
        // GET: /EnvironmentManagement/CalculationOfControlLimit/
        public ActionResult Index()
        {
            var te80EnvMesp = _environmentBaseDomain.GetLocationItemByType("1");

            var model = new CalculationOfControlLimitViewModel
            {
                DurationFrom = te80EnvMesp.Any() ? te80EnvMesp.FirstOrDefault().F80_D_From.Value.ToString("dd/M/yyyy") : DateTime.Now.ToString("dd/M/yyyy"),
                DurationTo = te80EnvMesp.Any() ? te80EnvMesp.FirstOrDefault().F80_D_To.Value.ToString("dd/M/yyyy") : DateTime.Now.ToString("dd/M/yyyy"),
                StartDate = DateTime.Now.AddDays(-89).ToString("dd/M/yyyy"),
                EndDate = DateTime.Now.AddDays(-1).ToString("dd/M/yyyy"),
                Grid = GenerateGrid()
            };
            return View(model);
        }

        public ActionResult GetData(CalculationOfControlLimitViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F80_Type";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _calculationOfControlLimitDomain.SearchCriteria(gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string startDate,string endDate, string type, int id, string name, Constants.EnvMode envMode)
        {
            var dt1 = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt2 = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //if (dt2 > DateTime.Now)
            //{
            //    return Json(new
            //    {
            //        Success = false,
            //        ErrorCode = -2,
            //        Message = "The End Date is invalid."
            //    }, JsonRequestBehavior.AllowGet);
            //}

            //if ((dt2 - dt1).Days >= 90)
            //{
            //    return Json(new
            //    {
            //        Success = false,
            //        ErrorCode = -2,
            //        Message = "The different of dates is more than 90 days!"
            //    }, JsonRequestBehavior.AllowGet);
            //}

            //if (dt2 < dt1)
            //{
            //    return Json(new
            //    {
            //        Success = false,
            //        ErrorCode = -2,
            //        Message = "The Start date is larger than the End date!"
            //    }, JsonRequestBehavior.AllowGet);
            //}
            
            if (ModelState.IsValid)
            {
                var data = new List<Graphtbl>();
                var item = _environmentBaseDomain.GetLocationItemByName(type,id, name);
                var b_DispHumidity = false;
                var dtm1 = dt1.AddHours(8);
                var dtm2 = dt2.AddDays(1).AddHours(8);
                if (item.F80_Humidity == "0")
                {
                    b_DispHumidity = false;
                }
                else
                {
                    b_DispHumidity = true;
                }
                var envMespData = _environmentBaseDomain.GetMespVal(Constants.EnvType.TYPE_RM, item.F80_Id, envMode);

                if (!envMespData.Item1)
                {
                    return Json(new
                    {
                        Success = false,
                        ErrorCode = -2,
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
                if (envMespData.Item1)
                {

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
                    double rl_range;
                    _environmentBaseDomain.SetValueConfigFromTe80(envMespData.Item3, out t_ucl, out t_lcl, out t_usl,
                        out t_lsl, out t_dis_up, out h_dis_up,
                        out t_dis_lo, out h_dis_lo, out h_ucl, out h_lcl, out h_usl, out h_lsl, out t_cal_up,
                        out t_cal_lo, out h_cal_up, out h_cal_lo);
                    var check = true;
                    var dateTime = dt1;
                    do
                    {
                        var calcData = _environmentBaseDomain.CalcData(Constants.EnvType.TYPE_RM, item.F80_Id, null, dateTime, Constants.TypeOfTable.CALC_MGMT_LIMIT, 1,
                        envMode, t_ucl, t_lcl, t_usl, t_lsl, t_dis_up, h_dis_up, t_dis_lo, h_dis_lo, h_ucl, h_lcl, h_usl,
                        h_lsl, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, b_DispHumidity, data);
                        if (!calcData)
                        {
                            check = false;
                            break;
                        }
                        dateTime = dateTime.AddDays(1);
                    } while (dateTime <= dt2);

                    if (check)
                    {
                        var tcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_MGMT_LIMIT,
                            Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP, item.F80_Id, dtm1, dtm2,
                            t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                        if (tcalcMean.Item1)
                        {
                            item.F80_T_Mean = Math.Round(tcalcMean.Item2, 4);
                        }
                        var tsigma = _environmentBaseDomain.CalcSigma(Constants.TypeOfTable.CALC_MGMT_LIMIT,
                            Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP, item.F80_Id, dtm1, dtm2, t_cal_up, t_cal_lo,
                            h_cal_up, h_cal_lo, data);
                        if (tsigma.Item1)
                        {
                            item.F80_T_Sigma = Math.Round(tsigma.Item2, 4);
                        }
                        rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM,
                            Constants.TypeOfTable.CALC_MGMT_LIMIT, Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP,
                            item.F80_Id, dtm1, dtm2, t_dis_up, h_dis_up, data);
                        rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM,
                            Constants.TypeOfTable.CALC_MGMT_LIMIT, Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP,
                            item.F80_Id,
                            dtm1, dtm2, t_dis_lo, h_dis_lo, data);
                        rl_range = rl_max - rl_min;
                        item.F80_T_Range = Math.Round(rl_range, 4);
                        double rl_ucl;
                        double rl_lcl;
                        double rl_cp = 0;
                        double rl_cpk = 0;
                        _environmentBaseDomain.CallLimit(tcalcMean.Item2, tsigma.Item2, out rl_ucl, out rl_lcl);
                        item.F80_T_Ucl = Math.Round(rl_ucl, 4);
                        item.F80_T_Lcl = Math.Round(rl_lcl, 4);
                        _environmentBaseDomain.CalcCp(Constants.TypeOfTable.CALC_TETMP_TEMP90, tcalcMean.Item2,
                            tsigma.Item2, rl_range, t_usl, t_lsl,
                            ref rl_cp, ref rl_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, 0.0, 0.0);
                        item.F80_T_Cp = Math.Round(rl_cp, 4);
                        item.F80_T_Cpk = Math.Round(rl_cpk, 4);

                        if (b_DispHumidity)
                        {
                            var hcalcMean = _environmentBaseDomain.CalcMean(Constants.TypeOfTable.CALC_MGMT_LIMIT,
                                Constants.TypeOfTable.CALC_MGMT_LIMIT_HUMID, item.F80_Id, dtm1, dtm2,
                                t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                            if (hcalcMean.Item1)
                            {
                                item.F80_H_Mean = Math.Round(hcalcMean.Item2, 4);
                            }
                            var hsigma = _environmentBaseDomain.CalcSigma(Constants.TypeOfTable.CALC_MGMT_LIMIT,
                                Constants.TypeOfTable.CALC_MGMT_LIMIT_HUMID, item.F80_Id, dtm1, dtm2, t_cal_up, t_cal_lo,
                                h_cal_up, h_cal_lo, data);
                            if (hsigma.Item1)
                            {
                                item.F80_H_Sigma = Math.Round(hsigma.Item2, 4);
                            }
                            rl_max = _environmentBaseDomain.GetMax(Constants.EnvType.TYPE_RM,
                                Constants.TypeOfTable.CALC_MGMT_LIMIT, Constants.TypeOfTable.CALC_MGMT_LIMIT_HUMID,
                                item.F80_Id, dtm1, dtm2, t_dis_up, h_dis_up, data);
                            rl_min = _environmentBaseDomain.GetMin(Constants.EnvType.TYPE_RM,
                                Constants.TypeOfTable.CALC_MGMT_LIMIT, Constants.TypeOfTable.CALC_MGMT_LIMIT_HUMID,
                                item.F80_Id,
                                dtm1, dtm2, t_dis_lo, h_dis_lo, data);
                            rl_range = rl_max - rl_min;
                            item.F80_H_Range = Math.Round(rl_range, 4);

                            _environmentBaseDomain.CallLimit(hcalcMean.Item2, hsigma.Item2, out rl_ucl, out rl_lcl);
                            item.F80_H_Ucl = Math.Round(rl_ucl, 4);
                            item.F80_H_Lcl = Math.Round(rl_lcl, 4);
                            _environmentBaseDomain.CalcCp(Constants.TypeOfTable.CALC_TETMP_TEMP90, hcalcMean.Item2,
                                hsigma.Item2, rl_range, t_usl, t_lsl,
                                ref rl_cp, ref rl_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, 0.0, 0.0);
                            item.F80_H_Cp = Math.Round(rl_cp, 4);
                            item.F80_H_Cpk = Math.Round(rl_cpk, 4);
                            _environmentBaseDomain.UpdateTe80(item);
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            Success = false,
                            ErrorCode = -1,//Nothing and continue for in grid
                        }, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            return Json(new
            {
                Success = true,
                Message = "Update Success"
            }, JsonRequestBehavior.AllowGet);
        }

        #region private method

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("Grid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("GetData", "CalculationOfControlLimit",
                    new { Area = "EnvironmentManagement" }))
                .SetDefaultSorting("F80_Type", SortOrder.Asc)
                .SetFields(
                    new Field("F80_Type")
                        .SetTitle(" ")
                        .SetWidth(20).SetVisible(false)
                        //.SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("DateFromTo")
                        .SetTitle("Date (From – To)(DD/MM/YYYY)")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(80)
                        .SetSorting(false),
                    new Field("F80_T_Ucl")
                        .SetTitle("UCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(50)
                        .SetSorting(false),
                    new Field("F80_T_Lcl")
                        .SetTitle("LCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(50)
                        .SetSorting(false),
                    new Field("F80_T_Mean")
                        .SetTitle("Mean")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(50)
                        .SetSorting(false),
                    new Field("F80_T_Sigma")
                        .SetTitle("Sigma")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(50),
                    new Field("F80_T_Cp")
                        .SetTitle("Cp")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(50),
                    new Field("F80_T_Cpk")
                        .SetTitle("Cpk")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_T_Range")
                        .SetTitle("Range")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Ucl")
                        .SetTitle("UCL")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Lcl")
                        .SetTitle("LCL")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Mean")
                        .SetTitle("Mean")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Sigma")
                        .SetTitle("Sigma")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                   new Field("F80_H_Cp")
                        .SetTitle("Cp")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                   new Field("F80_H_Cpk")
                        .SetTitle("Cpk")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Range")
                        .SetTitle("Range")
                        .SetWidth(50)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat")

                );
        }
        #endregion
    }
}