using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using  KCSG.Core.Helper;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.Controllers
{
    [MvcAuthorize("TCPP051F")]
    public class MaterialSimulationController : BaseController
    {
        private IMaterialSimulationDomain _materialSimulationDomain;
        private IPreProductDomain _preProductDomain;
        public MaterialSimulationController(ISupplierDomain iSupplierDomain,IMaterialSimulationDomain iMaterialSimulationDomain,IMaterialDomain materialDomain,IPreProductDomain preProductDomain)
            : base(iSupplierDomain, materialDomain)
        {
            _materialSimulationDomain = iMaterialSimulationDomain;
            _preProductDomain = preProductDomain;
        }
        // GET: Master/MaterialSimulation
        public ActionResult Index()
        {
            var model = new MaterialSimulationViewModel()
            {
                Grid = GenerateGrid(),IsPlan = false,    
            };
            return View(model);
        }
     
        public ActionResult Generate(MaterialSimulationViewModel model,ProductPlanViewModel mProductPlan)
        {
            if (!model.SimulationType.Equals(Constants.SimulationType.Material))
            {
                var productPlanViewModel = new ProductPlanViewModel();
                productPlanViewModel.From = model.From;
                productPlanViewModel.To = model.To;
                //productPlanViewModel.Grid = GenerateGridProduct(model);
                productPlanViewModel.productName = _materialSimulationDomain.GetPreProductName(ConvertHelper.ConvertToDateTimeFull(model.From),ConvertHelper.ConvertToDateTimeFull( model.To));
                productPlanViewModel.LstPreProductPlanSimuItem =
                    _materialSimulationDomain.GenerateProductPlan(ConvertHelper.ConvertToDateTimeFull(model.From),
                        ConvertHelper.ConvertToDateTimeFull(model.To),
                        model.InventoryUnderRetrieval.Equals(Constants.Choice.Yes),
                        model.AcceptedMaterialOnly.Equals(Constants.Choice.Yes),
                        model.MaterialUsedInOtherCommands.Equals(Constants.Choice.Yes));
                return PartialView("MaterialSimulation/_PartialMaterialSimulationProductPlan", productPlanViewModel);
            }
            else
            {
                var result = _materialSimulationDomain.GenerateMaterial(ConvertHelper.ConvertToDateTimeFull(model.From), ConvertHelper.ConvertToDateTimeFull(model.To), model.InventoryUnderRetrieval.Equals(Constants.Choice.Yes), model.AcceptedMaterialOnly.Equals(Constants.Choice.Yes), model.MaterialUsedInOtherCommands.Equals(Constants.Choice.Yes), model.SelectMaterial);
                var materialModel = new MaterialViewModel();
                materialModel.MaterialCode = model.SelectMaterial;
                var material = _materialDomain.GetById(model.SelectMaterial);
                materialModel.MaterialName = material!=null?material.F01_MaterialDsp:"";
                materialModel.Chart = result;
                return PartialView("MaterialSimulation/_PartialViewMaterialSimulationMaterial", materialModel);
            }
            
            
        }
        private Grid GenerateGrid()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("MaterialGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(5)
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchByName", "Material", new { Area = "ProductionPlanning" }))
                .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F01_MaterialCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F01_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100));
        }

       
        [AllowAnonymous]
        [HttpPost]
        public JsonResult CheckDate(string @from,string to = null)
        {
           var currentdate = DateTime.Now;
            var fromDate =!string.IsNullOrEmpty(from)? ConvertHelper.ConvertToDateTimeFull(from):currentdate.AddDays(-1);
            if (string.IsNullOrEmpty(to))
            {
                return currentdate <= fromDate ? Json(false, JsonRequestBehavior.AllowGet) : Json(currentdate.AddDays(-90) <= fromDate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var toDate = ConvertHelper.ConvertToDateTimeFull(to);
                if (toDate.Date < currentdate.Date)
                {
                    return Json(MessageResource.MSG33, JsonRequestBehavior.AllowGet);
                }
                if (currentdate.AddDays(90) <= toDate)
                {
                    return  Json(MessageResource.MSG33, JsonRequestBehavior.AllowGet);
                }
                if (!string.IsNullOrEmpty(from) && (fromDate.AddDays(90) <= toDate||fromDate > toDate))
                {
                    return Json(MessageResource.MSG31, JsonRequestBehavior.AllowGet);
                }
            }
         
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        #region Pop UP

        public ActionResult PopUp(string from ,string to,int inRetrieval,int acceptedMaterial,int mCommand,string preProductCode,string preProductName, string productDate, string lot )
        {
            var productPlanPopUpModel = new ProductPlanPopUp()
            {
                Command = productDate,
                LotQuantity = lot,
                PreProductCode = preProductCode,
                PreProductName = preProductName,
                Grid = GenerateGridPopUP(from, to, inRetrieval==(int)Constants.Choice.Yes, acceptedMaterial==(int)Constants.Choice.Yes, mCommand==(int)Constants.Choice.Yes, preProductCode, productDate)
                
            };
            return View("MaterialSimulation/_PartialProductionPlan", productPlanPopUpModel);
        }

        public ActionResult SearchPopUp(bool iRetrieval, bool acceptedM, string preProductCode, string productDate,
            string lot, string startDate, string endDate,bool mCommands,GridSettings gridSettings)
        {
            var result = _materialSimulationDomain.GetDataPopUp(ConvertHelper.ConvertToDateTimeFull(startDate),ConvertHelper.ConvertToDateTimeFull(endDate),preProductCode,
                productDate,
                iRetrieval,
                acceptedM, mCommands,gridSettings);
            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        private Grid GenerateGridPopUP(string from ,string to,bool inRetrieval,bool acceptedMaterial,bool mCommand, string preProductCode, string productDate)
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("PopUpGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(false)
               
                .SetPageLoading(true)
                .SetAutoload(true)
                .SetSearchUrl(urlHelper.Action("SearchPopUp", "MaterialSimulation",
                    new
                    {
                        Area = "ProductionPlanning",
                        iRetrieval = inRetrieval,
                        acceptedM = acceptedMaterial,
                        mCommands = mCommand,
                        preProductCode,
                        productDate,
                        startDate = from,
                        endDate = to
                    }))
                .SetDefaultSorting("MaterialCode", SortOrder.Asc)
                .SetFields(

                    new Field("MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("MaterialName")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("RequiredQuantity")
                        .SetTitle("Required Qty")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetWidth(100),
                    new Field("Stock")
                        .SetTitle("Stock")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetWidth(100),
                    new Field("Remainder")
                        .SetTitle("Remainder")
                        .SetItemTemplate("gridHelper.numberFormat")
                        .SetWidth(100));
        }

        #endregion
       
    }
}