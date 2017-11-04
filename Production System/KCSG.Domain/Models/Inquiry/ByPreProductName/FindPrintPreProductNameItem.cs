using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByPreProductName
{
    public class FindPrintPreProductNameItem
    {
        public string PreProductCode { get; set; }

        public string OutsidePreProductCode { get; set; }
        public string RBL { get; set; }

        public string PreProductName { get; set; }

        public string PalletNo { get; set; }
        public string PalletClass { get; set; }
        public string SupplierCode { get; set; }
        public string LotNo { get; set; }
        public string CF { get; set; }

        public string ContainerCode { get; set; }
        public string ContainerType { get; set; }
        public string ContainerNo { get; set; }
        public string PreProductLotNo { get; set; }
        public DateTime? SDate1 { get; set; }

        public string SDate { get; set; }
        public string Stime { get; set; }
        public string OutsidePreProductLotNo { get; set; }
        public string Status { get; set; }

        public string ShelfNo { get; set; }
        public double Quantity { get; set; }
        public string Quantityst { get; set; }
        public int? Quantityint { get; set; }
        public string LoadQuantity { get; set; }
        public double Amount { get; set; }

        public string AmountString
        {
            get { return String.Format("{0:#,##0.00}", Amount); }
        }
        public double Total { get; set; }
    }
}
