using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class ProductMasterManagementItem : Te85_Env_Prod
    {
        public string ProductName { get; set; }

        public string Location { get; set; }

        public string NewProductName { get; set; }

        public double? USLMean { get; set; }

        public double? UCLMean { get; set; }

        public double? LSLMean { get; set; }

        public double? LCLMean { get; set; }

        public double? USLRange { get; set; }

        public double? LSLRange { get; set; }

        public int? NoOFLot { get; set; }

    }
}
