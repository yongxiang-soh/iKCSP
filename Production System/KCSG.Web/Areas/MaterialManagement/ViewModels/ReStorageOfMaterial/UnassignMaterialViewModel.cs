using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ReStorageOfMaterial
{
    public class UnassignMaterialViewModel
    {        
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(3)]
        [Display(Name = @"Pallet No.")]
        public string PalletNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12, ErrorMessage = "Material Code cannot be longer than 12 characters.")]
        [Display(Name = @"Material Code")]
        public string MaterialCode { get; set; }

        [StringLength(16, ErrorMessage = "Material Name cannot be longer than 16 characters.")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Material Name")]
        public string MaterialName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[Required()]
        [StringLength(16, ErrorMessage = @"Material Lot No. (1) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo01 { get; set; }

        //[Required()]
        [StringLength(16, ErrorMessage = @"Material Lot No. (2) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo02 { get; set; }

        //[Required()]
        [StringLength(16, ErrorMessage = @"Material Lot No. (3) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo03 { get; set; }

        //[Required()]
        [StringLength(16, ErrorMessage = @"Material Lot No. (4) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo04 { get; set; }

        //[Required()]
        [StringLength(16, ErrorMessage = @"Material Lot No. (5) cannot be longer than 16 characters.")]
        [Display(Name = @"Material Lot No.")]
        public string MaterialLotNo05 { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Unit (1) is out of range.")]
        public int PackUnit01 { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Unit (2) is out of range.")]
        public int PackUnit02 { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Unit (3) is out of range.")]
        public int PackUnit03 { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Unit (4) is out of range.")]
        public int PackUnit04 { get; set; }

        [Display(Name = @"Pack Unit")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Unit (5) is out of range.")]
        public int PackUnit05 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Qty")]
        //[Range(1, 999, ErrorMessage = @"At least one line must be valid, please input data!")]
        public int PackQuantity01 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Qty")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Qty (2) is out of range.")]
        public int PackQuantity02 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Qty")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Qty (3) is out of range.")]
        public int PackQuantity03 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Qty")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Qty (4) is out of range.")]
        public int PackQuantity04 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Qty")]
        [Range(0, 999, ErrorMessage = @"The inputted value of Pack Qty (5) is out of range.")]
        public int PackQuantity05 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Fraction")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Fraction (1) is out of range.")]
        public int Fraction01 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Fraction")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Fraction (2) is out of range.")]
        public int Fraction02 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Fraction")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Fraction (3) is out of range.")]
        public int Fraction03 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Fraction")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Fraction (4) is out of range.")]
        public int Fraction04 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Fraction")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Fraction (5) is out of range.")]
        public int Fraction05 { get; set; }

        [Display(Name = @"Total")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Total (1) is out of range.")]
        public int Total01 { get; set; }

        [Display(Name = @"Total")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Total (2) is out of range.")]
        public int Total02 { get; set; }

        [Display(Name = @"Total")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Total (3) is out of range.")]
        public int Total03 { get; set; }

        [Display(Name = @"Total")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Total (4) is out of range.")]
        public int Total04 { get; set; }

        [Display(Name = @"Total")]
        [Range(0, 999999, ErrorMessage = @"The inputted value of Total (5) is out of range.")]
        public int Total05 { get; set; }

        public string F01_Unit { get; set; }

        [Display(Name = @"Grand Total")]
        //[Range(1, Int32.MaxValue, ErrorMessage = @"Pack quantity must be more than zero!")]
        public int GrandTotal { get; set; }
        public bool ShowInRetrivel { get; set; }
    }
}