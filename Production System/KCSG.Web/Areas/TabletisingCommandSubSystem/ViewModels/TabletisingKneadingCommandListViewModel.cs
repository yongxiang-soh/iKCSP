using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.ViewModels
{
    public class TabletisingKneadingCommandListViewModel
    {
        /// <summary>
        /// List of kneading commands should be displayed on the screen.
        /// </summary>
        public Grid KneadingCommands { get; set; }

        /// <summary>
        /// List of tabletising commands should be displayed on the screen.
        /// </summary>
        public Grid TabletisingCommands { get; set; }

        /// <summary>
        /// Grid which contains product information.
        /// </summary>
        public Grid ProductInformation { get; set; }

        /// <summary>
        /// Get product detail with first char of preProductCode is X
        /// </summary>
        public Grid Detail { get; set; }

        ///// <summary>
        ///// Get product detail with first char of preProductCode not  equal X
        ///// </summary>
        //public Grid Detail2 { get; set; }
    }
}