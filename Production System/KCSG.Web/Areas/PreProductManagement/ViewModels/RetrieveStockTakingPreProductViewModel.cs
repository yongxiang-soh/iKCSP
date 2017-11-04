using KCSG.Core.Constants;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class RetrieveStockTakingPreProductViewModel
    {
        /// <summary>
        /// Item pre-product
        /// </summary>
        public string PreProductCode { get; set; }

        /// <summary>
        /// Item shelf row.
        /// </summary>
        public string LsRow { get; set; }

        /// <summary>
        /// Item shelf bay.
        /// </summary>
        public string LsBay { get; set; }

        /// <summary>
        /// Item shelf level
        /// </summary>
        public string LsLevel { get; set; }

        /// <summary>
        /// Item container code
        /// </summary>
        public string ContainerCode { get; set; }
    }
}