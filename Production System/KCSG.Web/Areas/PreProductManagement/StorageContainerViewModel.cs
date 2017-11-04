using System.ComponentModel;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.PreProductManagement
{
    /// <summary>
    /// Contains information which are submitted from Mark retrieved Container is in used (TCIP023F)
    /// </summary>
    public class StorageContainerViewModel
    {
        [DisplayName("Quantity")]
        public double Quantity { get; set; }

        [DisplayName("Container Code")]
        public string ContainerCode { get; set; }
        
        public Constants.ContainerMode Mode { get; set; }
    }
}