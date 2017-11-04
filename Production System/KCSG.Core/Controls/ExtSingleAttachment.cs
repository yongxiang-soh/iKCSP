using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace KCSG.Core.CustomControls
{
    public static class ExtSingleAttachment
    {
        public static MvcHtmlString ExtSingleAttachmentFor<TModel>(this HtmlHelper<TModel> htmlHelper,
            string id, string callBackFunction = "", object htmlAttributes = null)
        {
            var url = new UrlHelper(HttpContext.Current.Request.RequestContext, RouteTable.Routes);
            //Html Attributes
            var html = htmlAttributes == null
                ? new RouteValueDictionary()
                : new RouteValueDictionary(htmlAttributes);

            //Hidden Input
            var hiddenInputBuilder = htmlHelper.Hidden(id + "-hidden-id");

            //Browse button
            var browseButtonBuilder = new TagBuilder("label");
            browseButtonBuilder.SetInnerText("browse for files");
            browseButtonBuilder.AddCssClass("btn");
            browseButtonBuilder.AddCssClass("browseFilesButton");

            //Inner div
            var innerDivBuilder = new TagBuilder("div");
            innerDivBuilder.AddCssClass("dz-message");
            innerDivBuilder.InnerHtml = "Drag and drop for upload or " + browseButtonBuilder;

            //Dropzone
            var dropzoneBuilder = new TagBuilder("div");
            dropzoneBuilder.AddCssClass("ieDropzone");
            dropzoneBuilder.MergeAttributes(html);
            dropzoneBuilder.MergeAttribute("id", id);     
            dropzoneBuilder.InnerHtml = innerDivBuilder.ToString();

            //Script
            TagBuilder scriptBuilder = new TagBuilder("script");
            var script = "$(\"#" + id + "\").dropzone({";
            script += "url: '" + url.Action("SaveUploadedFile", "RemarkAttachment", new {Area = string.Empty}) + "',";
            script += "uploadmultiple: false,";
            script += "maxFiles: 1,";
            script += "clickable: \"#" + id + " .browseFilesButton\",";
            script += "previewsContainer: false,";
            script += "init: function() {";
            script += "this.on(\"maxfilesexceeded\", function(file) {";
            script += "this.removeAllFiles();";
            script += "this.addFile(file);";
            script += "});";
            script += "this.on(\"complete\", function (file) {";
            script += "if (file.status !== \"error\") {";
            script += "var data = JSON.parse(file.xhr.responseText);";
            script += "if (data !== null && typeof data !== \"undefined\" && data.Message !== \"Error in saving file\") {";
            if (!string.IsNullOrWhiteSpace(callBackFunction))
            {
                script += callBackFunction + "(data.attachment);";
            }
            script += "$(\"#" + id + "-hidden-id\").val(data.attachment.Id)";
            script += "}";
            script += "}";
            script += "});";
            script += "this.on(\"error\", function (files, message, xhr) {";
            script += "if(message !== \"You can not upload any more files.\" ){";
            script += "alert(message);";
            script += "}";
            script += "});";
            script += "}";
            script += "});";
            scriptBuilder.InnerHtml = script;

            //Build the string to return
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(dropzoneBuilder.ToString());
            stringBuilder.AppendLine(scriptBuilder.ToString());
            stringBuilder.AppendLine(hiddenInputBuilder.ToString());

            return new MvcHtmlString(stringBuilder.ToString());
        }
    }
}
