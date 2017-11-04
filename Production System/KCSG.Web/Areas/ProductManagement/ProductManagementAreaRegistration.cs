using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.ProductManagement
{
    public class ProductManagementAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "ProductManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ProductManagement_default",
                "ProductManagement/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}