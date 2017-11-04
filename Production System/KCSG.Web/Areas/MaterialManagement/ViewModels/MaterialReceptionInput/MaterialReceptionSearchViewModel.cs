using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Messaging;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput
{
    public class MaterialReceptionSearchViewModel
    {
        [Display(Name=@"P.O.No.")]
        [MaxLength(15)]
        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string PrcOrdNo { get; set; }

        [Display(Name = @"Partial Delivery")]
        [MaxLength(2)]
       // [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string PartialDelivery { get; set; }

        public Grid GridMaterialReception { get; set; }
    }
}