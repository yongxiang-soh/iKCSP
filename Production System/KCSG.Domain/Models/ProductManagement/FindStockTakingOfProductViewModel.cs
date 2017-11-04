using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.ProductManagement
{
    public class FindStockTakingOfProductViewModel
    {
        [Required]
        [StringLength(2)]
        public string ShelfRowFrom { get; set; }

        [Required]
        [StringLength(2)]
        public string ShelfBayFrom { get; set; }

        [Required]
        [StringLength(2)]
        public string ShelfLevelFrom { get; set; }

        [Required]
        [StringLength(2)]
        public string ShelfRowTo { get; set; }

        [Required]
        [StringLength(2)]
        public string ShelfBayTo { get; set; }

        [Required]
        [StringLength(2)]
        public string ShelfLevelTo { get; set; }
    }
}