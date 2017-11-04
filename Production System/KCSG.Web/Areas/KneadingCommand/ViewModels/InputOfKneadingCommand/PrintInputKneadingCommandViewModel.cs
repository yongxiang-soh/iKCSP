using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.KneadingCommand.ViewModels.InputOfKneadingCommand
{
    public class PrintInputKneadingCommandViewModel
    {
        /// <summary>
        /// Pre-Product Code of kneading command.
        /// </summary>
        [Required]
        public string PreProductCode { get; set; }

        /// <summary>
        /// Kneading command no.
        /// </summary>
        public string CommandNo { get; set; }
    }
}