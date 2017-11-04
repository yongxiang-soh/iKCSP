using System.Web.Mvc;

namespace KCSG.Web.Areas.ProductionPlanning
{
    public class ProductionPlanningAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ProductionPlanning";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ProductionPlanning_default",
                "ProductionPlanning/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}