using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper.Execution;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Common.ViewModels;
using KCSG.Web.Controllers;

namespace KCSG.Web.Areas.Common.Controllers
{
    public class CommonController : BaseController
    {
        #region Domain Declaration

        private readonly ICommonSearchDomain _commonSearchDomain;
        #endregion

        #region Constructor

        public CommonController(ICommonSearchDomain commonSearchDomain)
        {
            _commonSearchDomain = commonSearchDomain;
        }
        #endregion


        public ActionResult LoadPalletNoSelectWithParameter(string modelId)
        {
            var model = new CommonViewModel();
            model.Grid = GenerateGridPalletNoWithParameter();
            model.ListLabel = "List of Pallet No.";
            model.ModelType = modelId;
            return PartialView("_PartialSelectView", model);
        }

        public ActionResult LoadPONoWithParameter(string id)
        {
            var model = new CommonViewModel();
            model.Grid = GenerateGridPONOWithParameter();
            model.ListLabel = "List of P.O.No. Select";
            model.ModelType = id;
            return PartialView("_PartialSelectView", model);
        }
        public ActionResult LoadProductLotNoWithParameter(string id)
        {
            var model = new CommonViewModel();
            model.Grid = GenerateGridProductLotNoWithParameter();
            model.ListLabel = "List of Product Lot No.";
            model.ModelType = id;
            return PartialView("_PartialSelectView", model);
        }

        //public ActionResult LoadProductLabels()
        //{
        //    var model = new CommonViewModel();
        //    model.Grid = InitiateProductLabelsList();
        //    model.ModelType = "labelPrintManagement";
        //    model.ListLabel = "Product labels list";
        //    return PartialView("_PartialSelectView", model);
        //}

