using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KCSG.Core.Constants;
using KCSG.Domain.Models.KneadingCommand;
using Resources;

namespace KCSG.Web.Areas.KneadingCommand.ViewModels.KneadingStartEnd
{
    public class StartKneadingCommandViewModel
    {
        /// <summary>
        /// List of kneading commands in the system.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public IList<FindKneadingCommandItem> KneadingCommands { get;set; }

        /// <summary>
        /// Kneading line mode.
        /// </summary>
        public Constants.KndLine KneadingLine { get; set; } 
    }
}