using System.Collections;
using System.Collections.Generic;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class PalletGridDetail : TX31_MtrShfSts
    {
        public string ShelfNo { get { return this.F31_ShelfRow + "-" + this.F31_ShelfBay + "-" + this.F31_ShelfLevel; } }

        public string PalletNo { get; set; }

        public double ShelfTotal { get; set; }

        public string ShelfRow { get; set; }

        public string ShelfBay { get; set; }

        public string ShelfLevel { get; set; }
    }
}