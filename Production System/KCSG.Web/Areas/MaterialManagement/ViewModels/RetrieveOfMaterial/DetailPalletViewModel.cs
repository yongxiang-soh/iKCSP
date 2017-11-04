using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial
{
    public class DetailPalletViewModel
    {
        public string ShelfNo { get; set; }
        public string PalletNo { get; set; }
        public double PalletTotal { get; set; }
        public string LotNo1 { get; set; }
        public string LotNo2 { get; set; }
        public string LotNo3 { get; set; }
        public string LotNo4 { get; set; }
        public string LotNo5 { get; set; }
        public string Quantity1 { get; set; }
        public string Quantity2 { get; set; }
        public string Quantity3 { get; set; }
        public string Quantity4 { get; set; }
        public string Quantity5 { get; set; }

        public Grid Grid { get; set; }
    }
}