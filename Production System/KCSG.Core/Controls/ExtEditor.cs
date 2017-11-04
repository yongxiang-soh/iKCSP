using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
	public static class ExtEditor
	{
		public static MvcHtmlString ExtEditorFullFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, IEnumerable<TValue>>> expression, bool readOnly = false, object htmlAttributes = null)
		{
			var html = htmlAttributes == null
							? new RouteValueDictionary()
							: new RouteValueDictionary(htmlAttributes);
			html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
			html = HtmlAttributeHelper.AddTextAreaStyle(html);

			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(htmlHelper.TextAreaFor(expression, html).ToString());


			var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
            stringBuilder.AppendLine("<script>");			
			if (readOnly)
			{

				stringBuilder.AppendLine(string.Format("CKEDITOR.replace('{0}', {{ readOnly: true }})", controlId));
			}
			else
			{
				stringBuilder.AppendLine(string.Format("CKEDITOR.replace('{0}')", controlId));
			}
            stringBuilder.AppendLine("</script>");

			return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
		}


		public static MvcHtmlString ExtEditorBasicFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
					Expression<Func<TModel, IEnumerable<TValue>>> expression, bool readOnly = false, object htmlAttributes = null)
		{
			var html = htmlAttributes == null
							? new RouteValueDictionary()
							: new RouteValueDictionary(htmlAttributes);
			html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
			html = HtmlAttributeHelper.AddTextAreaStyle(html);

			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(htmlHelper.TextAreaFor(expression, html).ToString());


			var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
			stringBuilder.AppendLine("<script>");
			if (readOnly)
			{
				stringBuilder.AppendLine(string.Format("CKEDITOR.replace('{0}', {{ customConfig: 'config-basic.js', readOnly: true }})", controlId));				
			}
			else
			{
				stringBuilder.AppendLine(string.Format("CKEDITOR.replace('{0}', {{ customConfig: 'config-basic.js' }})", controlId));
			}
			
			stringBuilder.AppendLine("</script>");

			return CustomControlHelper.GenerateWithValidationMessage(htmlHelper, stringBuilder.ToString(), expression);
		}

	}
}
