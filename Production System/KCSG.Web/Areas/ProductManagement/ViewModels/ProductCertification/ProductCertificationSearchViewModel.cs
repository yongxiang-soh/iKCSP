using System;
using System.ComponentModel.DataAnnotations;
using KCSG.Core;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification
{
    public class ProductCertificationSearchViewModel
    {

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DataType(DataType.Date)]
        [Display(Name = @"Certification Date")]
        public string YearMonth { get; set; }

        public Constants.StorageOfProductStatus StorageOfProductStatus { get; set; }
        public Grid GridNormal { get; set; }

        /// <summary>
        /// Grid which is used for displaying pre-products.
        /// </summary>
        public Grid GridOutOfPlan { get; set; }


        public Grid GridSample { get; set; }
    }
}