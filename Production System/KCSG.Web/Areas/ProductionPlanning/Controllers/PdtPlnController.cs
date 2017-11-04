using System;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP041F")]
    public class PdtPlnController : KCSG.Web.Controllers.BaseController
    {
        #region Controller Constructor

        public PdtPlnController(IPdtPlnDomain pdtPlnDomain, IPreProductDomain preProductDomain,ICommonDomain iDomain)
        {
            _pdtPlnDomain = pdtPlnDomain;
            _preProductDomain = preProductDomain;
            _commonDomain = iDomain;
        }

        #endregion

        //
        // GET: /Master/PdtPln/

        public ActionResult Index()
        {
            var model = new PdtPlnSearchViewModel
            {
                Grid = GenerateGrid(),
                YearMonth = DateTime.Now.ToString("MM/yyyy")
            };
            return View(model);
        }


        public ActionResult Edit(string date, string preProdCode,int line)
        {
            var model = new PdtPlnViewModel
            {
                KndLine = line,
                IsCreate = true,
                F39_Status = Constants.Status.Yet.ToString(),
                F39_KndEptBgnDate = DateTime.Now.ToString("dd/MM/yyyy")
            };
            if (!string.IsNullOrEmpty(preProdCode))
            {
                var entity = _pdtPlnDomain.GetById(ConvertHelper.ConvertToDateTimeFull(date), preProdCode);
                if (entity != null)
                {
                    model = Mapper.Map<PdtPlnViewModel>(entity);
                    model.KndLine = line;
                    model.F39_Status = Enum.GetName(typeof(Constants.Status), ConvertHelper.ToInteger(model.F39_Status));
                    model.IsCreate = false;
                }
            }
            return PartialView("PdtPln/_PartialViewEditPdtPln", model);
        }

        [HttpPost]
        public ActionResult Edit(PdtPlnViewModel model)
        {
            try
            {
                var item = Mapper.Map<PdtPlnItem>(model);
                var isSuccess = _pdtPlnDomain.CreateOrUpdate(item);
                if (!isSuccess.IsSuccess)
                    return Json(new {Success = false, Message = isSuccess.ErrorMessages}, JsonRequestBehavior.AllowGet);
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
        public ActionResult Delete(string date, string code)
        {
            try
            {
                var dateDelete = ConvertHelper.ConvertToDateTimeFull(date);
                var check = _pdtPlnDomain.Delete(dateDelete, code);
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

        //[HttpPost]
        //[ValidateInput(false)]

        //public ActionResult SearchByDate(PdtPlnSearchViewModel model, GridSettings gridSettings)
        //{
        //    var result = _pdtPlnDomain.SearchCriteria(model.F39_KndEptBgnDate, gridSettings);
        //    if (!result.IsSuccess)
        //    {
        //        return Json(null, JsonRequestBehavior.AllowGet);
        //    }

        //    var response = Mapper.Map<GridResponse<TX39_PdtPln>>(result.Data);
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByDate(PdtPlnSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F39_KndEptBgnDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _pdtPlnDomain.SearchCriteria(model.YearMonth, model.KndLine, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);

            var response = Mapper.Map<GridResponse<PdtPlnItem>>(result.Data);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetProductCode(string preProductCode)
        {
            var result = _preProductDomain.GetById(preProductCode);
            return Json(new {result}, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("PdtPlnGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByDate", "PdtPln", new { KndLine = Constants.KndLine.Conventional, Area = "ProductionPlanning" }))
                .SetDefaultSorting("F39_KndEptBgnDate", SortOrder.Asc)
                .SetFields(
                    new Field("F39_KndEptBgnDate")
                        .SetWidth(20)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F39_KndEptBgnDate")
                        .SetTitle("Production Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(100),
                    new Field("F39_PreProductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("PreProductName")
                        .SetTitle("Pre-Product Name")
                        .SetWidth(150),
                    new Field("F39_PrePdtLotAmt")
                        .SetTitle("Lot Qty")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("Color")
                        .SetTitle("Color")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("StatusYet")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F39_KndCmdNo")
                        .SetTitle("Command No")
                        .SetWidth(100)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckExistCode(string f39_preproductcode, string f39_KndEptBgnDate, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var kndData = ConvertHelper.ConvertToDateTimeFull(f39_KndEptBgnDate);
            var result = !_pdtPlnDomain.CheckUnique(f39_preproductcode, kndData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpOptions]
        public JsonResult CheckHoliday(string f39_KndEptBgnDate,string kndLine, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var checkHoliday =
                !_commonDomain.CheckHoliday(ConvertHelper.ConvertToDateTimeFull( f39_KndEptBgnDate));
            var checkKndLine =
                !_pdtPlnDomain.CheckUnique3Reco(ConvertHelper.ConvertToDateTimeFull( f39_KndEptBgnDate), kndLine);
            if (!checkHoliday)
            {
                return Json(false,JsonRequestBehavior.AllowGet);
            }
            if (!checkKndLine)
            {
                return Json(MessageResource.MSG30, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #region Domain Declaration

        private readonly IPdtPlnDomain _pdtPlnDomain;
        private readonly IPreProductDomain _preProductDomain;
        private readonly ICommonDomain _commonDomain;

        #endregion
    }
}