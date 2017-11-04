using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ReStorageOfMaterial
{
    public class RestorageMaterialViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource))]
        public string MaterialLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource))]
        public double PackQuantity { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource))]
        public double Fraction { get; set; }   

        public double PackUnit { get; set; }
    }
}