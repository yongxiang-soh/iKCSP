using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class StorePreProductViewModel
    {
        [Required]
        public string CommandNo { get; set; }

        [Required]
        public string ContainerCode { get; set; }

        [Required]
        public string PreProductLotNo { get; set; }

        [Required]
        public string PreProductCode { get; set; }

        public double Quantity { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string ContainerNo { get; set; }

        [Required]
        public string ContainerType { get; set; }

        [Required]
        public string LotEndFlag { get; set; }

        [Required]
        public string Row { get; set; }

        [Required]
        public string Bay { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string ColorClass { get; set; }
    }
}