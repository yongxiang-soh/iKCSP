using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtRadioButton
    {
        public static MvcHtmlString ExtRadioButtonFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, IEnumerable<SelectListItem> values,
            object htmlAttributes = null,  bool isInline = false,int size = 1)
        {
            var isDisabled = false;
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            if (html["disabled"] != null && html["disabled"].ToString() == "disabled")
            {
                isDisabled = true;
            }

            var stringBuilder = new StringBuilder();
            if (isInline)
            {
                foreach (var selectListItem in values)
                {
                    if (selectListItem.Selected)
                    {
                        html.Add("checked", "true");
                    }
                    else
                    {
                        html.Remove("checked");
                    }
                    var input = htmlHelper.RadioButtonFor(expression, selectListItem.Value, html);
                    var label = selectListItem.Text;

                    var divBuilder = new TagBuilder("div");
                    if (isDisabled)
                    {
                        divBuilder.AddCssClass("disabled");
                    }
                   
                    divBuilder.AddCssClass("col-xs-" + size);
                    divBuilder.InnerHtml = input +" " + label;

                    stringBuilder.Append(divBuilder);
                }
            }
            else
            {
                foreach (var selectListItem in values)
                {
                    if (selectListItem.Selected)
                    {
                        html.Add("checked", "true");
                    }
                    else
                    {
                        html.Remove("checked");
                    }
                    var input = htmlHelper.RadioButtonFor(expression, selectListItem.Value, html);
                    var label = selectListItem.Text;
                   
                    var labelBuilder = new TagBuilder("label")
                    {
                        InnerHtml = input + label
                    };

                    var divBuilder = new TagBuilder("div");
                    divBuilder.AddCssClass("radio");
                    if (isDisabled)
                    {
                        divBuilder.AddCssClass("disabled");
                    }

                    divBuilder.InnerHtml = labelBuilder.ToString();

                    stringBuilder.Append(divBuilder);
                }
            }    

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
        }
    }
}
