using System.Web.ModelBinding;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.KneadingCommand.ViewModels
{
    public class KneadingCommandControlViewModel
    {
        /// <summary>
        /// Kneading line
        /// </summary>
        public Constants.KndLine KneadingLine { get; set; }

        public string KneadingNo { get; set; }
        public string PreProductCode { get; set; }
        public string PreProductName { get; set; }

        /// <summary>
        /// Result grid which is used for display filtering results.
        /// This property shouldn't be bound by client request.
        /// </summary>
        [BindNever]
        public Grid Grid { get; set; }
    }
}