        public ActionResult LoadSelected(string modelId)
        {
        
            var model = new CommonViewModel();
            switch (modelId)
            {
                case "modalSupplierCodeSelect":
                    model.Grid = GenerateGridSupplier();
                    model.ListLabel = "List of Supplier";
                    break;
                case "modalContainerTypeSelect":
                    model.Grid = GenerateGridContainer();
                    model.ListLabel = "List of Container Type";
                    break;
                case "modalMaterialCodeSelect":
                    model.Grid = GenerateGridMaterial();
                    model.ListLabel = "List of Material";
                    break;
                case "modalPreProductCodeSelect":
                    model.Grid = GenerateGridPreProduct();
                    model.ListLabel = "List of  Pre-Product ";
                    break;
                case "modalProductCodeSelect":
                    model.Grid = GenerateGridProduct();
                    model.ListLabel = "List of Product";
                    break;
                case "modalSupplementMaterialCodeSelect":
                    model.Grid = GenerateGridSupplementMaterial();
                    model.ListLabel = "List of Supplement Material";
                    break;
                case "modalEndUserCodeSelect":
                    model.Grid = GenerateGridEndUser();
                    model.ListLabel = "List of  End Users";
                    break;
                case "modalShippingNoSelect":
                    model.Grid = GenerateGridShippingNo();
                    model.ListLabel = "List of Shipping No";
                    break;
                case "modalProductLotNoSelect":
                    model.Grid = GenerateGridProductLotNo();
                    model.ListLabel = "List of Product Lot No.";
                    break;
                case "modalPONOSelect":
                    model.Grid = GenerateGridPONO();
                    model.ListLabel = "List of P.O NO";
                    break;
                case "modalPalletNoSelect":
                    model.Grid = GenerateGridPalletNo();
                    model.ListLabel = "List of Pallet No";
                    break;
                case "modalTabletingLineSelect":
                    model.Grid = GenerateGridTabletingLine();
                    model.ListLabel = "List of Tableting Line";
                    break;
                case "modalShelfNoSelect":
                    model.Grid = GenerateGridShelfNo();
                    model.ListLabel = "List of Empty Shelf";
                    break;
                case "modalOutOfPlanProductCodeSelect":
                    model.Grid = GenerateGridOutOfPlanProduct();
                    model.ListLabel = "List Of Out Of Plan Product";
                    break;
                case "modalLabelPrintManagement":
                    model.Grid = GenerateProductLabelsList();
                    model.ListLabel = "List  Product";
                    break;
                default:
                    break;
            }
            model.ModelType = modelId;
            return PartialView("_PartialSelectView", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchPONo(CommonViewModel model, GridSettings gridSettings)
        {
            ResponseResult<GridResponse<CommonSearchItem>> result;
            result = _commonSearchDomain.GetPONo(model.ModelType,model.KeyWord,gridSettings);

            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearProductLotNo(CommonViewModel model, GridSettings gridSettings)
        {
            ResponseResult<GridResponse<CommonSearchItem>> result;
            result = _commonSearchDomain.GetProductLotNoWithProductCode(model.KeyWord, gridSettings,model.ModelType);

            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchPalletNo(CommonViewModel model, GridSettings gridSettings)
        {
            try
            {
                ResponseResult<GridResponse<CommonSearchItem>> result;
                result = _commonSearchDomain.GetPalletNoWithStockFlag(model.KeyWord, gridSettings);

                if (!result.IsSuccess)
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                return Json(result.Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                throw;
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SearchByKeyWord(CommonViewModel model, GridSettings gridSettings)
        {
            ResponseResult<GridResponse<CommonSearchItem>> result; 
            switch (model.ModelType)
            {
                case "modalContainerTypeSelect":
                    result = _commonSearchDomain.GetContainerType(model.KeyWord, gridSettings);
                    break;
                case "modalMaterialCodeSelect":
                    result = _commonSearchDomain.GetMaterialCode(model.KeyWord, gridSettings);
                    break;
                case "modalPreProductCodeSelect":
                    result = _commonSearchDomain.GetPreproductCode(model.KeyWord, gridSettings);
                    break;
                case "modalProductCodeSelect":
                    result = _commonSearchDomain.GetProductCode(model.KeyWord, gridSettings);
                    break;
                case "modalOutOfPlanProductCodeSelect":
                    result = _commonSearchDomain.GetOutOfPlanProduct(model.KeyWord, gridSettings);
                    break;
                case "modalSupplementMaterialCodeSelect":
                    result = _commonSearchDomain.GetSupMatCode(model.KeyWord, gridSettings);
                    break;
                case "modalEndUserCodeSelect":
                    result = _commonSearchDomain.GetEndUserCode(model.KeyWord, gridSettings);
                    break;
                case "modalShippingNoSelect":
                    result = _commonSearchDomain.GetShippingNo(model.KeyWord, gridSettings);
                    break;
                case "modalProductLotNoSelect":
                    result = _commonSearchDomain.GetProductLotNo(model.KeyWord, gridSettings);
                    break;
                case "modalPONOSelect":
                    result = _commonSearchDomain.GetPONo("",model.KeyWord, gridSettings);
                    break;
                case "modalPalletNoSelect":
                    result = _commonSearchDomain.GetProductPalletNo(model.KeyWord, gridSettings);
                    break;
                case "modalTabletingLineSelect":
                    result = _commonSearchDomain.GetTabletingLine(model.KeyWord, gridSettings);
                    break;
                case "modalShelfNoSelect":
                    result = _commonSearchDomain.GetShelfNo(model.KeyWord, gridSettings);
                    break;
                case "modalLabelPrintManagement":
                    result = _commonSearchDomain.FindProductLabelList(model.KeyWord, gridSettings);
                    break;
                default:
                  result=  _commonSearchDomain.GetSupplierCodes(model.KeyWord, gridSettings);
                    break;

            }
           
            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        #region Private Method
        #region Supplier code
        private Grid GenerateGridSupplier()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F04_SupplierCode", SortOrder.Asc)
                .SetFields(
                    new Field("F04_SupplierCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F04_SupplierCode")
                        .SetTitle("Supplier Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F04_SupplierName")
                        .SetTitle("Supplier Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                          new Field("F04_MaxLoadAmount").SetVisible(false)
                       
                );
        }

        private Grid GenerateProductLabelsList()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F56_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F56_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F56_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false),
                    new Field("F09_Label")
                        .SetTitle("Product label")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false),
                    new Field("F56_KndCmdNo")
                        .SetTitle("Kneading Command")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_TabletSize")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false),
                    new Field("F09_TabletSize2")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false),
                    new Field(nameof(TM09_Product.F09_TabletType))
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false)

                );
        }
        //private Grid InitiateProductLabelsList()
        //{
        //    var urlHelper = new UrlHelper(ControllerContext.RequestContext);

        //    return new Grid("ProductLabelsList")
        //        .SetMode(GridMode.Listing)
        //        .SetWidth("100%")
        //        .SetSorting(true)
        //        .SetPaging(true)
        //        .SetPageSize(10)
        //        .SetPageLoading(true)
        //        .SetAutoload(false)
        //        .SetSelected(true)
        //        .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
        //         .SetDefaultSorting("F09_Label", SortOrder.Asc)
        //        .SetFields(
        //            new Field("F09_Label")
        //                .SetTitle("Product label")
        //                .SetItemTemplate("gridHelper.generateNameColumn")
        //                .SetWidth(150),
        //            new Field("F42_KneadingCommand")
        //                .SetTitle("Kneading command")
        //                .SetItemTemplate("gridHelper.generateNameColumn")
        //                .SetWidth(150)
        //        );
        //}

        #endregion

        #region Container
        private Grid GenerateGridContainer()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F08_ContainerType", SortOrder.Asc)
                .SetFields(
                    new Field("F08_ContainerType")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F08_ContainerType")
                        .SetTitle("Container Type")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F08_ContainerName")
                        .SetTitle("Container Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion

        #region material
        private Grid GenerateGridMaterial()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F01_MaterialCode", SortOrder.Asc)
                .SetFields(
                    new Field("F01_MaterialCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F01_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_MaterialDsp")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                         new Field("F01_Unit")
                        .SetVisible(false)
                        .SetWidth(150),
                    new Field("F01_PackingUnit")
                        .SetVisible(false),
                    new Field("F01_EntrustedClass")
                        .SetVisible(false),
                     new Field("F01_LiquidClass")
                        .SetVisible(false),
                    new Field("F01_RtrPosCls")
                        .SetVisible(false)
                        
                        
                );
        }
        #endregion

        #region Pre Product
        private Grid GenerateGridPreProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F03_PreproductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F03_PreproductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F03_PreproductCode")
                        .SetTitle("Pre-Product  Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F03_PreproductName")
                        .SetTitle("Pre-Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion
        #region  Product
        private Grid GenerateGridProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F09_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F09_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F09_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion
        #region  OutOfPlanProduct
        private Grid GenerateGridOutOfPlanProduct()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F09_ProductCode", SortOrder.Asc)
                .SetFields(
                    new Field("F09_ProductCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F09_ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F09_ProductDesp")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                        new Field("F58_PrePdtLotNo")
                        .SetTitle("PrePdt LotNo")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F58_ProductLotNo")
                        .SetTitle("Lot No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F58_TbtCmdEndPackAmt")
                        .SetTitle("Pack Qty")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F58_TbtCmdEndFrtAmt")
                        .SetTitle("Fraction")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("F58_TbtEndDate")
                        .SetTitle("Tabletising ")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate")
                );
        }
        #endregion
        #region  End User
        private Grid GenerateGridEndUser()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F10_EndUserCode", SortOrder.Asc)
                .SetFields(
                    new Field("F10_EndUserCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F10_EndUserCode")
                        .SetTitle("End User Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F10_EndUserName")
                        .SetTitle("End User Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion

        #region  Supplement Material
        private Grid GenerateGridSupplementMaterial()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F15_submaterialcode", SortOrder.Asc)
                .SetFields(
                    new Field("F15_submaterialcode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F15_submaterialcode")
                        .SetTitle("Sup. Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F15_materialdsp")
                        .SetTitle("Sup. Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F01_Unit")
                        .SetVisible(false)
                );
        }
        #endregion

        #region  Shipping No
        private Grid GenerateGridShippingNo()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("ShippingNo", SortOrder.Asc)
                .SetFields(
                    new Field("ShippingNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShippingNo")
                        .SetTitle("Shipping No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("ProductCode")
                        .SetTitle("Product Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("EndUserName")
                        .SetTitle("End User Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion
        #region  Product Lot No
        private Grid GenerateGridProductLotNo()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F40_ProductLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("F40_ProductLotNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F40_ProductLotNo")
                        .SetTitle("Product Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                    
                );
        }
        #endregion
        #region  PO NO
        private Grid GenerateGridPONO()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new {Area = "Common" }))
                .SetDefaultSorting("F30_PrcOrdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F30_PrcOrdNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F30_PrcOrdNo")
                        .SetTitle("P.O.No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F30_PrtDvrNo")
                        .SetTitle("Partial Delivery")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F33_MaterialCode")
                        .SetTitle("MaterialCode")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false),
                    new Field("MaterialName")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false)
                );
        }
        #endregion
        #region  Pallet No
        private Grid GenerateGridPalletNo()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F40_PalletNo", SortOrder.Asc)
                .SetFields(
                    new Field("F40_PalletNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F40_PalletNo")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion

        #region  Tableting Line
        private Grid GenerateGridTabletingLine()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F14_DeviceCode", SortOrder.Asc)
                .SetFields(
                    new Field("F14_DeviceCode")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F14_DeviceCode")
                        .SetTitle("Tableting Line")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                   new Field("F14_DeviceName")
                        .SetTitle("Tableting Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion

        #region  Shelf No
        private Grid GenerateGridShelfNo()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchByKeyWord", "Common", new { Area = "Common" }))
                .SetDefaultSorting("ShelfNo", SortOrder.Asc)
                .SetFields(
                    new Field("ShelfNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ShelfNo")
                        .SetTitle("Shelf No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                );
        }
        #endregion

        #region Get pallet no with stock flag is 0
        private Grid GenerateGridPalletNoWithParameter()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(5)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchPalletNo", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F33_PalletNo", SortOrder.Asc)
                .SetFields(
                    new Field("F33_PalletNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F33_PalletNo")
                        .SetTitle("Pallet No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F33_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetVisible(false)
                        .SetWidth(150),
                    new Field("MaterialName")
                        .SetTitle("Material Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetVisible(false)
                        .SetWidth(150),
                    new Field("F01_PackingUnit")
                        .SetTitle("Packing Unit")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetVisible(false)
                        .SetWidth(150),
                    new Field("F01_Unit")
                        .SetTitle("Unit")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetVisible(false)
                        .SetWidth(150)

                );
        }
        #endregion


        #region Get PONo with parameter
        private Grid GenerateGridPONOWithParameter()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);
            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearchPONo", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F30_PrcOrdNo", SortOrder.Asc)
                .SetFields(
                    new Field("F30_PrcOrdNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F30_PrcOrdNo")
                        .SetTitle("P.O.No")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F30_PrtDvrNo")
                        .SetTitle("Partial Delivery")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150),
                    new Field("F33_MaterialCode")
                        .SetTitle("Material Code")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)
                        .SetVisible(false)
                );
        }
        #endregion
        #region Get Product Lot No with prameter

        #endregion
        private Grid GenerateGridProductLotNoWithParameter()
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("SupplierGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(10)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSelected(true)
                .SetSearchUrl(urlHelper.Action("SearProductLotNo", "Common", new { Area = "Common" }))
                .SetDefaultSorting("F40_ProductLotNo", SortOrder.Asc)
                .SetFields(
                    new Field("F40_ProductLotNo")
                        .SetWidth(20).SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("F40_ProductLotNo")
                        .SetTitle("Product Lot No.")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(150)

                );
        }
        #endregion
    }
}