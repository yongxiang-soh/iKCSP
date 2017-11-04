using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Packaging;

namespace KCSG.Web.Areas.ProductManagement.ViewModels
{
    public class StorageOfProductViewModel
    {
        [Display(Name = @"Product Code")]
        public string ProductCode1 { get; set; }

        public string ProductCode2 { get; set; }

        public string ProductCode3 { get; set; }

        public string ProductCode4 { get; set; }

        public string ProductCode5 { get; set; }

        [Display(Name = @"Product Name")]
        public string ProductName1 { get; set; }

        public string ProductName2 { get; set; }

        public string ProductName3 { get; set; }

        public string ProductName4 { get; set; }

        public string ProductName5 { get; set; }

        [Display(Name = @"Prepdt LotNo")]
        public string PreProductLotNo1 { get; set; }

        public string PreProductLotNo2 { get; set; }

        public string PreProductLotNo3 { get; set; }

        public string PreProductLotNo4 { get; set; }

        public string PreProductLotNo5 { get; set; }

        [Display(Name = @"Lot No")]
        public string LotNo1 { get; set; }

        public string LotNo2 { get; set; }

        public string LotNo3 { get; set; }

        public string LotNo4 { get; set; }

        public string LotNo5 { get; set; }

        [Display(Name = @"Pack Qty")]
        public double PackQty1 { get; set; }

        public double PackQty2 { get; set; }

        public double PackQty3 { get; set; }

        public double PackQty4 { get; set; }

        public double PackQty5 { get; set; }

        [Display(Name = @"Fraction")]
        public double Fraction1 { get; set; }

        public double Fraction2 { get; set; }

        public double Fraction3 { get; set; }

        public double Fraction4 { get; set; }

        public double Fraction5 { get; set; }

    }
}