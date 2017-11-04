using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StorageOfExternalPreProduct
{
    public class StorageOfExternalPreProductC3ViewModel
    {
        [Required]
        public string LotNo { get; set; }

        [Required]
        public string PreProductCode { get; set; }
        public string Kndcmdno { get; set; }
    }
}