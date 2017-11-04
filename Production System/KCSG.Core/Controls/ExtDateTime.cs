using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using KCSG.Core.CustomControls;

namespace KCSG.Core.Controls
{
   public static class ExtDateTime
    {
       public static MvcHtmlString ExtDateTimeFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
          Expression<Func<TModel, TValue>> expression, DateTimeOption option = null, object htmlAttributes = null)
       {
           var html = HtmlAttributeHelper.AddDefaultClass(htmlAttributes);
          // html.Add("onkeydown", "return false");
           var maxLength = expression.MaxLength();
           if (maxLength != 0)
           {
               html = HtmlAttributeHelper.AddMaxLength(html, maxLength);
           }
           else
           {
               html = HtmlAttributeHelper.AddMaxLength(html, 255);
           }
           var controlId = HtmlAttributeHelper.GetControlIdFromExpression(expression);
           var stringBuilder = new StringBuilder();
           stringBuilder.AppendLine("<div class='input-group date ' id='dv" + controlId + "'>");
           stringBuilder.AppendLine(htmlHelper.TextBoxFor(expression, html).ToString());
           

         
           var options = new List<string>();

           if (option != null)
           {
               if (!string.IsNullOrEmpty(option.ViewMode))
               {
                   options.Add(string.Format("viewMode: '{0}'", option.ViewMode));
               }
               if (!string.IsNullOrEmpty(option.Format))
               {
                   options.Add(string.Format("format: '{0}'", option.Format));
                   if (option.Format =="LT" || option.Format == "HH:mm" || option.Format == "HH:mm:ss")
                   { 
                       stringBuilder.AppendLine("<span class='input-group-addon'><span class='glyphicon glyphicon-time'></span></span>");
                   }
                   else
                   {
                       stringBuilder.AppendLine("<span class='input-group-addon'><span class='glyphicon glyphicon-calendar'></span></span>");
                      
                    }
               }
               if (option.DaysOfWeekDisabled!=null)
               {
                   options.Add(string.Format("daysOfWeekDisabled: {0}", option.DaysOfWeekDisabled));
               }
               if (!string.IsNullOrEmpty(option.MinDate))
               {
                   options.Add(string.Format("minDate: {0}", option.MinDate));
               }
               else { options.Add(string.Format("minDate: {0}", " moment('01/01/1990')")); }
               if (!string.IsNullOrEmpty(option.MaxDate))
               {
                   options.Add(string.Format("maxDate: {0}", option.MaxDate));
               }
               if (option.EnabledHours.HasValue)
               {
                   options.Add(string.Format("enabledHours : '{0}'", option.EnabledHours.Value));
               }
               if (option.ViewDate.HasValue)
               {
                   options.Add(string.Format("viewDate : {0}", option.ViewDate.Value));
               }
               if (option.Inline.HasValue)
               {
                   options.Add(string.Format("inline : {0}", option.Inline.Value));
               }
              // options.Add(string.Format("defaultDate:{0}", DateTime.Now.ToString("d")));
               
           }
           stringBuilder.AppendLine("</div><script>$(function(){");
           var optionsStr = string.Join(", ", options);
           stringBuilder.AppendLine(string.Format("$('#dv{0}').datetimepicker( {{{1}}});", controlId, optionsStr));  
           //stringBuilder.AppendLine(string.Format("$('#{0}').datetimepicker('setDate',{1});", controlId,string.Format( "$('#{0}').val()",controlId)));
           stringBuilder.AppendLine("});</script>");

           if (option != null && option.IsValidationMessageSupported)
               return new MvcHtmlString(stringBuilder.ToString());
           return CustomControlHelper.GenerateWithValidationMessage(htmlHelper,
               stringBuilder.ToString(), expression);

       }
    }

    public class DateTimeOption
    {
        public string ViewMode { get; set; }
        public string Format { get; set; }
        public string DaysOfWeekDisabled { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }
        public bool? EnabledHours { get; set; }
        public bool? ViewDate { get; set; }
        public bool? Inline { get; set; }

        /// <summary>
        /// Whether validation message should be shown automatcally when the input field is not valid or not.
        /// </summary>
        public bool IsValidationMessageSupported { get; set; }
    }
}
