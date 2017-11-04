using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem
{
    public class TabletisingCommandSubSystemAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "TabletisingCommandSubSystem";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "TabletisingCommandSubSystem_default",
                "TabletisingCommandSubSystem/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}