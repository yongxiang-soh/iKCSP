using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class ProductShippingPlanningItem : TX44_ShippingPlan
    {
        public bool IsCreate { get; set; }

        //public string ProductName { get; set; }

        public string F10_EndUserName { get; set; }

        public string F09_ProductDesp { get; set; }

        public string DeliveryDate { get; set; }
        public string Mode { get; set; }
    }
}
