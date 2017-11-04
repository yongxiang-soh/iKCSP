using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Models.ManagementOfProductLabel
{
    public class ManagementOfProductLabelViewModel
    {
        [Display(Name=@"Mode")]
        public string Mode { get; set; }
        
        [Display(Name = @"Cmd No.")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [StringLength(6)]
        public string CmdNo { get; set; }

        [Display(Name = @"Production Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        public string ProductionCode { get; set; }

        [Display(Name = @"Production Name")]
        public string ProductionName { get; set; }

        [Display(Name = @"Shelf Life")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [Range(0,99)]
        public int? ShelfLife { get; set; }

        [Display(Name = @"Pieces")]
        [Range(1,99)]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int Pieces { get; set; }

        [Display(Name = @"External Label")]
        public string ExternalLabel { get; set; }

        [Display(Name = @"Code Heading")]
        [StringLength(10)]
        public string CodeHeading { get; set; }

        [Display(Name = @"Internal Label")]
        public string InternalLabel { get; set; }

        [Display(Name = @"SCS Part No.")]
        [StringLength(17)]
        public string ScsPartNo { get; set; }

        [Display(Name = @"Code Label")]
        public string CodeLabel { get; set; }

        [StringLength(5)]
        public string SpecificCodeLabel { get; set; }

        [Display(Name = @"Model")]
        public string InternalModelName { get; set; }

        [Display(Name = @"Model")]
        public string ExternalModelName { get; set; }

        [Display(Name = @"Quantity")]
        public double? Quantity { get; set; }

        [Display(Name=@"Type")]
        [StringLength(15)]
        public string InternalLabelType { get; set; }

        [Display(Name = @"Type")]
        [StringLength(15)]
        public string ExternalLabelType { get; set; }

        [Display(Name = @"C/S No.")]
        public int CsNo1 { get; set; }

        public int CsNo2 { get; set; }

        [Display(Name = @"Lot No.")]
        [StringLength(10)]
        public string InternalLotNo { get; set; }

        [Display(Name = @"Lot No.")]
        [StringLength(10)]
        public string ExternalLotNo { get; set; }

        [Display(Name = @"Mfg-Date")]
        public string MfgDate { get; set; }

        [Display(Name = @"Size")]
        public double? Size1 { get; set; }

        [Display(Name = @"Size")]
        public double? Size2 { get; set; }

        [Display(Name = @"Expired")]
        public string Expired { get; set; }

        [Display(Name = @"Lot No. IDY")]
        public bool LotNoIDY { get; set; }

        [Display(Name = @"Small font")]
        public bool SmallFont { get; set; }

        [Display(Name = @"KAP (Yes/No)")]
        public bool KAP { get; set; }

        [Display(Name = @"Barcode of Expire Date (Yes/No)")]
        public bool BarcodeOfExpireDate { get; set; }

        public bool MainFlow { get; set; }

        [Display(Name = @"Size")]
        public int? InternalSize1 { get; set; }

        [Display(Name = @"Size")]
        public double? InternalSize2 { get; set; }
        
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = @"Expired")]
        public string ExpiredSTLGG { get; set; }

        [Display(Name = @"Expired")]
        public string ExpiredGTBF { get; set; }

        [Display(Name = @"Mfg-Date")]
        public string MfgDateGTBF { get; set; }
    }

}