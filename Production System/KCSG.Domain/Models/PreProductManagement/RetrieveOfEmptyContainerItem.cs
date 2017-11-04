using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class RetrieveOfEmptyContainerItem
    {
        public string ContainerNo { get; set; }
        public string ContainerCode { get; set; }
        public string ContainerType { get; set; }
        public string ShelfRow { get; set; }
        public string ShelfBay { get; set; }
        public string ShelfLevel { get; set; }
    }
}
