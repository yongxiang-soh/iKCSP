using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtCheckBox
    {
        public static MvcHtmlString ExtCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, bool>> expression, object htmlAttributes = null, string checkboxText = null)
        {
            var labelBuilder = new TagBuilder("label")
            {
                InnerHtml = htmlHelper.CheckBoxFor(expression, htmlAttributes).ToString()
            };
            if (!string.IsNullOrEmpty(checkboxText))
            {
                labelBuilder.InnerHtml += checkboxText;
                labelBuilder.Attributes["style"] = "font-weight: bold, margin-left:10px";
            }
            var divBuilder = new TagBuilder("div");
            divBuilder.AddCssClass("checkbox");
            divBuilder.InnerHtml = labelBuilder.ToString();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(divBuilder);

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
        }

        private static MvcHtmlString ExtCheckBoxListSingleColumnFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, IEnumerable<TValue>>> expression, IEnumerable<SelectListItem> values,
            IEnumerable<TValue> selectedIds, object htmlAttributes = null)
        {
            var isDisabled = false;
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            if (html["disabled"] != null && html["disabled"].ToString() == "disabled")
            {
                isDisabled = true;
            }

            var selectedValues = selectedIds.Select(x => x.ToString()).ToList();
            var variableName = ExpressionHelper.GetExpressionText(expression);
            var stringBuilder = new StringBuilder();

            foreach (var selectListItem in values)
            {
                var itemName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(variableName);
                var itemId = itemName + CustomControlHelper.GetControlCount(itemName);

                var divBuilder = new TagBuilder("div");
                divBuilder.AddCssClass("checkbox");
                if (isDisabled)
                {
                    divBuilder.AddCssClass("disabled");
                }

                var labelBuilder = new TagBuilder("label");
                labelBuilder.MergeAttribute("for", itemId);

                var inputBuilder = new TagBuilder("input");
                if (selectedValues.Contains(selectListItem.Value))
                {
                    inputBuilder.MergeAttribute("checked", "checked");
                }
                if (isDisabled)
                {
                    inputBuilder.MergeAttribute("disabled", "disabled");
                }
                inputBuilder.MergeAttribute("id", itemId);
                inputBuilder.MergeAttribute("name", itemName);
                inputBuilder.MergeAttribute("type", "checkbox");
                inputBuilder.MergeAttribute("value", selectListItem.Value);

                var labelInner = inputBuilder + selectListItem.Text;
                labelBuilder.InnerHtml = labelInner;
                divBuilder.InnerHtml = labelBuilder.ToString();
                stringBuilder.Append(divBuilder);
            }

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
        }

        public static MvcHtmlString ExtCheckBoxListFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, IEnumerable<TValue>>> expression, IEnumerable<SelectListItem> values,
            IEnumerable<TValue> selectedIds, int columnNumber = 1, object htmlAttributes = null)
        {
            if (columnNumber <= 1)
            {
                return htmlHelper.ExtCheckBoxListSingleColumnFor(expression, values, selectedIds, htmlAttributes);
            }

            var isDisabled = false;
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            if (html["disabled"] != null && html["disabled"].ToString() == "disabled")
            {
                isDisabled = true;
            }

            var variableName = ExpressionHelper.GetExpressionText(expression);
            var itemName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(variableName);

            var selectedValues = selectedIds.Select(x => x.ToString()).ToList();
            var selectListItems = values.ToList();
            var rowNumber = selectListItems.Count/columnNumber + 1;

            var tableBuilder = new TagBuilder("table");
            for (var rowIndex = 0; rowIndex < rowNumber; rowIndex++)
            {
                var trBuilder = new TagBuilder("tr");
                for (var columnIndex = 0; columnIndex < columnNumber; columnIndex++)
                {
                    var itemIndex = rowIndex*columnNumber + columnIndex;
                    if (itemIndex >= selectListItems.Count)
                    {
                        break;
                    }

                    var itemId = itemName + CustomControlHelper.GetControlCount(itemName);

                    var inputBuilder = new TagBuilder("input");
                    inputBuilder.MergeAttribute("id", itemId);
                    inputBuilder.MergeAttribute("name", itemName);
                    inputBuilder.MergeAttribute("type", "checkbox");
                    inputBuilder.MergeAttribute("value", selectListItems[itemIndex].Value);
                    if (selectedValues.Contains(selectListItems[itemIndex].Value))
                    {
                        inputBuilder.MergeAttribute("checked", "checked");
                    }
                    if (isDisabled)
                    {
                        inputBuilder.MergeAttribute("disabled", "disabled");
                    }

                    var labelInner = inputBuilder + selectListItems[itemIndex].Text;

                    var labelBuilder = new TagBuilder("label");
                    labelBuilder.MergeAttribute("for", itemId);
                    labelBuilder.InnerHtml = labelInner;

                    var divBuilder = new TagBuilder("div");
                    //divBuilder.AddCssClass("checkbox");
                    if (isDisabled)
                    {
                        divBuilder.AddCssClass("disabled");
                    }
                    divBuilder.InnerHtml = labelBuilder.ToString();

                    var tdBuilder = new TagBuilder("td")
                    {
                        InnerHtml = divBuilder.ToString()
                    };
                    trBuilder.InnerHtml += tdBuilder.ToString();
                }

                tableBuilder.InnerHtml += trBuilder.ToString();
            }

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, tableBuilder.ToString(), expression);
        }
    }
}