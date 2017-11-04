using System;
using System.Web.Mvc;

namespace KCSG.Core.CustomControls
{
    public class ExtModal : IDisposable
    {
        private const string HtmlBeginModal =
            "<div id=\"{0}\" class=\"modal fade ie-modal\" role=\"dialog\">"
            + "<div class=\"modal-dialog {1}\">"
            + "<div class=\"modal-content\">"
            + "<div class=\"modal-header\">"
            + "<button type=\"button\" class=\"close\" data-dismiss=\"modal\">&times;</button>"
            + "<h4 class=\"modal-title\">{2}</h4>"
            + "</div>";

        private const string HtmlEndModal =
            "</div>"
            + "</div>"
            + "</div>";

        private readonly ViewContext _viewContext;

        public ExtModal(ViewContext viewContext, string modalId, ModalSize size, string title)
        {
            string modalSizeClass;
            switch (size)
            {
                case ModalSize.Small:
                    modalSizeClass = "modal-sm";
                    break;
                case ModalSize.Medium:
                    modalSizeClass = "modal-md";
                    break;
                case ModalSize.Large:
                    modalSizeClass = "modal-lg";
                    break;
                default:
                    modalSizeClass = "modal-md";
                    break;
            }

            var htmlBegin = string.Format(HtmlBeginModal, modalId, modalSizeClass, title);
            viewContext.Writer.Write(htmlBegin);
            _viewContext = viewContext;
        }

        public void Dispose()
        {
            _viewContext.Writer.Write(HtmlEndModal);
            GC.SuppressFinalize(this);
        }

        public enum ModalSize
        {
            Small,
            Medium,
            Large
        }
    }

    public class ExtModalBody : IDisposable
    {
        private readonly ViewContext _viewContext;

        public ExtModalBody(ViewContext viewContext, string customCssClass = null)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("modal-body");
            if (!string.IsNullOrEmpty(customCssClass))
            {
                tagBuilder.AddCssClass(customCssClass);
            }

            viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            _viewContext = viewContext;
        }

        public void Dispose()
        {
            _viewContext.Writer.Write("</div>");
            GC.SuppressFinalize(this);
        }
    }

    public class ExtModalFooter : IDisposable
    {
        private readonly ViewContext _viewContext;

        public ExtModalFooter(ViewContext viewContext, string customCssClass = null)
        {
            var tagBuilder = new TagBuilder("div");
            tagBuilder.AddCssClass("modal-footer");
            if (!string.IsNullOrEmpty(customCssClass))
            {
                tagBuilder.AddCssClass(customCssClass);
            }

            viewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
            _viewContext = viewContext;
        }

        public void Dispose()
        {
            _viewContext.Writer.Write("</div>");
            GC.SuppressFinalize(this);
        }
    }

    public static class ExtModalHelper
    {
        public static ExtModal ExtModalFor(this HtmlHelper htmlHelper, string modalId, ExtModal.ModalSize size,
            string title)
        {
            var viewContext = htmlHelper.ViewContext;
            return new ExtModal(viewContext, modalId, size, title);
        }

        public static ExtModalBody ExtModalBodyFor(this HtmlHelper htmlHelper, string customCssClass = null)
        {
            var viewContext = htmlHelper.ViewContext;
            return new ExtModalBody(viewContext, customCssClass);
        }

        public static ExtModalFooter ExtModalFooterFor(this HtmlHelper htmlHelper, string customCssClass = null)
        {
            var viewContext = htmlHelper.ViewContext;
            return new ExtModalFooter(viewContext, customCssClass);
        }
    }
}