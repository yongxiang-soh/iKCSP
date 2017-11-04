using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class StorageOfProductItem : TX56_TbtPdt
    {
        public string ProductName { get; set; }
        public double PackingUnit { get; set; }

        public double Total
        {
            get { return F56_TbtCmdEndAmt - F56_StorageAmt; }
        }

        public string F09_ProductDesp { get; set; }

        public string OutofSpecFlag
        {
            get
            {
                return
                    EnumsHelper.GetEnumDescription(
                        (Constants.F56_CertificationFlag) Int32.Parse(this.F56_CertificationFlag));
            }
        }

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

        public double PackQty1 { get; set; }

        public double PackQty2 { get; set; }

        public double PackQty3 { get; set; }

        public double PackQty4 { get; set; }

        public double PackQty5 { get; set; }

        public double Fraction1 { get; set; }

        public double Fraction2 { get; set; }

        public double Fraction3 { get; set; }

        public double Fraction4 { get; set; }

        public double Fraction5 { get; set; }
    }
}