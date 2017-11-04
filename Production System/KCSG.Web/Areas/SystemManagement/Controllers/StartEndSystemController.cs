using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Web.Areas.SystemManagement.ViewModels;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.SystemManagement.Controllers
{
   [MvcAuthorize("TCSC011F")]
    public class StartEndSystemController : KCSG.Web.Controllers.BaseController
    {
        #region Domain Declaration

        private readonly IStartEndSystemDomain _startEndSystemDomain;

        /// <summary>
        /// Identity domain - provides function to analyze claim identity in HttpContext.
        /// </summary>
        private readonly IIdentityService _identityDomain;

        #endregion

        #region Constructors

        /// <summary>
        /// Inititate controller with dependency injections.
        /// </summary>
        /// <param name="startEndSystemDomain"></param>
        /// <param name="identityDomain"></param>
        public StartEndSystemController(IStartEndSystemDomain startEndSystemDomain, IIdentityService identityDomain)
        {
            _startEndSystemDomain = startEndSystemDomain;
            _identityDomain = identityDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders index page.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(bool? reload)
        {
            var model = new StartEndSystemViewModel();
            var device = _startEndSystemDomain.GetStatus();
            if (device != null)
            {
                model.Status = device.F14_DeviceStatus.Equals(Constants.StatusStart.NormalStart.ToString("D"))
                    ? MessageResource.SystemIsStart
                    : MessageResource.SystermIsEnd;
                model.IsStart = device.F14_DeviceStatus.Equals(Constants.StatusStart.NormalStart.ToString("D"));
                model.Device = true;
            }
            else
            {
                model.Status = MessageResource.Msg1;
                model.IsStart = false;
                model.Device = false;
                
            }
            model.Reload = reload.HasValue ? reload.Value : false;
            return View(model);
        }

        [HttpPost]
        public ActionResult EndSystem(StartEndSystemViewModel model)
        {
            // Find terminal no in HttpContext.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            ResponseResult result;
            result = model.IsStart ? _startEndSystemDomain.EndSystem(model.StatusEnd, terminalNo) : _startEndSystemDomain.EndSystem(model.StatusStart, terminalNo);
          
            if (result.IsSuccess)
            {
                return RedirectToAction("index",new {reload = true});
            }
            return Json(new{Susscess = false}, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}