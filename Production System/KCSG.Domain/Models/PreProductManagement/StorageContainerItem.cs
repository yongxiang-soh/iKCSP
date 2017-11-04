using KCSG.Core.Constants;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class StorageContainerItem
    {
        /// <summary>
        /// Storage quantity.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Container code
        /// </summary>
        public string ContainerCode { get; set; }

        /// <summary>
        /// Mode of storage container.
        /// </summary>
        public Constants.ContainerMode Mode { get; set; }

        public string Conveyor { get; set; }

        public string PreProductCode { get; set; }

        public string Temperature { get; set; }
    }
}