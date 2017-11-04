using System;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtTextBoxNumber
    {
        public static MvcHtmlString ExtTextBoxNumberFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
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
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(format))
            {
                stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, format, html).ToString());
            }
            else
            {
                stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
            }

            var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
            stringBuilder.AppendLine("<script>$(function(){");
            stringBuilder.AppendLine("$('#" + controlId + "').keypress(function(event) {");
            // stringBuilder.AppendLine("var charCode = (event.which) ? event.which : event.keyCode;");
            stringBuilder.AppendLine("if (event.which != 8 && event.which != 0 && (event.which < 48 || event.which > 57)) {");
            stringBuilder.AppendLine("return false;");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("else {");
            stringBuilder.AppendLine("return true;");
            stringBuilder.AppendLine("};");
            stringBuilder.AppendLine("});");
            stringBuilder.AppendLine("});</script>");
            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                            stringBuilder.ToString(), expression);
            
        }
    }
}