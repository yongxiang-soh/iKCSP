using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace KCSG.Core.CustomControls
{
    public static class ExtDropDownList
    {
        public static MvcHtmlString ExtDropDownListFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> selectList,
            string optionLabel = null, object htmlAttributes = null)
        {
            var html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                htmlHelper.DropDownListFor(expression, selectList, optionLabel, html).ToString(), expression);
        }
    }
}