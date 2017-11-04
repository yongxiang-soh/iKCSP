using System.Collections.Generic;

namespace KCSG.Domain.Models
{
    public class AreaInformation
    {
        #region Properties

        /// <summary>
        /// Area name.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// List of screens
        /// </summary>
        public List<ScreenInformation> Screens { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initiate area with blank screens list.
        /// </summary>
        public AreaInformation()
        {
            Screens = new List<ScreenInformation>();
        }

        #endregion
    }
}