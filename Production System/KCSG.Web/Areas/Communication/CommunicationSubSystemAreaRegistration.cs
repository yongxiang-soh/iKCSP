using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.Communication
{
    public class CommunicationSubSystemAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Communication";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Communication_default",
                "Communication/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}