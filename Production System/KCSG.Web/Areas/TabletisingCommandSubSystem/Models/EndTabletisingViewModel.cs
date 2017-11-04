using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Models
{
    public class EndTabletisingViewModel
    {
        [DisplayName("Product Code")]
        public string ProductCode { get; set; }

         [DisplayName("Product Name")]
        public string ProductName { get; set; }
          [DisplayName("Lot No.")]
        public string LotNo { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG36")]
        public int? Package { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG53")]
        public double? Fraction { get; set; }
        public string Fractionst { get; set; }

        public double PackingUnit { get; set; }
        public string PackingUnitst { get; set; }
        public string CommandNo { get; set; }
        public string PreProductLotNo { get; set; }
    }
}