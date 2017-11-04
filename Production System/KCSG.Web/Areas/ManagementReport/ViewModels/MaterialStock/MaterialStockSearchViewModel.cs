using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.MaterialStock
{
    public class MaterialStockSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}")]
        

        public string DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Matetial Code")]
        public string MaterialCode { get; set; }

        [StringLength(16)]
        [Display(Name = @"Material Name")]                
        public string MaterialName { get; set; }


        //[StringLength(4)]
        [Display(Name = @"Bailment Class")]
        public string BailmentClass { get; set; }

        //[StringLength(4)]
        [Display(Name = @"Grand Total")]
        public double GrandTotal { get; set; }

        public Constants.PrintOptions PrintOptions { get; set; }


        public Grid Grid { get; set; }
        //public Grid GridSelected { get; set; }
    }
}