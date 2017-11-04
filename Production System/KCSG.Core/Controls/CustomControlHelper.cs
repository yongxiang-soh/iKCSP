using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace KCSG.Core.CustomControls
{
    public static class CustomControlHelper
    {
        private static readonly Dictionary<string, int> ControlNameCount = new Dictionary<string, int>();

        internal static int GetControlCount(string controlName)
        {
            if (ControlNameCount.ContainsKey(controlName))
            {
                var count = ControlNameCount[controlName];
                count += 1;
                ControlNameCount[controlName] = count;
                return count;
            }

            ControlNameCount.Add(controlName, 1);
            return 1;
        }

        internal static MvcHtmlString GenerateWithValidationMessage<TModel, TValue>(HtmlHelper<TModel> htmlHelper,
            string control, Expression<Func<TModel, TValue>> expression)
        {
            var validationMessage = htmlHelper.ValidationMessageFor(expression);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(control);
            stringBuilder.AppendLine(validationMessage.ToString());

            return new MvcHtmlString(stringBuilder.ToString());
        }
    }
}