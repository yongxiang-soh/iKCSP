using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class StorageOfMaterialItem
    {
        public string PalletNo { get; set; }

        public string F30_PrcOrdNo { get; set; }
        public string F30_PrtDvrNo { get; set; }

        public string F01_MaterialCode { get; set; }

        public string F01_MaterialDsp { get; set; }

        public string MaterialLotNo01 { get; set; }

        public string MaterialLotNo02 { get; set; }
        public string MaterialLotNo03 { get; set; }

        public string MaterialLotNo04 { get; set; }

        public string MaterialLotNo05 { get; set; }

        public int PackUnit01 { get; set; }

        public int PackUnit02 { get; set; }

        public int PackUnit03 { get; set; }
        public int PackUnit04 { get; set; }

        public int PackUnit05 { get; set; }

        public int PackQuantity01 { get; set; }

        public int PackQuantity02 { get; set; }

        public int PackQuantity03 { get; set; }

        public int PackQuantity04 { get; set; }

        public int PackQuantity05 { get; set; }
        public int Fraction01 { get; set; }

        public int Fraction02 { get; set; }

        public int Fraction03 { get; set; }

        public int Fraction04 { get; set; }

        public int Fraction05 { get; set; }

        public int Total01 { get; set; }

        public int Total02 { get; set; }

        public int Total03 { get; set; }
        public int Total04 { get; set; }
        public int Total05 { get; set; }

        public int GrandTotal { get; set; }


        public string UnitFlag { get; set; }
    }
}
