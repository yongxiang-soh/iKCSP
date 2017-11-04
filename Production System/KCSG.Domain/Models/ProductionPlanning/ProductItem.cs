using System;
using System.Collections.Generic;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class ProductItem:TM09_Product
    {
        public bool IsCreate { get; set; }
        public List<PckMtrItem> ListPckMtr { get; set; }

        public string TabletSize
        {
            get
            {
                var tableSize = string.IsNullOrEmpty(F09_TabletSize) ? "" : F09_TabletSize + " (mm)";
                var tableSize2 = string.IsNullOrEmpty(F09_TabletSize2) ? "" : F09_TabletSize2 + " (g)";
                return !string.IsNullOrEmpty(tableSize)?tableSize+" X " +tableSize2:tableSize2;
            }
        }

        public string InnerLabelReq
        {
            get
            {
                return
                    EnumsHelper.GetDescription<Constants.InsideLabelClass>(ConvertHelper.ToInteger(F09_InsideLabelClass));
            }
        }
        public string Temperature
        {
            get
            {
                return
                    Enum.GetName(typeof(Constants.Temperature),ConvertHelper.ToInteger(F09_LowTmpCls));
            }
        }
    }
}
