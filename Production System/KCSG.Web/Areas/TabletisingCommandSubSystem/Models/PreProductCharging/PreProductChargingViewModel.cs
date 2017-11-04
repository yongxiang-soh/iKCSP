using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KCSG.Core.Constants;
using Resources;
using System.Web.Mvc;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Models.PreProductCharging
{
    public class PreProductChargingViewModel
    {
        [Display(Name = "Kneading Cmd No.")]
        //[Remote("CheckTX41_TbtCmd", "PreProductCharging", AdditionalFields = "KneadingCmdNo")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [StringLength(6)]
        public string KneadingCmdNo { get; set; }

        [Display(Name = "Pre-Product Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string PreProductCode { get; set; }

        [Display(Name = "Pre-Product Name")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string PreProductName { get; set; }

        [Display(Name = "Pre-Product Lot No.")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        public string PreProductLotNo { get; set; }

        [Display(Name = "Product Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ProductCode { get; set; }

        [Display(Name = "Product Name")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ProductName { get; set; }

        [Display(Name = "Product Lot No.")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ProductLotNo { get; set; }

        [Display(Name = "Container Code 1")]
        [StringLength(12)]
        public string ContainerCode1 { get; set; }

        [Display(Name = "Container Code 2")]
        [Remote("CheckContainerExist", "PreProductCharging", ErrorMessage = "Container does not exist!", AdditionalFields = "ContainerCode2")]
        [System.ComponentModel.DataAnnotations.Compare("ContainerCode1", ErrorMessage = @"Wrong container!")]
        [MinLength(12, ErrorMessage = "Input must be 12 characters!")]
        [MaxLength(12, ErrorMessage = "Input must be 12 characters!")]
        [Required(ErrorMessage = "Input must be 12 characters!")]
        public string ContainerCode2 { get; set; }

        public Constants.LockStatus LockStatus { get; set; }
        public bool IsError { get; set; }
        public string ErrorCode { get; set; }

    }
}