using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtAutoSuggestion
    {
        public static MvcHtmlString ExtAutoSuggestionFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, IEnumerable<TValue>>> expression, string controller, string action, string controlId,
            object htmlAttributes = null)
        {
            #region Attributes            

            var html = htmlAttributes == null
               ? new RouteValueDictionary()
               : new RouteValueDictionary(htmlAttributes);

            // htmlAttributes: extValue
            var extValue = string.Empty;
            if (html["extValue"] != null)
            {
                extValue = html["extValue"].ToString();
            }

            // htmlAttributes: extText
            var extText = string.Empty;
            if (html["extText"] != null)
            {
                extText = html["extText"].ToString();
            }

            // htmlAttributes: extDisabled
            var extDisabled = string.Empty;           
            if (html["extDisabled"] != null && html["extDisabled"].ToString() == "yes")
            {
                extDisabled = "disabled";
            }

            // htmlAttributes: extClass
            var extClass = string.Empty;
            if (html["extClass"] != null)
            {
                extClass = html["extClass"].ToString();
            }

            // htmlAttributes: extAjaxMethod
            string extAjaxMethod = "GET";
            if (html["extAjaxMethod"] != null)
            {
                extAjaxMethod = html["extAjaxMethod"].ToString();
            }

            // htmlAttributes: extBeforeSendRequestFunc
            string extBeforeSendRequestFunc = "extBeforeSendRequestFunc";
            if (html["extBeforeSendRequestFunc"] != null)
            {
                extBeforeSendRequestFunc = html["extBeforeSendRequestFunc"].ToString();
            }

            // htmlAttributes: extSelectedCallbackFunc
            string extSelectedCallbackFunc = "extSelectedCallbackFunc";
            if (html["extSelectedCallbackFunc"] != null)
            {
                extSelectedCallbackFunc = html["extSelectedCallbackFunc"].ToString();
            }

            // htmlAttributes: extChangedCallbackFunc
            string extChangedCallbackFunc = "extChangedCallbackFunc";
            if (html["extChangedCallbackFunc"] != null)
            {
                extChangedCallbackFunc = html["extChangedCallbackFunc"].ToString();
            }

            // htmlAttributes: extPlaceholder
            string extPlaceholder = string.Empty;
            if (html["extPlaceholder"] != null)
            {
                extPlaceholder = html["extPlaceholder"].ToString();
            }

            // htmlAttributes: extDelay
            string extDelay = "100";
            if (html["extDelay"] != null)
            {
                extDelay = html["extDelay"].ToString();
            }

            // htmlAttributes: extMinLength
            string extMinLength = "1";
            if (html["extMinLength"] != null)
            {
                extMinLength = html["extMinLength"].ToString();
            }

            // htmlAttributes: extShowNoResultFound
            bool extShowNoResultFound = false;
            if (html["extShowNoResultFound"] != null && html["extShowNoResultFound"].ToString() == "yes")
            {
                extShowNoResultFound = true;
            }            

            // Field Name
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var extName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);            

            #endregion

            #region Html Control

            var htmlControl = string.Format("<div id='{0}' class='ext-auto-suggestion'>" +
                                            "\n\t<input type='hidden' class='ext-auto-suggestion-value' name='{1}' value='{2}' />" + // Hidden store selected value
                                            "\n\t<input type='text' class='form-control ext-auto-suggestion-text' value='{5}' placeholder='{3}' {4} />" // Textbox showing suggestion
                                            , controlId, extName, extValue, extPlaceholder, extDisabled, extText);
            if (extShowNoResultFound)
            {

                // htmlAttributes: extNotFoundText
                string extNotFoundText = "No results found!";
                if (html["extNotFoundText"] != null)
                {
                    extNotFoundText = html["extNotFoundText"].ToString();
                }

                // htmlAttributes: extNotFoundButtonText
                string extNotFoundButtonText = "Create New";
                if (html["extNotFoundButtonText"] != null)
                {
                    extNotFoundButtonText = html["extNotFoundButtonText"].ToString();
                }

                // htmlAttributes: extNotFoundButtonClickFunc
                string extNotFoundButtonClickFunc = string.Empty;
                if (html["extNotFoundButtonClickFunc"] != null)
                {
                    extNotFoundButtonClickFunc = html["extNotFoundButtonClickFunc"].ToString() + "();";
                }

                htmlControl += string.Format("\n\t<div class='ext-auto-suggestion-empty'>" +
                                             "\n\t\t<span class='ext-auto-suggestion-empty-text'>{0}</span>" +
                                             "\n\t\t<button class='btn btn-teal ext-auto-suggestion-empty-btn' onclick='{2}'>{1}</button>" +
                                             "\n\t</div>", extNotFoundText, extNotFoundButtonText, extNotFoundButtonClickFunc); // end .ext-auto-suggestion-empty                                         
            }

            htmlControl += "\n</div>"; // end .ext-auto-suggestion

            #endregion

            #region Script 

            var url = string.Format("/{0}/{1}", controller, action);
            htmlControl += "\n<script type='text/javascript'>" +
                              "\n\t $('#" + controlId + " .ext-auto-suggestion-text').autocomplete({" +
                              "\n\t\t highlightClass: 'bold-text'," +
                              "\n\t\t minLength: " + extMinLength + "," +
                              "\n\t\t delay: " + extDelay + "," +
                              "\n\t\t source: function (request, response) {" +
                              "\n\t\t\t var url = '" + url + "';" +
                              "\n\t\t\t var dataRequest = {searchText : request.term};" +
                              "\n\t\t\t $.ajax({" +
                              "\n\t\t\t\t async: false," +
                              "\n\t\t\t\t url: url," +
                              "\n\t\t\t\t type: '" + extAjaxMethod + "'," +
                              "\n\t\t\t\t data: dataRequest," +
                              "\n\t\t\t\t dataType: 'json'," +
                              "\n\t\t\t\t beforeSend: function (xhr) {if (typeof (beforeSendRequestFunc) === 'function') beforeSendRequestFunc(xhr);}," +
                              "\n\t\t\t\t success: function (result, status, xhr) {" +
                              "\n\t\t\t\t\t var suggestion = [];" +
                              "\n\t\t\t\t\t $.each(result, function(index, item){" +
                              "\n\t\t\t\t\t\t suggestion.push( {" +
                              "\n\t\t\t\t\t\t\t label: item.Text," +
                              "\n\t\t\t\t\t\t\t value: item.Value" +
                              "\n\t\t\t\t\t\t });" +
                              "\n\t\t\t\t\t }); " +
                              "\n\t\t\t\t\t response(suggestion);" +
                              "\n\t\t\t\t\t if (typeof (successCallbackFunc) === 'function') successCallbackFunc(result);" +
                              "\n\t\t\t\t }," +
                              "\n\t\t\t\t error: function (xhr, status, error) {" +
                              "\n\t\t\t\t\t toastr.warning('Error occurred: ' + xhr.status + error);" +
                              "\n\t\t\t\t\t if (typeof (errorCallbackFunc) === 'function') errorCallbackFunc(xhr);" +
                              "\n\t\t\t\t } " +
                              "\n\t\t\t });" +
                              "\n\t\t }," +
                              // The response callback
                              "\n\t\t response: function(event, ui) {" +
                              "\n\t\t   if($('#" + controlId + " .ext-auto-suggestion-empty').length > 0){" +
                              "\n\t\t\t     if (ui.content.length === 0) {" +
                              "\n\t\t\t\t      $('#" + controlId + " .ext-auto-suggestion-empty').show();" +
                              "\n\t\t\t     } else {" +
                              "\n\t\t\t\t      $('#" + controlId + " .ext-auto-suggestion-empty').hide();" +
                              "\n\t\t\t     }" +
                              "\n\t\t\t  }" +
                              "\n\t\t }," +
                              // Event triggered before a search is performed, after minLength and delay are met.
                              "\n\t\t search: function (event, ui) {if (typeof (searchCallbackFunc) === 'function') searchCallbackFunc(event, ui);}," +
                              // Event triggered when an item is selected from list.
                              "\n\t\t select: function (event, ui) {" +
                              "\n\t\t\t $('#" + controlId + " .ext-auto-suggestion-value').val(ui.item.value);" +
                              "\n\t\t\t if (typeof (" + extSelectedCallbackFunc + ") === 'function') " + extSelectedCallbackFunc + "(event, ui);}," +
                              // Event triggered when the field is blurred, if the value has changed.
                              "\n\t\t change: function (event, ui) {" +
                              "\n\t\t\t if (ui.item === null) $('#" + controlId + " .ext-auto-suggestion-value').val('');" +
                              "\n\t\t\t if (event.target.value === '' && $('#" + controlId + " .ext-auto-suggestion-empty').length > 0) $('#" + controlId + " .ext-auto-suggestion-empty').hide();" +
                              "\n\t\t\t if (typeof (" + extChangedCallbackFunc + ") === 'function') " + extChangedCallbackFunc + "(event, ui);}" +
                              "\n\t });" +
                              "\n</script>";
            #endregion

            return new MvcHtmlString(htmlControl.ToString());
        }
    }
}
