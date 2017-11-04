using System;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class SubMaterialItem :TM15_SubMaterial
    {
        public bool IsCreate{ get; set; }
        public string Bail { get { return EnumsHelper.GetDescription<Constants.Bailment>( Convert.ToInt32(this.F15_EntrustedClass)); } }


        //add field for 3.7	Storage of Supplementary Material
        public double Quantity { get; set; }

        public string Comment { get; set; }
    }
}
