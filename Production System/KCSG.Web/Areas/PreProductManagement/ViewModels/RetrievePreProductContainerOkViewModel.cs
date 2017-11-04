using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class RetrievePreProductContainerOkViewModel
    {
        [Required]
        public string Row { get; set; }

        [Required]
        public string Bay { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string ContainerCode { get; set; }

        [Required]
        public string ContainerNo { get; set; }
        
        public int ContainerType { get; set; }
        
    }
}