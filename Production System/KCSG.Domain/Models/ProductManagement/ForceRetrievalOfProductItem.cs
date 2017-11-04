using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class ForceRetrievalOfProductItem:TX40_PdtShfStk
    {
        public string ProductName { get; set; }

        //public string Flag { get { return Enum.GetName(typeof(Constants.F40_CertificationFlag), Convert.ToInt32(this.F40_CertificationFlg)); } }


        public string Flag
        {
            get
            {
                //return
                //    EnumsHelper.GetDescription<Constants.F40_CertificationFlag>(
                //        ConvertHelper.ToInteger(this.F40_CertificationFlg));
                return
                    EnumsHelper.GetDescription<Constants.F40_CertificationFlag>(
                        ConvertHelper.ToInteger(this.CerfFlag));
            }
        }
        public string Quantity { get { return F40_AssignAmount.ToString(); } }

        public string LotNo { get; set; }
        public double Amount { get; set; }
        public string ShelfNo1 { get; set; }

        public string ProductLotNo { get; set; }
        public string PreProdLotNo { get; set; }
        public Nullable<System.DateTime> CerfDate { get; set; }
        public string CerfFlag { get; set; }
    }
}
