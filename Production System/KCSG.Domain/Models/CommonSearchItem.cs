using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models
{
    public class CommonSearchItem
    {
        public string F09_ProductCode { get; set; }
        public string F09_ProductDesp { get; set; }
        public string F09_TabletType { get; set; }
        public string F09_ValidPeriod { get; set; }

        public string F04_SupplierCode { get; set; }
        public string F04_SupplierName { get; set; }

        public string F01_MaterialCode { get; set; }
        public string F01_MaterialDsp { get; set; }
        public double F01_PackingUnit { get; set; }

        public string F01_RtrPosCls { get; set; }

        public string F01_EntrustedClass { get; set; }

        public string F03_PreproductCode { get; set; }
        public string F03_PreproductName { get; set; }

        public string F15_submaterialcode { get; set; }
        public string F15_materialdsp { get; set; }

        public string F10_EndUserCode { get; set; }
        public string F10_EndUserName { get; set; }

        public string ShippingNo { get; set; }
        public string ProductCode { get; set; }
        public string EndUserName { get; set; }

        public string F40_ProductLotNo { get; set; }

        public string F30_PrcOrdNo { get; set; }
        public string F30_PrtDvrNo { get; set; }

        public string F08_ContainerType { get; set; }
        public string F08_ContainerName { get; set; }

        public string F32_PalletNo { get; set; }

        public string F14_DeviceCode { get; set; }
        public string F14_DeviceName { get; set; }

        public string ShelfNo { get; set; }
        public string F37_ShelfRow { get; set; }
        public string ProductPalletNo { get; set; }
        public string F40_PalletNo { get; set; }
        public string F01_Unit { get; set; }
        public string F01_LiquidClass { get; set; }

        public string F58_PrePdtLotNo { get; set; }
        public string F58_ProductLotNo { get; set; }
        public double F58_TbtCmdEndPackAmt { get; set; }
        public double F58_TbtCmdEndFrtAmt { get; set; }
        public DateTime? F58_TbtEndDate { get; set; }

        public string F33_PalletNo { get; set; }
        public string F33_MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public int F04_MaxLoadAmount { get; set; }

        public string F09_Label { get; set; }

        public string F42_KneadingCommand { get; set; }

        public string F09_TabletSize { get; set; }

        public string F09_TabletSize2 { get; set; }

        public string F56_ProductCode { get; set; }
        public string F56_KndCmdNo { get; set; }
        public string F56_Status { get; set; }
    }

    
}



