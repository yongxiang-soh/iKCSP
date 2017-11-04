using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialReceptionInput
{
    public class MaterialReceptionViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"P.O.No.")]
        [StringLength(15)]
       // [Remote("CheckExistPrcOrdNoCode", "MaterialReceptionInput", ErrorMessage = @"The same record exists in database.", HttpMethod = "POST", AdditionalFields = "IsCreate")]
        public string F30_PrcOrdNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Partial Delivery")]
        [StringLength(2)]
       // [Remote("CheckExistPrcOrdNoCode", "MaterialReceptionInput", ErrorMessage = @"The same record exists in database.", HttpMethod = "POST", AdditionalFields = "IsCreate")]
        public string F30_PrtDvrNo { get; set; }

        [Display(Name = @"Material Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string F30_MaterialCode { get; set; }

        [Display(Name = @"Delivery Quantity")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? F30_ExpectAmount { get; set; }

        [Display(Name = @"Delivery Date")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string F30_ExpectDate { get; set; }

        [Display(Name = @"Delivered Quantity")]
        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double F30_StoragedAmount { get; set; }

        //[StringLength(1)]
        [Display(Name = @"Accept Class")]
       // [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string F30_AcceptClass { get; set; }

        [Display(Name = @"Accepted Class")]
        public string AcceptClass { get; set;  }

        [Display(Name=@"Add Date")]
        public DateTime F30_AddDate { get; set; }

        [Display(Name=@"Update Date")]
        public DateTime F30_UpdateDate { get; set; }

        [Display(Name=@"Update Count")]
        
        public int F30_UpdateCount { get; set; }



       [Display(Name = @"Material Name")]
        public string Name { get; set; }

        public bool IsCreate { get; set; }
    }
}