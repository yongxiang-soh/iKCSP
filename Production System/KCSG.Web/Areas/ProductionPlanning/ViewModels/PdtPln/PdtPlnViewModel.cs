using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln
{
    public class PdtPlnViewModel
    {
        [Key,Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = @"Pre-Product Code")]
        [Remote("CheckExistCode", "PdtPln", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "F39_KndEptBgnDate,IsCreate")]
        public string F39_PreProductCode { get; set; }

        [StringLength(16)]
        [Display(Name = @"Pre-Product Name")]
        public string F39_PreProductName     { get; set; }

        public virtual TM03_PreProduct PreProduct { get; set; }

        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Production Date")]
        [Remote("CheckHoliday", "PdtPln", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG29", HttpMethod = "Options", AdditionalFields = "KndLine,IsCreate")]
        public string F39_KndEptBgnDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(1)]
        [Display(Name = @"Line")]
        public string F39_KneadingLine { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(6)]
        [Display(Name = @"Command No")]
        public string F39_KndCmdNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        
        [Display(Name = @"Production Lot Quantity")]
        [StringLength(2)]
        [Range(1, 20, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG28")]
        public string F39_PrePdtLotAmt { get; set; }

       // [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
         [Required]
        [Display(Name = @"Status")]
        public string F39_Status { get; set; }

        [StringLength(1)]
        [Display(Name = @"Color")]
        public string F39_ColorClass { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        [Display(Name = "F39_StartLotNo")]
        public string F39_StartLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "F39_EndLotAmont")]
        public int F39_EndLotAmont { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F39_AddDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F39_UpdateDate { get; set; }
        public int? F39_UpdateCount { get; set; }
        public bool IsCreate { get; set; }
        public int KndLine { get; set; }
    }
}