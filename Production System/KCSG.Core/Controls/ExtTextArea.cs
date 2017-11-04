using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtTextArea
    {
        public static MvcHtmlString ExtTextAreaFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            var html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
            html = HtmlAttributeHelper.AddTextAreaStyle(html);
            var maxLength = expression.MaxLength();
            if (maxLength != 0)
            {
                html = HtmlAttributeHelper.AddMaxLength(html, maxLength);
            }

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                htmlHelper.TextAreaFor(expression, html).ToString(), expression);
        }
    }
}