using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.Inquiry
{
    public class InquiryAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "Inquiry";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Inquiry_default",
                "Inquiry/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}