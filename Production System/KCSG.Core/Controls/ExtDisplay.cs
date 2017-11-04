using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtDisplay
    {
        public static MvcHtmlString ExtDisplayFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            return htmlHelper.DisplayFor(expression, htmlAttributes);
        }
    }
}