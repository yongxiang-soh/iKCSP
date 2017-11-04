using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.ProductManagement
{
    public class StoreProductItem
    {
        public Constants.StorageOfProductStatus StorageOfProductStatus { get; set; }

        public string PalletNo { get; set; }

        public bool OutOfSpec { get; set; }

        public string ProductCode1 { get; set; }

        public string ProductCode2 { get; set; }

        public string ProductCode3 { get; set; }

        public string ProductCode4 { get; set; }

        public string ProductCode5 { get; set; }

        public string ProductName1 { get; set; }

        public string ProductName2 { get; set; }

        public string ProductName3 { get; set; }

        public string ProductName4 { get; set; }

        public string ProductName5 { get; set; }

        public string PreProductLotNo1 { get; set; }

        public string PreProductLotNo2 { get; set; }

        public string PreProductLotNo3 { get; set; }

        public string PreProductLotNo4 { get; set; }

        public string PreProductLotNo5 { get; set; }

        public string LotNo1 { get; set; }

        public string LotNo2 { get; set; }

        public string LotNo3 { get; set; }

        public string LotNo4 { get; set; }

        public string LotNo5 { get; set; }

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

        public string CommandNo1 { get; set; }

        public string CommandNo2 { get; set; }

        public string CommandNo3 { get; set; }

        public string CommandNo4 { get; set; }

        public string CommandNo5 { get; set; }
        public double PackUnit1 { get; set; }
        public double PackUnit2 { get; set; }
        public double PackUnit3 { get; set; }
        public double PackUnit4 { get; set; }
        public double PackUnit5 { get; set; }

        public DateTime TabletingEndDate1 { get; set; }
        public DateTime TabletingEndDate2 { get; set; }
        public DateTime TabletingEndDate3 { get; set; }
        public DateTime TabletingEndDate4 { get; set; }
        public DateTime TabletingEndDate5 { get; set; }
       
    }

   
}
