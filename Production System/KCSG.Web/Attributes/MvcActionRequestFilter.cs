using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace KCSG.Web.Attributes
{
    public class MvcActionRequestFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        }
    }
}