using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Communication.ViewModels;
using KCSG.Web.Attributes;
using KCSG.Web.Controllers;

namespace KCSG.Web.Areas.Communication.Controllers
{
    [MvcAuthorize("AWT004")]
    public class WeighingEquipmentController : BaseController
    {
        private readonly IWeighingEquipmentDomain _weighingEquipmentDomain;

        public WeighingEquipmentController(IWeighingEquipmentDomain weighingEquipmentDomain)
        {
            _weighingEquipmentDomain = weighingEquipmentDomain;
        }
        // GET: Communication/WeighingEquipment
        public ActionResult Index()
        {
            var model = new WeighingEquipmentViewModel()
            {
                PreproductGrid = GeneratePreproductGrid(),
                MaterialGrid = GenerateMaterialGrid(),
                RetrievalGrid = GenerateRetrievalGrid(),
                KneadingCommandGrid = GenerateKndCommandGrid(),
                KneadingResultsGrid = GenerateKndResultGrid()
            };
            return View(model);
        }

        public ActionResult SearchByMaterial(WeighingEquipmentViewModel model, GridSettings gridSettings)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.SearchByMaterial(searchModel, gridSettings);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchByPreproduct(WeighingEquipmentViewModel model, GridSettings gridSettings)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.SearchByPreproduct(searchModel, gridSettings);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchByRetrieval(WeighingEquipmentViewModel model, GridSettings gridSettings)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.SearchByRetrieval(searchModel, gridSettings);
            var jsonResult = Json(result.Data, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        public ActionResult SearchByKndCommand(WeighingEquipmentViewModel model, GridSettings gridSettings)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.SearchByKndCommand(searchModel, gridSettings);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchByKndResult(WeighingEquipmentViewModel model, GridSettings gridSettings)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.SearchByKndResult(searchModel, gridSettings);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendMaterialMaster ()
        {
            var deviceCode = ConfigurationManager.AppSettings["C4DeviceCode"];
            var terminalNo = ConfigurationManager.AppSettings["C4_Term"];
            var result = _weighingEquipmentDomain.SendMaterialMaster( terminalNo, deviceCode);

            return Json(new { isSucces = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendPreproductMaster()
        {
            var deviceCode = ConfigurationManager.AppSettings["C4DeviceCode"];
            var terminalNo = ConfigurationManager.AppSettings["C4_Term"];
            var result = _weighingEquipmentDomain.SendPreproductMaster(terminalNo,deviceCode);
            return Json(new { isSucces = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDataOnQueue(WeighingEquipmentViewModel model)
        {
            var searchModel = Mapper.Map<WeighingEquipmentViewSearchModel>(model);
            var result = _weighingEquipmentDomain.DeleteDataOnQueue(searchModel);
            return Json(new { isSucces = result, message = Resources.MessageResource.MSG10 }, JsonRequestBehavior.AllowGet);
        }
        #region private Method
        public Grid GenerateMaterialGrid()
        {
            return new Grid("Material")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByMaterial"))
                 .SetDefaultSorting("Date", SortOrder.Asc)
                .SetFields(
                 new Field("Date")
                        .SetWidth(100)
                        .SetTitle("Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetSorting(false),
                    new Field("Terminal")
                        .SetWidth(100)
                        .SetTitle("Terminal")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Class")
                        .SetTitle("M/c Class")
                        .SetItemTemplate("gridHelper.displayNumber")
                        .SetWidth(100),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("MasterCode")
                        .SetTitle("Master Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("ErrorCode")
                        .SetTitle("Error Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                   
                );
        } 
        public Grid GeneratePreproductGrid()
        {
            return new Grid("Preproduct")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByPreproduct"))
                 .SetDefaultSorting("Date", SortOrder.Asc)
                .SetFields(
                 new Field("Date")
                        .SetWidth(100)
                        .SetTitle("Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetSorting(false),
                    new Field("Terminal")
                        .SetWidth(100)
                        .SetTitle("Terminal")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Class")
                        .SetTitle("M/c Class")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("MasterCode")
                        .SetTitle("Master Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("ErrorCode")
                        .SetTitle("Error Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                   
                );
        } 
        public Grid GenerateRetrievalGrid()
        {
            return new Grid("Retrieval")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByRetrieval"))
                 .SetDefaultSorting("Date", SortOrder.Asc)
                .SetFields(
                 new Field("Date")
                        .SetWidth(100)
                        .SetTitle("Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetSorting(false),
                    new Field("MasterCode")
                        .SetWidth(100)
                        .SetTitle("Master Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("Pallet No")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("Class")
                        .SetTitle("Machine Class")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("SendFlag")
                        .SetTitle("Send Flag")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("AbnormalCode")
                        .SetTitle("Abnormal Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                   
                );
        }

        public Grid GenerateKndCommandGrid()
        {
            return new Grid("KndCommand")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByKndCommand"))
                .SetDefaultSorting("Date",SortOrder.Asc)
                .SetFields(
                    new Field("Date")
                        .SetWidth(100)
                        .SetTitle("Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetSorting(false),
                    new Field("CommandNo")
                        .SetWidth(100)
                        .SetTitle("Command No. ")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("PreproductCode")
                        .SetTitle("Pre-product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("PreproductLotNo")
                        .SetTitle("Pre-product Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("Class")
                        .SetTitle("M/c Class")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("ErrorCode")
                        .SetTitle("Error Code")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")

                );
        }
        public Grid GenerateKndResultGrid()
        {
            return new Grid("KndResult")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(Url.Action("SearchByKndResult"))
                 .SetDefaultSorting("Date", SortOrder.Asc)
                .SetFields(
                    new Field("Date")
                        .SetWidth(100)
                        .SetTitle("Date")
                        .SetItemTemplate("gridHelper.displayDate")
                        .SetSorting(false),
                    new Field("Command")
                        .SetWidth(100)
                        .SetTitle("Command")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("PreproducttLotNo")
                        .SetTitle("Ppdt Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),

                    new Field("MaterialLotNo")
                        .SetTitle("Material Lot No.")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),

                    new Field("KneadingLine")
                        .SetTitle("Kneading Line")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("StartDate")
                        .SetTitle("B/Start Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate"),
                    new Field("EndDate")
                        .SetTitle("B/End Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate"),

                    new Field("Sequence")
                        .SetTitle("Sequence")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("ChargedQty")
                            .SetTitle("Charged Qty")
                            .SetWidth(100)
                            .SetSorting(false)
                            .SetItemTemplate("gridHelper.formatNumber")


                );
        } 
        #endregion
    }
}