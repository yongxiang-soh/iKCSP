using System;
using System.Globalization;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM011F")]
    public class MaterialReceptionInputController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly IMaterialReceptionDomain _materialReceptionDomain;

        #endregion

        #region Constructors

        public MaterialReceptionInputController(IMaterialReceptionDomain materialReceptionDomain)
        {
            _materialReceptionDomain = materialReceptionDomain;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCRM011F;
            var model = new MaterialReceptionSearchViewModel
            {
                GridMaterialReception = GenerateGridMaterial()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByName(MaterialReceptionSearchViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F30_ExpectDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _materialReceptionDomain.SearchCriteria(model.PrcOrdNo, model.PartialDelivery, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistPrcOrdNoCode(string F30_PrcOrdNo,string F30_PrtDvrNo, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var result = !_materialReceptionDomain.CheckUnique(F30_PrcOrdNo, F30_PrtDvrNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckExistPrtDvrNoCode(string F30_PrtDvrNo, bool isCreate)
        {
            if (!isCreate) return Json(true, JsonRequestBehavior.AllowGet);
            var result = !_materialReceptionDomain.CheckPrtDvrNo(F30_PrtDvrNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        private Grid GenerateGridMaterial()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("MaterialReceptionGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("auto")
                .SetSorting(true)
                .SetSelected(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "MaterialReceptionInput",
                    new {Area = "MaterialManagement"}))
                .SetDefaultSorting("F30_ExpectDate", SortOrder.Asc)
                .SetFields(
                    new Field("F30_PrcOrdNo")
                        .SetTitle(" ")
                        .SetWidth(30)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F30_ExpectDate")
                        .SetTitle("Delivery Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetWidth(130),
                    new Field("F30_PrcOrdNo")
                        .SetTitle("P.O.No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F30_PrtDvrNo")
                        .SetTitle("P.D.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F30_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("Name")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F30_ExpectAmount")
                        .SetTitle("Delivery Qty")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("F30_StoragedAmount")
                        .SetTitle("Delivered Qty")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("AcceptClass")
                        .SetTitle("Accepted Class")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        public ActionResult Edit(string prcOrdNo, string prtDvrNo)
        {
            var model = new MaterialReceptionViewModel {IsCreate = true};
            model.AcceptClass = Constants.TX30_Reception.Pending.ToString("G");
            model.F30_ExpectDate = DateTime.Now.ToString("dd/MM/yyyy");
            if (!string.IsNullOrEmpty(prcOrdNo) && !string.IsNullOrEmpty(prtDvrNo))
            {
                var entity = _materialReceptionDomain.GetByMaterialReception(prcOrdNo, prtDvrNo);
                if (entity != null)
                {
                    model = Mapper.Map<MaterialReceptionViewModel>(entity);

                    var expectDate = Convert.ToDateTime(model.F30_ExpectDate).ToString("dd/MM/yyyy");
                    model.F30_ExpectDate = expectDate;

                    model.IsCreate = false;
                }
            }
            return PartialView("MaterialReceptionInput/_PartialViewEditMaterialReception", model);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult Edit(MaterialReceptionViewModel model)
        {
            if (ModelState.IsValid)
                try
                {
                    //if (model.AcceptClass.Equals(Constants.TX30_Reception.Accepted.ToString("G")))
                    //    model.F30_AcceptClass = Constants.TX30_Reception.Accepted.ToString("D");
                    //else if (model.AcceptClass.Equals(Constants.TX30_Reception.NonAccept.ToString("G")))
                    //    model.F30_AcceptClass = Constants.TX30_Reception.NonAccept.ToString("D");
                    //else if (model.AcceptClass.Equals(Constants.TX30_Reception.Rejected.ToString("G")))
                    //    model.F30_AcceptClass = Constants.TX30_Reception.Rejected.ToString("D");
                    //else
                    //    model.F30_AcceptClass = Constants.TX30_Reception.Pending.ToString("D");

                    var expectDate = DateTime.ParseExact(model.F30_ExpectDate, "dd/MM/yyyy", null)
                        .ToString("MM/dd/yyyy");
                    model.F30_ExpectDate = expectDate;
                    var item = Mapper.Map<MaterialReceptionItem>(model);
                    var isSuccess = _materialReceptionDomain.CreateOrUpdate(item);
                    if (!isSuccess.IsSuccess)
                        return Json(new {Success = false, Message = isSuccess.ErrorMessages},
                            JsonRequestBehavior.AllowGet);
                    return
                        Json(
                            new {Success = true, Message = model.IsCreate ? MessageResource.MSG6 : MessageResource.MSG9},
                            JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
                }
            return Json(new {Success = false}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string prcOrdNo, string prtDvrNo, string materialCode)
        {
            try
            {
                var result = _materialReceptionDomain.Delete(prcOrdNo, prtDvrNo, materialCode);
                if (result)
                    return Json(new {Success = true, Message = MessageResource.MSG10},
                        JsonRequestBehavior.AllowGet);
                return Json(new {Success = false, Message = MaterialResource.MSG4},
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}