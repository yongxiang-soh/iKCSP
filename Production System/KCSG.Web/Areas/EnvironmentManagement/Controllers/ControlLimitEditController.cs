using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.ControlLimitEdit;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN074F")]
    public class ControlLimitEditController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly IEnvironmentBaseDomain _environmentBaseDomain;
        private readonly IControlLimitEditDomain _controlLimitEditDomain;

        #endregion

        #region Constructor

        public ControlLimitEditController(IEnvironmentBaseDomain environmentBaseDomain,
            IControlLimitEditDomain controlLimitEditDomain)
        {
            _environmentBaseDomain = environmentBaseDomain;
            _controlLimitEditDomain = controlLimitEditDomain;
        }

        #endregion

        //
        // GET: /EnvironmentManagement/ControlLimitEdit/
        public ActionResult Index()
        {
            ViewBag.ListLocation = _environmentBaseDomain.GetLocationItemByType("1").Select(x => new SelectListItem
            {
                Text = x.F80_Name,
                Value = x.F80_Id.ToString()
            });
            var model = new ControlLimitEditViewModel();
            model.Grid = GenerateGrid();

            model.From = DateTime.Now.ToString("dd/M/yyyy");
            model.To = DateTime.Now.ToString("dd/M/yyyy");
            return View(model);
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Search(string location, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F80_Id";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _controlLimitEditDomain.SearchCriteria(location, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        #region Private method

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
                .SetAutoload(false)
                .SetSearchUrl(urlHelper.Action("Search", "ControlLimitEdit",
                    new {Area = "EnvironmentManagement"}))
                .SetDefaultSorting("F80_Id", SortOrder.Asc)
                .SetFields(
                    new Field("F80_Id")
                        .SetTitle(" ")
                        .SetWidth(30)
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("DateFromTo")
                        .SetTitle("Date (From – To)")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(130),
                    new Field("F80_T_Ucl")
                        .SetTitle("T_UCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(130),
                    new Field("F80_T_Lcl")
                        .SetTitle("T_LCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100),
                    new Field("F80_T_Mean")
                        .SetTitle("T_Mean")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100),
                    new Field("F80_T_Sigma")
                        .SetTitle("T_Sigma")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F80_T_Cp")
                        .SetTitle("T_Cp")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F80_T_Cpk")
                        .SetTitle("T_Cpk")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_T_Range")
                        .SetTitle("T_Range")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Ucl")
                        .SetTitle("H_UCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(130),
                    new Field("F80_H_Lcl")
                        .SetTitle("H_LCL")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100),
                    new Field("F80_H_Mean")
                        .SetTitle("H_Mean")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetWidth(100),
                    new Field("F80_H_Sigma")
                        .SetTitle("H_Sigma")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F80_H_Cp")
                        .SetTitle("H_Cp")
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F80_H_Cpk")
                        .SetTitle("H_Cpk")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat"),
                    new Field("F80_H_Range")
                        .SetTitle("H_Range")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberFormat")
                );
        }

        #endregion

        [HttpPost]
        public ActionResult Update(ControlLimitEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = Mapper.Map<ControlLimitEditItem>(model);
                    _controlLimitEditDomain.Update(item);
                    return Json(new {Success = true, Message = EnvironmentResource.MSG9});
                }
                catch (Exception e)
                {
                    return Json(new {Success = false, Message = e.Message});
                }
            }
            return Json(new {Success = false});
        }
    }
}