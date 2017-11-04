using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class TempTableItem:TX51_PdtShfSts
    {
        public string ShelfNo { get { return F51_ShelfRow+'-'+F51_ShelfBay+'-'+F51_ShelfLevel;} }

        public string Status
        {
            get
            {
                return EnumsHelper.GetEnumDescription(
                        (Constants.TX51SheflStatus) Int32.Parse(this.F51_ShelfStatus)); 
            }
        }
        public double RemainingAmount { get; set; }
    }
}
