using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN024F")]
    public class CreepingAndRollSpeedDurationController : KCSG.Web.Controllers.BaseController
    {
        public ICreepingAndRollSpeedDurationDomain _CreepingAndRollSpeedDurationDomain;

        public CreepingAndRollSpeedDurationController(
            ICreepingAndRollSpeedDurationDomain creepingAndRollSpeedDurationDomain)
        {
            _CreepingAndRollSpeedDurationDomain = creepingAndRollSpeedDurationDomain;
        }
        // GET: EnvironmentManagement/CreepingAndRollSpeed
        public ActionResult Index()
        {
            var model = new CreepingAndRollSpeedDurationModel()
            {
                SearchCriteriaModel = new SearchCriteriaModel()
                {
                    Location =  "Machine",
                    EndDate = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy"),
                    StartDate = DateTime.Now.AddDays(-89).ToString("dd/MM/yyyy"),
                },
                LeftModel = new ChartModel() { ChartName = "Left" },
                RightModel = new ChartModel() { ChartName = "Right" },
                RollModel = new ChartModel() { ChartName = "Roll" }
                
            };
            ViewBag.ListLocation = new SelectList(EnumsHelper.GetListItemsWithDescription<Constants.RollMachine>(),"Value","Text");
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchCriteriaModel searchModel)
        {
            var result =
                _CreepingAndRollSpeedDurationDomain.Search(ConvertHelper.ConvertToDateTimeFull(searchModel.StartDate),
                    ConvertHelper.ConvertToDateTimeFull(searchModel.EndDate),
                    (Constants.RollMachine) Enum.Parse(typeof (Constants.RollMachine), searchModel.Location),
                    searchModel.Mode);

            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        
    }
}