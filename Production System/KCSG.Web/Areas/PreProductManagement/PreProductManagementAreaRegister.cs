using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.PreProductManagement
{
    public class PreProductManagementAreaRegister : AreaRegistration
    {
        public override string AreaName
        {
            get { return "PreProductManagement"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "PreProductManagement_default",
                "PreProductManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}