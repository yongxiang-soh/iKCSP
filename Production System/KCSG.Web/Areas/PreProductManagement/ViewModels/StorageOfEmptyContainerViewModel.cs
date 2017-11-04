using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Configuration;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class StorageOfEmptyContainerViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Container Type")]
        public string ContainerType { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Container Name")]
        public string ContainerName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Container No.")]
        //[Remote("CheckedExistsTX50", "StorageOfEmptyContainer", HttpMethod = "Options")]
        public int ContainerNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Storage Shelf No.")]
        public string StorageShelfNo { get; set; }
    }
}