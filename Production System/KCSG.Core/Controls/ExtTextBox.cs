using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtTextBox
    {
        public static MvcHtmlString ExtTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression, object htmlAttributes = null, string format = null )
        {
            var html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
            var maxLength = expression.MaxLength();
            if (maxLength != 0)
            {
                html = HtmlAttributeHelper.AddMaxLength(html, maxLength);
            }
            else
            {
                html = HtmlAttributeHelper.AddMaxLength(html, 255);
            }

			if (!string.IsNullOrEmpty(format))
	        {
				return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
								htmlHelper.TextBoxFor(expression, format, html).ToString(), expression);				
	        }

			return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
							htmlHelper.TextBoxFor(expression, html).ToString(), expression);
            
        }
    }
}