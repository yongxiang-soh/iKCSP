using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN041F")]
    public class XRTemperatureManagementController : KCSG.Web.Controllers.BaseController
    {
        private IXRTemperatureManagement _xrTemperatureManagement;

        public XRTemperatureManagementController(IXRTemperatureManagement temperatureManagement)
        {
            _xrTemperatureManagement = temperatureManagement;
        }

        // GET: EnvironmentManagement/XRTemperatureManagement
        public ActionResult Index()
        {
            var model = new XRTemperatureManagementModel()
            {
                SearchCriteriaModel = new SearchCriteriaModel()
                {
                    StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    EndDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    Location = "Product"
                },
                TempModel = new ChartModel()
                {
                    ChartName = "Average",
                },
                HimIdModel = new ChartModel()
                {
                    ChartName = "Humidity"
                }
            };
            ViewBag.ListLocation = _xrTemperatureManagement.GetProduct();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchCriteriaModel model)
        {
            var result =
                _xrTemperatureManagement.Search(model.Location, model.Mode,
                    ConvertHelper.ConvertToDateTimeFull(model.StartDate),
                    ConvertHelper.ConvertToDateTimeFull(model.EndDate)).Data;
            if (result != null)
            {
                var resultModel = new XRTemperatureManagementModel();
                resultModel.TempModel = new ChartModel()
                {
                    LstData = result.tblTemp,
                    LstTime = result.TimeTemp,
                    High = result.HighTemp,
                    Range = result.RangeTemp,
                    Cp = result.CpTemp,
                    Cpk = result.CpkTemp,
                    LCL = result.LCLTemp,
                    Low = result.LowTemp,
                    Mean = result.MeanTemp,
                    Sigma = result.SigmaTemp,
                    UCL = result.UCLTemp
                };
                resultModel.HimIdModel = new ChartModel()
                {
                    LstData = result.tblHumid,
                    LstTime = result.TimeHumid,
                    High = result.HighHumid,
                    Range = result.RangeHumid,
                    Cp = result.CpHumid,
                    Cpk = result.CpkHumid,
                    LCL = result.LCLHumid,
                    Low = result.LowHumid,
                    Mean = result.MeanHumid,
                    Sigma = result.SigmaHumid,
                    UCL = result.UCLHumid
                };


                return Json(resultModel, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}