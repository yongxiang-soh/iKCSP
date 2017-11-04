using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class ForcedRetrievalOfPreProductItem:TX42_KndCmd
    {
        public string PreProductName { get; set; }
        public string ContainerNo { get; set; }
        public string ShelfRow { get; set; }
        public string F49_ContainerCode { get; set; }
        public string F37_ContainerNo { get; set; }
        public string F37_ShelfNo { get; set; }
    }
}
