using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class SubmitContainerStorageViewModel
    {
        #region Properties
        
        public bool IsChecked { get; set; }

        [Required]
        public string PreProductCode { get; set; }

        [Required]
        public string ContainerCode { get; set; }

        #endregion
    }
}