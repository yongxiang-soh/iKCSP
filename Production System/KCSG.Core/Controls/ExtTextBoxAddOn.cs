using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using KCSG.Core.Controls;

namespace KCSG.Core.CustomControls
{
    public static class ExtTextBoxAddOn
    {
        public static MvcHtmlString ExtTextBoxAddOnFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression,CustomControlConstants.AddOn addOn,string target=null ,object htmlAttributes = null, string format = null )
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
            //html.Add("readonly", true);
            var textBox = "<div class='input-group'>";
            textBox += htmlHelper.TextBoxFor(expression, html).ToString();
            if (addOn == CustomControlConstants.AddOn.Search)
            {
                textBox +=
                    "<span class='input-group-addon' style='cursor: pointer' data-toggle='modal' data-target='"+target+"'><i class='fa fa-search'></i></span>";
            }
            textBox += "</div>";
			if (!string.IsNullOrEmpty(format))
	        {
				return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
								htmlHelper.TextBoxFor(expression, format, html).ToString(), expression);				
	        }

			return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                            textBox, expression);
            
        }

        public static MvcHtmlString ExtTextBoxAddOnForEnable<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, CustomControlConstants.AddOn addOn, string target = null, object htmlAttributes = null, string format = null)
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
            //html.Add("readonly", true);
            var textBox = "<div class='input-group'>";
            textBox += htmlHelper.TextBoxFor(expression, html).ToString();
            if (addOn == CustomControlConstants.AddOn.Search)
            {
                textBox +=
                    "<span class='input-group-addon' style='cursor: pointer' data-toggle='modal' data-target='" + target + "'><i class='fa fa-search'></i></span>";
            }
            textBox += "</div>";
            if (!string.IsNullOrEmpty(format))
            {
                return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                                htmlHelper.TextBoxFor(expression, format, html).ToString(), expression);
            }

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                            textBox, expression);

        }
    }
}