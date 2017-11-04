using System;
using System.Linq.Expressions;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    internal static class HtmlAttributeHelper
    {
        internal static RouteValueDictionary AddDefaultClass(object htmlAttributes)
        {
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            if (html["class"] != null)
            {
                html["class"] = html["class"] + " form-control";
            }
            else
            {
                html.Remove("class");
                html.Add("class", "form-control");
            }

            return html;
        }

        internal static RouteValueDictionary AddTextAreaStyle(RouteValueDictionary html)
        {
            if (html["style"] != null)
            {
                html["style"] = "min-height: 100px;" + html["style"];
            }
            else
            {
                html.Remove("style");
                html.Add("style", "min-height: 100px;");
            }

            return html;
        }
     
        internal static RouteValueDictionary AddMaxLength(RouteValueDictionary html, int maxLength)
        {
            if (html == null)
            {
                html = new RouteValueDictionary();
            }

            html.Remove("maxlength");
            html.Add("maxlength", maxLength);

            html.Remove("onkeypress");
            html.Add("onkeypress", "setMaxLength($(this));");

            return html;
        }		

        internal static string GetControlIdFromExpression<TModel, TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var expressionStr = expression.Body.ToString();
            var controlId = expressionStr.Substring(expressionStr.IndexOf('.') + 1).Replace('.', '_');
            return controlId;
        }
    }
}