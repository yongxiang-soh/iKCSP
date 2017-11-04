using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.InterFloorMovementOfProduct
{
    public class InterFloorMovementOfProductViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[MaxLength(1)]
        public int From { get; set; }
        //[MaxLength(1)]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int To { get; set; }
    }
}