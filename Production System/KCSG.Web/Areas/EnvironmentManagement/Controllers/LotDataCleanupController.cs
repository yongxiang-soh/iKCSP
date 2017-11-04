using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.LotDataCleanup;
using KCSG.Web.Areas.TabletisingCommandSubSystem.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN043F")]
    public class LotDataCleanupController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly ILotDataCleanupDomain _lotDataCleanupDomain;

        #endregion

        #region Constructor

        public LotDataCleanupController(ILotDataCleanupDomain lotDataCleanupDomain)
        {
            _lotDataCleanupDomain = lotDataCleanupDomain;
        }

        #endregion

        //
        // GET: /EnvironmentManagement/LotDataCleanup/
        public ActionResult Index()
        {
            var result = _lotDataCleanupDomain.GetLots();
            var te84EnvLot = _lotDataCleanupDomain.GetListTe84EnvLots();
            var model = new LotDataCleanupViewModel();
            var oldCutOfDate = te84EnvLot.FirstOrDefault().F84_S_Time.Date.ToString("dd/MM/yyyy");
            var oldCutOfTime = te84EnvLot.FirstOrDefault().F84_S_Time.ToString("HH:mm");

            var lots = result;
            model.OldCutOffDate = oldCutOfDate;
            model.OldCutOfTime = oldCutOfTime;
            model.Lot1 = lots;

            return View(model);
        }

        [HttpPost]
        public ActionResult Testing(string stringNewCutOffDate, string stringNewCutOffTime)
        {
            var lots = _lotDataCleanupDomain.Testing(stringNewCutOffDate, stringNewCutOffTime);
            return Json(lots);
        }

        [HttpPost]
        public ActionResult Delete(string stringNewCutOffDate, string stringNewCutOffTime)
        {
            var result = _lotDataCleanupDomain.Delete(stringNewCutOffDate, stringNewCutOffTime);
            if (result)
            {
                return Json(new
                {
                    Success = true,
                    Message = EnvironmentResource.MSG10
                });
            }
            return Json(new
            {
                Success = false,
                Message = EnvironmentResource.MSG7
            });
        }
    }
}