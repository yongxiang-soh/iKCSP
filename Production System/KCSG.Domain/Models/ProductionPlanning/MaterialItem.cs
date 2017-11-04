using System;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class MaterialItem : TM01_Material
    {
        public bool IsCreate { get; set; }
        public string Bali { get { return EnumsHelper.GetDescription<Constants.Bailment>(ConvertHelper.ToInteger(this.F01_EntrustedClass)); } }

        public string Retrieval
        {
            get
            {
                var resul = Enum.GetName(typeof (Constants.RetrievalLocation), Constants.RetrievalLocation.Both);
                switch (Convert.ToInt32(this.F01_RtrPosCls))
                {
                    case (int)Constants.RetrievalLocation.FourthFloorOnly:
                        resul = "4F";
                        break;
                             case (int)Constants.RetrievalLocation.ThreerdFloorOnly:
                        resul = "3F";
                        break;
                }
                return resul;
            }
        }

        public string Unit { get { return this.F01_Unit == "0" ? "K" : "P"; } }
        public string Comms { get
        {
            return Enum.GetName(typeof (Constants.F52_MsrMacCls), Convert.ToInt32(this.F01_MsrMacSndFlg));
        } }
        public string Liquid { get { return EnumsHelper.GetDescription<Constants.Liquid>( Convert.ToInt32(this.F01_LiquidClass)); } }
    }
}
