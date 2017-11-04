using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.SystemManagement.Controllers
{
    [MvcAuthorize("TCSC031F")]
    public class MonthlyProcessController : KCSG.Web.Controllers.BaseController
    {
        private IMonthlyProcessDomain _imMonthlyProcessDomain;

        public MonthlyProcessController(IMonthlyProcessDomain iMonthlyProcessDomain)
        {
            _imMonthlyProcessDomain = iMonthlyProcessDomain;
        }
        //
        // GET: /SystemManagement/MonthProcess/
        public ActionResult Index()
        {
            return View();
        }

        public  ActionResult MonthlyProcess()
        {
            var result =  _imMonthlyProcessDomain.MonthlyProcess();
            if (result)
            {
                return Json(new {Success = true, Message = MessageResource.MSG11_Systemmanagerment},
                    JsonRequestBehavior.AllowGet);
            }
            return Json(new {Success = false, Message = MessageResource.MSG11_Error}, JsonRequestBehavior.AllowGet);
        }
    }
}