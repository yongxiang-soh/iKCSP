using System;
using System.ComponentModel.DataAnnotations;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification
{
    public class AnalyzeProductCertificationViewModel
    {
        /// <summary>
        /// Flag of product certification.
        /// </summary>
        public Constants.F40_CertificationFlag CertificationFlag { get; set; }

        public string status { get; set; }

        /// <summary>
        /// Code of product whose certification will be analyzed.
        /// </summary>
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        public string ProductCode { get; set; }

        /// <summary>
        /// Pre-product lot number.
        /// </summary>
        public string PrePdtLotNo { get; set; }

        /// <summary>
        /// Lot number of product whose certification will be analyzed.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        public string ProductLotNo { get; set; }

        /// <summary>
        /// Quantity of product whose certification will be analyzed.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
     //   [StringLength(10)]
        public double Quantity { get; set; }
        
        /// <summary>
        /// Certification date which is submitted from client.
        /// </summary>

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DataType(DataType.Date)]
        [Display(Name = @"Certification Date")]
        public string CertificationDate { get; set; }

        
        
    }
}