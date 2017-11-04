using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtSuggestion
	{
        public static MvcHtmlString ExtSingleSuggestionFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, SingleSuggestionOption option, object htmlAttributes = null)
        {
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
            var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
            stringBuilder.AppendLine("<script>$(function(){");
            stringBuilder.AppendLine(string.Format("$('#{0}').tokenInput('{1}',{{", controlId, option.SearchUrl));
            stringBuilder.AppendLine("theme: 'facebook',");
            stringBuilder.AppendLine("tokenLimit: 1,");
            stringBuilder.AppendLine("method: 'POST',");
            stringBuilder.AppendLine(string.Format("required: {0},", expression.IsRequired().ToString().ToLower()));
            stringBuilder.AppendLine("queryParam: 'query',");
            stringBuilder.AppendLine("tokenValue: 'Id',");
            stringBuilder.AppendLine("propertyToSearch: 'Name',");
            stringBuilder.AppendLine("minChars: 0,");
            if (option.DefaultValue != null)
            {
                stringBuilder.AppendLine(string.Format("prePopulate: [{0}],", Newtonsoft.Json.JsonConvert.SerializeObject(option.DefaultValue)));
            }

            if (!string.IsNullOrEmpty(option.OnAdd))
            {
                stringBuilder.AppendLine(string.Format("onAdd: {0},", option.OnAdd));
            }

            if (!string.IsNullOrEmpty(option.OnDelete))
            {
                stringBuilder.AppendLine(string.Format("onDelete: {0},", option.OnDelete));
            }

            if (!string.IsNullOrEmpty(option.AdditionalParam))
            {
                stringBuilder.AppendLine(string.Format("additionalParam: {0},", option.AdditionalParam));
            }
            if (option.LocalData != null)
            {
                stringBuilder.AppendLine(string.Format("local_data: {0},", Newtonsoft.Json.JsonConvert.SerializeObject(option.LocalData)));
            }
            if (option.CreateNew.HasValue)
            {
                stringBuilder.AppendLine(string.Format("createNew: {0},", option.CreateNew.Value.ToString().ToLower()));
            }

            if (!string.IsNullOrEmpty(option.OnResult))
            {
                stringBuilder.AppendLine(string.Format("onResult: {0},", option.OnResult));
            }

            stringBuilder.AppendLine("});});</script>");
            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                stringBuilder.ToString(), expression);
        }
        public static MvcHtmlString ExtMultiSuggestionFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TValue>> expression, MultiSuggestionOption option, object htmlAttributes = null)
        {

            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
            var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
            stringBuilder.AppendLine("<script>$(function(){");
            stringBuilder.AppendLine(string.Format("$('#{0}').tokenInput('{1}',{{", controlId, option.SearchUrl));
            stringBuilder.AppendLine("theme: 'facebook',");
            if (option.LimitedItem != 0)
            {
                stringBuilder.AppendLine(string.Format("tokenLimit: {0},", option.LimitedItem));
            }
            stringBuilder.AppendLine("method: 'POST',");
            stringBuilder.AppendLine("queryParam: 'query',");
            stringBuilder.AppendLine("tokenValue: 'Id',");
            stringBuilder.AppendLine(string.Format("required: {0},", expression.IsRequired().ToString().ToLower()));
            stringBuilder.AppendLine("propertyToSearch: 'Name',");
            stringBuilder.AppendLine("preventDuplicates: true,");
            stringBuilder.AppendLine("minChars: 0,");
            if (option.DefaultValues != null)
            {
                stringBuilder.AppendLine(string.Format("prePopulate: {0},", Newtonsoft.Json.JsonConvert.SerializeObject(option.DefaultValues)));
            }

            if (!string.IsNullOrEmpty(option.OnAdd))
            {
                stringBuilder.AppendLine(string.Format("onAdd: {0},", option.OnAdd));
            }

            if (!string.IsNullOrEmpty(option.OnDelete))
            {
                stringBuilder.AppendLine(string.Format("onDelete: {0},", option.OnDelete));
            }

            if (!string.IsNullOrEmpty(option.AdditionalParam))
            {
                stringBuilder.AppendLine(string.Format("additionalParam: {0},", option.AdditionalParam));
            }
            if (option.LocalData != null)
            {
                stringBuilder.AppendLine(string.Format("local_data: {0},", Newtonsoft.Json.JsonConvert.SerializeObject(option.LocalData)));
            }
            if (option.CreateNew.HasValue)
            {
                stringBuilder.AppendLine(string.Format("createNew: {0},", option.CreateNew.Value.ToString().ToLower()));
            }
            if (!string.IsNullOrEmpty(option.OnResult))
            {
                stringBuilder.AppendLine(string.Format("onResult: {0},", option.OnResult));
            }

            stringBuilder.AppendLine("});});</script>");
            return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
                stringBuilder.ToString(), expression);
        }
	}

    public class SingleSuggestionOption
    {
        public string SearchUrl { get; set; }
        public IEnumerable<object> LocalData { get; set; }
        public string OnAdd { get; set; }
        public string OnDelete { get; set; }
        public string AdditionalParam { get; set; }
        public string OnResult { get; set; }
        public object DefaultValue { get; set; }
        public bool? CreateNew { get; set; }
    }

    public class MultiSuggestionOption
    {
        public string SearchUrl { get; set; }
        public IEnumerable<object> LocalData { get; set; }
        public string OnAdd { get; set; }
        public string OnDelete { get; set; }
        public string AdditionalParam { get; set; }
        public int LimitedItem { get; set; }
        public IEnumerable<object> DefaultValues { get; set; }
        public bool? CreateNew { get; set; }
        public string OnResult { get; set; }
    }
    public class SuggestionObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
