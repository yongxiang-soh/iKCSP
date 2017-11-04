using System.Web;
using System.Web.Mvc;
using KCSG.Web.Attributes;

namespace KCSG.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MvcExceptionFilter());
            filters.Add(new MvcActionRequestFilter());
        }
    }
}
