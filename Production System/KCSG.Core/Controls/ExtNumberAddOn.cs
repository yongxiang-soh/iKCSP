using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using KCSG.Core.Controls;

namespace KCSG.Core.CustomControls
{
    public static class ExtNumberAddOn
    {
        public static MvcHtmlString ExtNumberAddOnFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,CustomControlConstants.AddOn addOn,String textAddOn = null, NumberOption option = null, object htmlAttributes = null)
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
            stringBuilder.AppendLine("<div class='input-group padding-right-8'>");
            stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
            switch (addOn)
            {
                case CustomControlConstants.AddOn.Text:
                    stringBuilder.AppendLine(" <span class='input-group-addon'>" + textAddOn + "</span>");
                    break;
              
            }
            stringBuilder.AppendLine("</div >");
            var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);

            stringBuilder.AppendLine("<script>$(function(){");
            var options = new List<string>()
            {
                "aSep: ','",
                "aDec: '.'"
            };

            if (option != null)
            {
                if (option.Min.HasValue)
                {
                    options.Add(string.Format("vMin: '{0}'", option.Min.Value));
                }
                if (option.Max.HasValue)
                {
                    options.Add(string.Format("vMax: '{0}'", option.Max.Value));
                }
                else
                {
                    options.Add(string.Format("vMax: '{0}'", 9999999));
                }
                if (!string.IsNullOrEmpty(option.ASign))
                {
                    options.Add(string.Format("aSign: '{0}'", option.ASign));
                }
                if (!string.IsNullOrEmpty(option.PSign))
                {
                    options.Add(string.Format("pSign: '{0}'", option.PSign));
                }
                if (option.NumberOfDecimal.HasValue)
                {
                    options.Add(string.Format("mDec : {0}", option.NumberOfDecimal.Value));
                }
            }
            var optionsStr = string.Join(", ", options);
            stringBuilder.AppendLine(string.Format("$('#{0}').autoNumeric('init', {{{1}}});", controlId, optionsStr));
            stringBuilder.AppendLine("});</script>");
            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                stringBuilder.ToString(), expression);
        }
    }

 
}