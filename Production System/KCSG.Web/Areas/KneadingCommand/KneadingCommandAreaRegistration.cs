using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.KneadingCommand
{
    public class KneadingCommandAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "KneadingCommand";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "KneadingCommand_default",
                "KneadingCommand/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}