using System;
using System.ComponentModel.DataAnnotations;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification
{
    public class PrintProductCertificationViewModel
    {
        /// <summary>
        /// Product certification which should be printed.
        /// </summary>
        //public Constants.StorageOfProductStatus Status { get; set; }

        public Constants.PrintProductCertificationStatus PrintProductCertificationStatus { get; set; }

       



        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY}", ApplyFormatInEditMode = false)]
        //[DataType(DataType.Date)]
//[Display(Name = @"")]
        public string StartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY}", ApplyFormatInEditMode = false)]
        //[DataType(DataType.Date)]
        //[Display(Name = @"")]
        public string EndDate { get; set; }

    }
}