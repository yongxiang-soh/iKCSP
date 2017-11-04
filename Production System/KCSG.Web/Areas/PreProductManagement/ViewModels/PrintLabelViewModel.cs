using System;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class PrintLabelViewModel
    {
        [Required]
        public string CommandNo { get; set; }

        [Required]
        public string PreProductCode { get; set; }

        [Required]
        public string PreProductName { get; set; }

        [Required]
        public string LotNo { get; set; }

        [Required]
        public double Quantity { get; set; }

        [Required]
        public string StorageSeqNo { get; set; }

        [Required]
        public string ContainerCode { get; set; }

        [BindNever]
        public string DateOfStorage
        {
            get { return DateTime.Now.ToString("dd/MM/yyyy"); }
        }

        [BindNever]
        public string DateOfIssued { get { return DateTime.Now.ToString("dd/MM/yyyy"); } }
    }
}