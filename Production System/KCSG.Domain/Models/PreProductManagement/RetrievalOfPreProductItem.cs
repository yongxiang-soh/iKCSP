using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class RetrievalOfPreProductItem:TX41_TbtCmd
    {
        public string PreProductName { get; set; }
        public int LotSeqNo { get; set; }
        public double ThrowAmount { get; set; }
        public string Line { get; set; }
        public int ContQuanty { get; set; }
    }
}
