using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.SystemManagement.Controllers
{
    [MvcAuthorize("TCSC021F")]
    public class DailyProcessController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declare

        private readonly IDailyProcessDomain _dailyProcessDomain;

        private readonly IIdentityService _identityService;

        #endregion
        #region Contructor

        public DailyProcessController(IDailyProcessDomain dailyProcessDomain, IIdentityService identityService)
        {
            _dailyProcessDomain = dailyProcessDomain;
            _identityService = identityService;
        }

        #endregion
        //
        // GET: /SystemManagement/DailyProcess/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DailyProcess()
        {
            try
            {
                // Find terminal no from identity attached to request.
                var terminalNo = _identityService.FindTerminalNo(HttpContext.User.Identity);
                _dailyProcessDomain.DailyProcess(terminalNo);
                return Json(new { Success = true, Message = MessageResource.MSG8_SystemManagement }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
	}
}