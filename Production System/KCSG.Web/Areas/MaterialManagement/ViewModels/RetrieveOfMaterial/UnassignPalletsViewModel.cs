using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial
{
    public class UnassignPalletsViewModel
    {
        [Required]
        public string ShelfRow { get; set; }

        [Required]
        public string ShelfBay { get; set; }

        [Required]
        public string ShelfLevel { get; set; }
    }
}