using System;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductCertificationManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification;
using KCSG.Web.Attributes;
using Resources;

//using KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln;

namespace KCSG.Web.Areas.ProductManagement.Controllers
{
    [MvcAuthorize("TCPR021F")]
    public class ProductCertificationController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public ProductCertificationController(IProductCertificationDomain prodCerDomain,
            IExportReportDomain exportReportDomain)
        {
            _prodCerDomain = prodCerDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        // GET: ProductManagement/ProductCertification
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCPR021F";
            var model = new ProductCertificationSearchViewModel
            {
                GridNormal = GenerateGridNormal(),
                GridOutOfPlan = GenerateGridOutOfPlan(),
                GridSample = GenerateGridSample()
            };
            return View(model);
        }

        public ActionResult Edit(string prodCode, string prePdtLotNo, string productFlg)
        {
            try
            {
                var model = new ProductCertificationViewModel
                {
                    IsCreate = true
                    //F39_Status = Constants.Status.Yet.ToString(),
                    //F39_KndEptBgnDate = DateTime.Now.ToString("MM/yyyy")
                };
                if (!string.IsNullOrEmpty(prodCode) && !string.IsNullOrEmpty(prePdtLotNo) &&
                    !string.IsNullOrEmpty(productFlg))
                {
                    var entity = _prodCerDomain.GetById(prodCode, prePdtLotNo, productFlg);
                    if (entity != null)
                    {
                        model = Mapper.Map<ProductCertificationViewModel>(entity);
                        //model.F39_Status = Enum.GetName(typeof(Constants.Status), ConvertHelper.ToInteger(model.F39_Status));
                        model.IsCreate = false;
                    }
                }
                return PartialView("_PartialViewEditProductCertification", model);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(ProductCertificationViewModel model)
        {
            try
            {
                var item = Mapper.Map<ProductCertificationItem>(model);
                var isSuccess = _prodCerDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new { Success = false, Message = ProductManagementResources.MSG21}, JsonRequestBehavior.AllowGet);
                return Json(
                    new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                    JsonRequestBehavior.AllowGet);
                ;
            }
            catch (DbUpdateException e)
            {
                var sb = new StringBuilder();
                sb.AppendLine("DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                    sb.AppendLine("Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated");

                Console.WriteLine(sb.ToString());
                throw;
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(string prodCode, string prePdtLotNo, string productFlg)
        {
            try
            {
                //var dateDelete = ConvertHelper.ConvertToDateTimeFull(date);
                var check = _prodCerDomain.Delete(prodCode, prePdtLotNo, productFlg);
                if (check)
                    return Json(new {Success = true, Message = MessageResource.MSG10},
                        JsonRequestBehavior.AllowGet);
                return Json(new {Success = false, Message = MessageResource.MSG51}, JsonRequestBehavior.AllowGet);
                //return Json(new { Success = true, Message = Resources.MessageResource.MSG10 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, MessageResource.MSG51}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByNormal(ProductCertificationSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F56_TbtEndDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _prodCerDomain.SearchNormal(model.YearMonth, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<StorageOfProductItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchBySample(ProductCertificationSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F67_ProductCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _prodCerDomain.SearchSample(model.YearMonth, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<ProductCertificationItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchByOutOfPlan(ProductCertificationSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F58_ProductCode";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _prodCerDomain.SearchOutOfPlan(model.YearMonth, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<ProductCertificationOutOfPlanItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetProductCode(string prodCerCode)
        //{
        //    //var dateDelete = ConvertHelper.ConvertToDateTimeFull(date);
        //    var result = _prodCerDomain.GetById(prodCerCode);
        //    return Json(new { result }, JsonRequestBehavior.AllowGet);
        //}


        /// <summary>
        ///     Tell the service to make the certification ok.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> MakeProductCertificationOk(
            AnalyzeProductCertificationViewModel productCertification)
        {
            try
            {
                // No parameter has been sent to service.
                if (productCertification == null)
                {
                    productCertification = new AnalyzeProductCertificationViewModel();
                    TryValidateModel(productCertification);
                }

                // Parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var result =                    
                        _prodCerDomain.CheckUnique67(productCertification.ProductCode, productCertification.PrePdtLotNo);
                if (result)
                {
                    return Json(new { Success = false, Message = ProductManagementResources.MSG21 }, JsonRequestBehavior.AllowGet);
                }

                await _prodCerDomain.MakeProductCertificationOkAsync(productCertification.status,
                    productCertification.ProductCode, productCertification.PrePdtLotNo,
                    productCertification.ProductLotNo,
                    productCertification.Quantity, productCertification.CertificationDate);

                //return new HttpStatusCodeResult(HttpStatusCode.OK);
                //return Json(new { Success = true, },
                //        JsonRequestBehavior.AllowGet);
                return Json(new { Success = true });
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Tell the service to make the certification ng.
        /// </summary>
        /// <returns></returns>        
        [HttpPost]
        public async Task<ActionResult> MakeProductCertificationNg(
            AnalyzeProductCertificationViewModel productCertification)
        {
            try
            {
                // No parameter has been sent to service.
                if (productCertification == null)
                {
                    productCertification = new AnalyzeProductCertificationViewModel();
                    TryValidateModel(productCertification);
                }

                // Parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var result =
                        _prodCerDomain.CheckUnique67(productCertification.ProductCode, productCertification.PrePdtLotNo);
                if (result)
                {
                    return Json(new { Success = false, Message = ProductManagementResources.MSG21 }, JsonRequestBehavior.AllowGet);
                }

                await _prodCerDomain.MakeProductCertificationNgAsync(productCertification.status,
                    productCertification.ProductCode, productCertification.PrePdtLotNo,
                    productCertification.ProductLotNo,
                    productCertification.Quantity, productCertification.CertificationDate);

                //return new HttpStatusCodeResult(HttpStatusCode.OK);
                return Json(new { Success = true, Message = "Successfully" },
                        JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Print the list of input commands.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>        
        public ActionResult Export()
        {
            try
            {
                var model = new PrintProductCertificationViewModel
                {
                    StartDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    EndDate = DateTime.Now.ToString("dd/MM/yyyy")
                    //IsCreate = true
                    //F39_Status = Constants.Status.Yet.ToString(),
                    //F39_KndEptBgnDate = DateTime.Now.ToString("MM/yyyy")
                };
                //if (!string.IsNullOrEmpty(prodCode) && !string.IsNullOrEmpty(prePdtLotNo) &&
                //    !string.IsNullOrEmpty(productFlg))
                //{
                //    var entity = _prodCerDomain.GetById(prodCode, prePdtLotNo, productFlg);
                //    if (entity != null)
                //    {
                //        model = Mapper.Map<ProductCertificationViewModel>(entity);
                //        //model.F39_Status = Enum.GetName(typeof(Constants.Status), ConvertHelper.ToInteger(model.F39_Status));
                //        model.IsCreate = false;
                //    }
                //}
                return PartialView("_PartialPrintProductCertification", model);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> ExportDetail(PrintProductCertificationViewModel item)
        {
            try
            {
                // Request parameters haven't been initialized.
                if (item == null)
                {
                    item = new PrintProductCertificationViewModel();
                    TryValidateModel(item);
                }

                // Request parameters are invalid.
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                string file = null;

                switch (item.PrintProductCertificationStatus)
                {
                    case Constants.PrintProductCertificationStatus.Certificated:
                        file = Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.StockCertified));
                        break;
                    case Constants.PrintProductCertificationStatus.OutOfSpec:
                        file = Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.StockListOutOfSpec));
                        break;
                    default:
                        file = Server.MapPath(string.Format("~/{0}", Constants.ExportTemplate.StockListUncertificated));
                        break;
                }

                var data = new
                {
                    items = await _prodCerDomain.FindProductCertificationsForPrinting(item.PrintProductCertificationStatus, ConvertHelper.ConvertToDateTimeFull(item.StartDate), ConvertHelper.ConvertToDateTimeFull(item.EndDate)),                    
                  //  page = item.Settings.PageIndex.ToString("###"),
                    company = WebConfigurationManager.AppSettings["CompanyName"], // TODO: Move to web.config
                    currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm")
                };
                    
                if (data.items != null)
                {
                    var template = await _exportReportDomain.ReadTemplateFileAsync(file);

                    // Export the template.
                    var render = await _exportReportDomain.ExportToFlatFileAsync(data, template);

                    return Json(new
                    {
                        render
                    });
                    
                }
                else
                {
                    return Json(new { Success = false, Message = ProductManagementResources.MSG1 });
                }

                // Read the template.
                
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string f67_ProductCode, string f67_ProductLotNo, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            //var kndData = ConvertHelper.ConvertToDateTimeFull("01/" + f39_KndEptBgnDate);
            var result = !_prodCerDomain.CheckUnique(f67_ProductCode, f67_ProductLotNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Domain Declaration

        private readonly IProductCertificationDomain _prodCerDomain;

        /// <summary>
        ///     Domain which handle report export.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        //private readonly IPreProductDomain _preProductDomain;

        #endregion

        #region Private Methods

        private Grid GenerateGridNormal()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("NormalGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByNormal", "ProductCertification",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F56_TbtEndDate", SortOrder.Asc)
                .SetFields(
                    new Field("F56_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F56_TbtEndDate")
                        .SetWidth(80)
                        .SetTitle("Tbt End Date")
                        .SetItemTemplate("gridHelper.displayDateFormat")
                        .SetSorting(false),
                    new Field("F56_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("OutofSpecFlag")
                        .SetTitle("OutofSpec FLag")
                        .SetWidth(150),
                    new Field("F56_PrePdtLotNo")
                        .SetTitle("Prepdt Lotno")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F56_ProductLotNo")
                        .SetTitle("Product Lotno")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F56_TbtCmdEndAmt")
                        .SetTitle("Quantity")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                );
        }

        private Grid GenerateGridSample()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SampleGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchBySample", "ProductCertification",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F67_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F67_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F67_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        .SetSorting(false),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F67_ProductLotNo")
                        .SetTitle("Product Lotno")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F67_Amount")
                        .SetTitle("Quantity")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(150),
                    new Field("F67_CertificationDate")
                        .SetTitle("Certification Date")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayDate")
                );
        }

        private Grid GenerateGridOutOfPlan()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("OutOfPlanGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByOutOfPlan", "ProductCertification",
                    new {Area = "ProductManagement"}))
                .SetDefaultSorting("F58_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F58_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F58_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100)
                        //.SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("OutofSpecFlag")
                        .SetTitle("OutofSpec Flag")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F58_PrePdtLotNo")
                        .SetTitle("Prepdt Lotno")
                        .SetWidth(150),
                    new Field("F58_ProductLotNo")
                        .SetTitle("Lot No.")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F58_TbtCmdEndAmt")
                        .SetTitle("Quantity")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                );
        }

        #endregion
    }
}