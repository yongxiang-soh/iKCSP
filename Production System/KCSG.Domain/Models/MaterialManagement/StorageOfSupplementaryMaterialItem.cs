using KCSG.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class StorageOfSupplementaryMaterialItem : TM15_SubMaterial
    {
        public double AddQuantity { get; set; }
        public double PackQuantity { get; set; }
        public double Fraction { get; set; }
        public double InventoryQuantity { get; set; }
        public string Comment { get; set; }

        public bool IsStore { get; set; }
    }
}
