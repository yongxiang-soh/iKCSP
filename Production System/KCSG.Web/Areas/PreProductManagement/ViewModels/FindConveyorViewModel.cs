using KCSG.Core.Constants;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class FindConveyorViewModel
    {
        /// <summary>
        /// Kneading line.
        /// </summary>
        public Constants.KndLine KneadingLine { get; set; }

        /// <summary>
        /// Colour class.
        /// </summary>
        public Constants.ColorClass Colour { get; set; }
    }
}