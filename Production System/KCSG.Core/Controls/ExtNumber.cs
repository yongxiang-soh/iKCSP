using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtNumber
    {
        public static MvcHtmlString ExtNumberFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, NumberOption option = null, object htmlAttributes = null, bool showmessage = true)
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
            stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
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
            if (showmessage)
            {
                return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                    stringBuilder.ToString(), expression);
            }
            else
            {
                return new MvcHtmlString(stringBuilder.ToString());
            }
        }
    }

    public class NumberOption
    {
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string ASign { get; set; }
        public string PSign { get; set; }
        public int? NumberOfDecimal { get; set; }
    }
}