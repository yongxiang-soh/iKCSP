using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialPostReceptionInput
{
    public class MaterialPostReceptionInputViewModel
    {
        [Display(Name = @"Material Code")]
        public string F33_MaterialCode { get; set; }
        [Display(Name = @"Material Name")]
        public string F01_MaterialDsp { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Partial Delivery")]
        public string PartialDelivery { get; set; }
        [Display(Name = @"Shelf No.")]
        public string ShelfNo { get; set; }
        [Display(Name = @"Pallet No.")]
        public string F33_PalletNo { get; set; }

        public string F31_StorageDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"P.O.No.")]
        public string P_O_No { get; set; }

        public string F32_PalletNo { get; set; }
        public string F32_PrcOrdNo { get; set; }
        public string F32_PrtDvrNo { get; set; }
        public string F32_MegaMsrMacSndEndFlg { get; set; }
        public string F32_GnrlMsrMacSndEndFlg { get; set; }
        public DateTime? F32_StorageDate { get; set; }
        public DateTime? F32_ReStorageDate { get; set; }
        public DateTime? F32_RetrievalDate { get; set; }

       // [Required]
        [MaxLength(16)]
        public string LotNo1 { get; set; }
       // [Required]
         [MaxLength(16)]
        public string LotNo2 { get; set; }
        //[Required]
        [MaxLength(16)]
        public string LotNo3 { get; set; }
      //  [Required]
         [MaxLength(16)]
        public string LotNo4 { get; set; }
       // [Required]
         [MaxLength(16)]
        public string LotNo5 { get; set; }
       //[Required]
        public double? Quantity1 { get; set; }
       // [Required]
        public double? Quantity2 { get; set; }
      //  [Required]
        public double? Quantity3 { get; set; }
       // [Required]
        public double? Quantity4 { get; set; }
       // [Required]
        public double? Quantity5 { get; set; }
        public Grid Grid { get; set; }
    }
}