using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Domains.EnvironmentManagement;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ProductMasterManagement;
using KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput;
using KCSG.Web.Attributes;
using Microsoft.AspNet.SignalR.Messaging;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN044F")]
    public class ProductMasterManagementController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly IEnvironmentBaseDomain _environmentBaseDomain;
        private readonly IProductMasterManagementDomain _productMasterManagementDomain;
        #endregion

        public ProductMasterManagementController(IEnvironmentBaseDomain environmentBaseDomain, IProductMasterManagementDomain productMasterManagementDomain)
        {
            _environmentBaseDomain = environmentBaseDomain;
            _productMasterManagementDomain = productMasterManagementDomain;
        }

        //
        // GET: /EnvironmentManagement/ProductMasterManagement/
        public ActionResult Index()
        {
            ViewBag.ListLocation = _environmentBaseDomain.GetLocationItemByType("1").Select(x => new SelectListItem
            {
                Text = x.F80_Name,
                Value = x.F80_Id.ToString()
            });

            ViewBag.ListProductName = _productMasterManagementDomain.GetProducts().Select(x => new SelectListItem
            {
                Text = x.F09_ProductDesp,
                Value = x.F09_ProductCode
            });
            var model = new ProductMasterManagementViewModel();
            model.Grid = GenerateGrid();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Search(ProductMasterManagementViewModel model, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F85_Code";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _productMasterManagementDomain.SearchCriteria(model.Location, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        #region private Methods
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
                .SetPageLoading(true).OnDataLoaded("OnloadSuccess")
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("Search", "ProductMasterManagement",
                    new { Area = "EnvironmentManagement" }))
                .SetDefaultSorting("F85_Code", SortOrder.Asc)
                .SetFields(
                    new Field("F85_Code")
                        .SetTitle(" ")
                        .SetWidth(30)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("ProductName")
                        .SetTitle("Product Name")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(130),
                    new Field("F85_M_Usl")
                        .SetTitle("USL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F85_M_Lsl")
                        .SetTitle("LSL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F85_M_Ucl")
                        .SetTitle("UCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F85_M_Lcl")
                        .SetTitle("LCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F85_R_Usl")
                        .SetTitle("USL")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F85_R_Lsl")
                        .SetTitle("LSL")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F85_No_Lot")
                        .SetTitle("Lot(s)")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                );
        }
        #endregion

        [HttpPost]
        public ActionResult AddOrUpdate(ProductMasterManagementViewModel model)
        {
            if (model.isCreate)
            {
                try
                {
                    //var checkLocation = _productMasterManagementDomain.CheckLocation(Int32.Parse(model.Location));
                    //if(!checkLocation)
                    //    return Json(new{Success=false,Message=EnvironmentResource.MSG8});

                    //var isChecked = _productMasterManagementDomain.CheckValueEntered(Int32.Parse(model.Location));
                    //if (!isChecked)
                    //    return Json(new { Success = false, Message = EnvironmentResource.MSG17 });

                    var item = Mapper.Map<ProductMasterManagementItem>(model);
                    _productMasterManagementDomain.Add(item);
                    return Json(new { Success = true, Message = EnvironmentResource.MSG11 });

                }
                catch (Exception e)
                {
                    return Json(new {Success = false, Message = e.Message});
                }
            }
            else
            {
                try
                {
                    var item = Mapper.Map<ProductMasterManagementItem>(model);
                    _productMasterManagementDomain.Edit(item);
                    return Json(new { Success = true, Message = EnvironmentResource.MSG9 });

                }
                catch (Exception e)
                {
                    return Json(new { Success = false, Message = e.Message });
                }
            }
        }

        

        [HttpPost]
        public ActionResult Delete(string code)
        {
            try
            {
                _productMasterManagementDomain.Delete(code);
                return Json(new { Success = true, Message = EnvironmentResource.MSG10 });
                
            }
            catch (Exception e)
            {
                return Json(new{Success=false,Message=e.Message});
            }
        }
    }
}