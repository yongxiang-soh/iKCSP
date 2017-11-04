using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByWareHouseLocation
{
    public class RawMaterialShelftStatusItem
    {
        public string ShelfStatus { get; set; }
        public string PalletNo { get; set; }
        public DateTime StorageDate { get; set; }
        public string PrcordNo { get; set; }
        public string PrtdvrNo { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public string AcceptanceClassification { get; set; }
        public string BailmentClassification { get; set; }
        public string MaterialLotNo1 { get; set; }
        public string MaterialLotNo2 { get; set; }
        public string MaterialLotNo3 { get; set; }
        public string MaterialLotNo4 { get; set; }
        public string MaterialLotNo5 { get; set; }
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
