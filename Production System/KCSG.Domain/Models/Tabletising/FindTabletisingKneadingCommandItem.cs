using System.Collections.Generic;

namespace KCSG.Domain.Models.Tabletising
{
    public class FindTabletisingKneadingCommandItem
    {
        /// <summary>
        /// List of filtered kneading commands.
        /// </summary>
        public List<TabletisingKneadingCommandItem> KneadingCommands { get; set; }
        
        /// <summary>
        /// Total items which match with the filtered conditions.
        /// </summary>
        public int Total { get; set; }
    }
}