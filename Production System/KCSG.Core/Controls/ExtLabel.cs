using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtLabel
    {
        public static MvcHtmlString ExtLabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression,  object htmlAttributes = null)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var property = htmlHelper.ViewData.ModelMetadata.Properties.First(p => p.PropertyName == propertyName);
            var displayName = string.IsNullOrEmpty(property.DisplayName) ? propertyName : property.DisplayName;

            if (expression.IsRequired())
            {
                displayName += "<i class=\"required\">*</i>";
            }

            var tempInput = htmlHelper.LabelFor(expression, "#?replace?#", htmlAttributes).ToString();
            tempInput = tempInput.Replace("#?replace?#", displayName);
            return new MvcHtmlString(tempInput);
        }

        public static MvcHtmlString ExtLabelRequireFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, Boolean IsRequiredStar, object htmlAttributes = null)
        {
            var propertyName = ExpressionHelper.GetExpressionText(expression);
            var property = htmlHelper.ViewData.ModelMetadata.Properties.First(p => p.PropertyName == propertyName);
            var displayName = string.IsNullOrEmpty(property.DisplayName) ? propertyName : property.DisplayName;

            if (expression.IsRequired() && IsRequiredStar)
            {
                displayName += "<i class=\"required\">*</i>";
            }
            else
            {
                displayName += "<i class=\"required\"></i>";
            }

            var tempInput = htmlHelper.LabelFor(expression, "#?replace?#", htmlAttributes).ToString();
            tempInput = tempInput.Replace("#?replace?#", displayName);
            return new MvcHtmlString(tempInput);
        }
    }
}