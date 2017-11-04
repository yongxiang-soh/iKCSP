using System;
using System.Web.Mvc;
using System.Web.UI;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Attributes;
using Newtonsoft.Json.Linq;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN042F")]
    public class LotDataSamplingController : KCSG.Web.Controllers.BaseController
    {
        public ILotDataSamplingDomain _LotDataSamplingDomain;

        public LotDataSamplingController(ILotDataSamplingDomain dataSamplingDomain)
        {
            _LotDataSamplingDomain = dataSamplingDomain;
        }

        // GET: EnvironmentManagement/LotDataSampling
        public ActionResult Index()
        {
            ViewBag.ListLotNo = _LotDataSamplingDomain.GetLotNo("", true);
            ViewBag.ListProduct = _LotDataSamplingDomain.GetProduct();

            var model = new LotDataSamplingModel()
            {
                Grid = GenerateGrid(),
                Date = DateTime.Now.ToString("dd/MM/yyyy")
            };
            return View(model);
        }

        [HttpOptions]
        public ActionResult GetLotNo(string productCode, bool newCheck)
        {
            var result = _LotDataSamplingDomain.GetLotNo(productCode, newCheck);
            return Json(result);
        }

        [HttpOptions]
        public ActionResult ValidateTime(string time, string product, string date)
        {
            var result = _LotDataSamplingDomain.GetValueWithTime(time, product, date);
            if (result.IsSuccess)
            {
                return Json(new
                {
                    result.IsSuccess,
                    value = result.ErrorMessages[0]
                }
                    , JsonRequestBehavior.AllowGet);
            }
            return Json(result.IsSuccess, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(LotDataSamplingModel model)
        {
            var result = _LotDataSamplingDomain.Search(model.Product, ConvertHelper.ConvertToDateTimeFull(model.Date),
                model.Mode, model.LotNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Add(LotDataSamplingModel model)
        {
            var result = _LotDataSamplingDomain.Add(model.Product, model.LotNo,
                ConvertHelper.ConvertToDateTimeFull(model.Date), model.Time,model.Temperature);

            return
                Json(
                    new
                    {
                        Success = result.IsSuccess,
                        Message =
                            result.IsSuccess ? Resources.MessageResource.MSG6 : result.ErrorMessages[0]
                    },
                    JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDate(string lotNo)
        {
            var result = _LotDataSamplingDomain.GetDate(lotNo);
            return Json(result);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = _LotDataSamplingDomain.Delete(id);
            return Json(new {Success = result, Message = Resources.MessageResource.MSG10}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetData(string productCode, GridSettings gridSettings)
        {
            var result = _LotDataSamplingDomain.GetTable(productCode, gridSettings);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }


        [HttpOptions]
        public ActionResult GetTe84EnvLot(string productCode,string lotNo,string date)
        {
            var result = _LotDataSamplingDomain.GetTe84EnvLot(productCode, lotNo, date);
            return Json(result);
        }
        #region private method

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("Grid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto").OnDataLoaded("LoadGridSuccess")
                .SetAutoload(false).SetPageLoading(true).SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetSearchUrl(urlHelper.Action("GetData", "LotDataSampling",
                    new {Area = "EnvironmentManagement"}))
                .SetDefaultSorting("F84_Id", SortOrder.Asc)
                .SetFields(
                    new Field("F84_Id")
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetWidth(20)
                        .SetSorting(false),
                    new Field("F84_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(130)
                        .SetSorting(false),
                    new Field("F84_S_Time")
                        .SetTitle("Time")
                        .SetItemTemplate("gridHelper.displayDateTimeOnly")
                        .SetWidth(130)
                        .SetSorting(false),
                    new Field("F84_ProductLotNo")
                        .SetTitle("Product LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F84_Temp")
                        .SetTitle("Temperature")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetWidth(100)
                        .SetSorting(false)
                );
        }

        #endregion
    }
}