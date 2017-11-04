using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    public class ScreenInformation
    {
        /// <summary>
        /// Id of screen.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of screen.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Action of screen.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// Controller which screen belongs to.
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// Properties of screen.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Route value dictionary
        /// </summary>
        public Dictionary<string, object> Routes { get; set; }
    }
}