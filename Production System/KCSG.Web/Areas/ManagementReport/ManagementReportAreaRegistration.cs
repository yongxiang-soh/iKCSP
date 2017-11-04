using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.ManagementReport
{
    public class ManagementReportAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "ManagementReport";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManagementReport_default",
                "ManagementReport/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}