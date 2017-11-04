using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.SystemManagement
{
    public class SystemManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SystemManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SystemManagement_default",
                "SystemManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}