using KCSG.Data.DataModel;

namespace KCSG.Domain.Models
{
    public class ThirdCommunicationResponse : TX47_PdtWhsCmd
    {
        /// <summary>
        /// Product which has been affected.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Old status.
        /// </summary>
        public string OldStatus { get; set; }

        /// <summary>
        /// Warehouse item.
        /// </summary>
        public object ProductWarehouseItem { get; set; }
    }
}