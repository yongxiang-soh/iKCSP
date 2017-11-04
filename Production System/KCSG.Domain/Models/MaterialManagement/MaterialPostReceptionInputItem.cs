using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class MaterialPostReceptionInputItem :TX32_MtrShf
    {
        public string F33_MaterialCode { get; set; }
        public string F01_MaterialDsp { get; set; }
        public string PartialDelivery { get; set; }
        public string ShelfNo { get; set; }
        public string F33_PalletNo { get; set; }
        public DateTime? F31_StorageDate { get; set; }
        public string P_O_No { get; set; }
        public string LotNo1 { get; set; }
        public string LotNo2 { get; set; }
        public string LotNo3 { get; set; }
        public string LotNo4 { get; set; }
        public string LotNo5 { get; set; }
        public double Quantity1 { get; set; }
        public double Quantity2 { get; set; }
        public double Quantity3 { get; set; }
        public double Quantity4 { get; set; }
        public double Quantity5 { get; set; }
        public Grid Grid { get; set; }
    }
}
