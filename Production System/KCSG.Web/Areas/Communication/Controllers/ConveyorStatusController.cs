using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.Communication.ViewModels;
using Resources;

namespace KCSG.Web.Areas.Communication.Controllers
{
    public class ConveyorStatusController : KCSG.Web.Controllers.BaseController
    {
        private readonly IConveyorDomain _conveyorDomain;

        public ConveyorStatusController(IConveyorDomain conveyorDomain)
        {
            _conveyorDomain = conveyorDomain;
        }

        // GET: Communication/ConveyorStatus
        public ActionResult Index(int communication)
        {
            var model = new ConveyorStatusViewModel()
            {
                ConveyorGrid = GenerateGrid(communication)
            };
            return View(model);
        }

        #region private

        public ActionResult Search(int comunication, GridSettings gridSettings)
        {
            if (string.IsNullOrEmpty(gridSettings.SortField))
            {
                gridSettings.SortField = "F05_AddDate";
                gridSettings.SortOrder = SortOrder.Asc;
            }
            var result = _conveyorDomain.Search(comunication, gridSettings);
            if (!result.IsSuccess)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(string code)
        {
            var entity = _conveyorDomain.GetConveyor(code);

            return Json(entity, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(ConveyorStatusViewModel model)
        {
            try
            {
                var item = _conveyorDomain.GetConveyor(model.ConveyorCode);
                item.F05_BufferUsing = model.UsingBuffer.Value;
                item.F05_StrRtrSts = model.ConveyorStatus.Value.ToString("D");
                _conveyorDomain.UpdateConveyor(item);
                return Json(new {Success = true, Message = MessageResource.MSG9}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Success = false, Message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        public Grid GenerateGrid(int comunication)
        {
            var urlHelper = new UrlHelper(ControllerContext.RequestContext);

            return new Grid("ConveyorGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(true).OnDataLoaded("LoadConveyorSuccess")
                .SetSearchUrl(urlHelper.Action("Search", "ConveyorStatus",
                    new {Area = "Communication", comunication = comunication}))
                .SetDefaultSorting("F05_AddDate", SortOrder.Asc)
                .SetFields(
                    new Field("F05_ConveyorCode")
                        .SetTitle(" ").SetSorting(false).SetWidth(30)
                        .SetItemTemplate("gridHelper.generateRadiobox"),
                    new Field("F05_ConveyorCode")
                        .SetWidth(100).SetSorting(false)
                        .SetTitle("Conveyor")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false),
                    new Field("F05_TerminalNo")
                        .SetTitle("Terminal No").SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetWidth(100),
                    new Field("F05_MaxBuffer")
                        .SetTitle("Max Buffer").SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("F05_BufferUsing")
                        .SetTitle("Using Buffer")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn")
                );
        }

        #endregion
    }
}