using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.EnvironmentManagement
{
    public class EnvironmentManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "EnvironmentManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "EnvironmentManagement_default",
                "EnvironmentManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}