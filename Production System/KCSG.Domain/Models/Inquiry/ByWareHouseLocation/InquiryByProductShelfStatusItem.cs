using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class InquiryByProductShelfStatusItem
    {
        public string ShelfNo { get; set; }
        public string ShelfStatus { get; set; }
        public string PalletNo { get; set; }
        public DateTime StorageDate { get; set; }
        public string ProductClassification { get; set; }
        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string ProductCode3 { get; set; }
        public string ProductCode4 { get; set; }
        public string ProductCode5 { get; set; }

        public string PreProductLotNo1 { get; set; }
        public string PreProductLotNo2 { get; set; }
        public string PreProductLotNo3 { get; set; }
        public string PreProductLotNo4 { get; set; }
        public string PreProductLotNo5 { get; set; }

        public string ProductLotNo1 { get; set; }
        public string ProductLotNo2 { get; set; }
        public string ProductLotNo3 { get; set; }
        public string ProductLotNo4 { get; set; }
        public string ProductLotNo5 { get; set; }

        public int PackQty1 { get; set; }
        public int PackQty2 { get; set; }
        public int PackQty3 { get; set; }
        public int PackQty4 { get; set; }
        public int PackQty5 { get; set; }

        public double Fraction1 { get; set; }
        public double Fraction2 { get; set; }
        public double Fraction3 { get; set; }
        public double Fraction4 { get; set; }
        public double Fraction5 { get; set; }

        public double Quantity1 { get; set; }
        public double Quantity2 { get; set; }
        public double Quantity3 { get; set; }
        public double Quantity4 { get; set; }
        public double Quantity5 { get; set; }

        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
    }
}
