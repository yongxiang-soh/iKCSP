using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtMultiSelect
    {
        public static MvcHtmlString ExtMultiSelectFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, IEnumerable<TValue>>> expression, IEnumerable<SelectListItem> values,
            IEnumerable<TValue> selectedIds, string controlId, object htmlAttributes = null)
        {
            var isDisabled = false;
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            if (html["disabled"] != null && html["disabled"].ToString() == "disabled")
            {
                isDisabled = true;
            }

            var selectListItems = values.ToList();

            var leftListBuilder = new TagBuilder("select");
            leftListBuilder.AddCssClass("form-control");
            leftListBuilder.MergeAttribute("name", "leftList" + CustomControlHelper.GetControlCount("leftList"));
            leftListBuilder.MergeAttribute("id", controlId + "_from");
            leftListBuilder.MergeAttribute("size", "8");
            leftListBuilder.MergeAttribute("multiple", "multiple");
            if (isDisabled)
            {
                leftListBuilder.MergeAttribute("disabled", "disabled");
            }

            foreach (var selectListItem in selectListItems)
            {
                var optionBuilder = new TagBuilder("option");
                optionBuilder.MergeAttribute("value", selectListItem.Value);
                optionBuilder.SetInnerText(selectListItem.Text);
                leftListBuilder.InnerHtml += optionBuilder.ToString();
            }

            var leftColumnBuilder = new TagBuilder("div");
            leftColumnBuilder.AddCssClass("col-xs-5");
            leftColumnBuilder.InnerHtml = leftListBuilder.ToString();

            var rightAllIconBuilder = new TagBuilder("i");
            rightAllIconBuilder.AddCssClass("glyphicon");
            rightAllIconBuilder.AddCssClass("glyphicon-forward");

            var rightAllButtonBuilder = new TagBuilder("button");
            rightAllButtonBuilder.MergeAttribute("type", "button");
            rightAllButtonBuilder.MergeAttribute("id", controlId + "_rightAll");
            rightAllButtonBuilder.AddCssClass("btn");
            rightAllButtonBuilder.AddCssClass("btn-block");
            rightAllButtonBuilder.InnerHtml = rightAllIconBuilder.ToString();

            var rightSelectedIconBuilder = new TagBuilder("i");
            rightSelectedIconBuilder.AddCssClass("glyphicon");
            rightSelectedIconBuilder.AddCssClass("glyphicon-chevron-right");

            var rightSelectedButtonBuilder = new TagBuilder("button");
            rightSelectedButtonBuilder.MergeAttribute("type", "button");
            rightSelectedButtonBuilder.MergeAttribute("id", controlId + "_rightSelected");
            rightSelectedButtonBuilder.AddCssClass("btn");
            rightSelectedButtonBuilder.AddCssClass("btn-block");
            rightSelectedButtonBuilder.InnerHtml = rightSelectedIconBuilder.ToString();

            var leftSelectedIconBuilder = new TagBuilder("i");
            leftSelectedIconBuilder.AddCssClass("glyphicon");
            leftSelectedIconBuilder.AddCssClass("glyphicon-chevron-left");

            var leftSelectedButtonBuilder = new TagBuilder("button");
            leftSelectedButtonBuilder.MergeAttribute("type", "button");
            leftSelectedButtonBuilder.MergeAttribute("id", controlId + "_leftSelected");
            leftSelectedButtonBuilder.AddCssClass("btn");
            leftSelectedButtonBuilder.AddCssClass("btn-block");
            leftSelectedButtonBuilder.InnerHtml = leftSelectedIconBuilder.ToString();

            var leftAllIconBuilder = new TagBuilder("i");
            leftAllIconBuilder.AddCssClass("glyphicon");
            leftAllIconBuilder.AddCssClass("glyphicon-backward");

            var leftAllButtonBuilder = new TagBuilder("button");
            leftAllButtonBuilder.MergeAttribute("type", "button");
            leftAllButtonBuilder.MergeAttribute("id", controlId + "_leftAll");
            leftAllButtonBuilder.AddCssClass("btn");
            leftAllButtonBuilder.AddCssClass("btn-block");
            leftAllButtonBuilder.InnerHtml = leftAllIconBuilder.ToString();

            if (isDisabled)
            {
                rightAllButtonBuilder.AddCssClass("disabled");
                rightSelectedButtonBuilder.AddCssClass("disabled");
                leftSelectedButtonBuilder.AddCssClass("disabled");
                leftAllButtonBuilder.AddCssClass("disabled");
                rightAllButtonBuilder.MergeAttribute("disabled", "disabled");
                rightSelectedButtonBuilder.MergeAttribute("disabled", "disabled");
                leftSelectedButtonBuilder.MergeAttribute("disabled", "disabled");
                leftAllButtonBuilder.MergeAttribute("disabled", "disabled");
            }

            var middleColumnBuilder = new TagBuilder("div");
            middleColumnBuilder.AddCssClass("col-xs-2");
            middleColumnBuilder.InnerHtml += rightAllButtonBuilder.ToString();
            middleColumnBuilder.InnerHtml += rightSelectedButtonBuilder.ToString();
            middleColumnBuilder.InnerHtml += leftSelectedButtonBuilder.ToString();
            middleColumnBuilder.InnerHtml += leftAllButtonBuilder.ToString();

            var variableName = ExpressionHelper.GetExpressionText(expression);
            var itemName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(variableName);

            var rightListBuilder = new TagBuilder("select");
            rightListBuilder.AddCssClass("form-control");
            rightListBuilder.MergeAttribute("name", itemName);
            rightListBuilder.MergeAttribute("id", controlId + "_to");
            rightListBuilder.MergeAttribute("size", "8");
            rightListBuilder.MergeAttribute("multiple", "multiple");

            if (isDisabled)
            {
                rightListBuilder.MergeAttribute("disabled", "disabled");
            }

            foreach (var selectedId in selectedIds)
            {
                var selectedItem = selectListItems.FirstOrDefault(x => x.Value == selectedId.ToString());
                if (selectedItem == null)
                {
                    continue;
                }

                var optionBuilder = new TagBuilder("option");
                optionBuilder.MergeAttribute("value", selectedItem.Value);
                optionBuilder.SetInnerText(selectedItem.Text);
                rightListBuilder.InnerHtml += optionBuilder.ToString();
            }

            var rightColumnBuilder = new TagBuilder("div");
            rightColumnBuilder.AddCssClass("col-xs-5");
            rightColumnBuilder.InnerHtml = rightListBuilder.ToString();

            var rowBuilder = new TagBuilder("div");
            rowBuilder.AddCssClass("row");
            rowBuilder.InnerHtml = leftColumnBuilder.ToString();
            rowBuilder.InnerHtml += middleColumnBuilder.ToString();
            rowBuilder.InnerHtml += rightColumnBuilder.ToString();

            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.MergeAttribute("type", "text/javascript");
            scriptBuilder.InnerHtml = "jQuery(document).ready(function($) { $('#" + controlId + "').multiselect(); });";

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(rowBuilder.ToString());
            stringBuilder.AppendLine(scriptBuilder.ToString());

            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
        }
    }
}