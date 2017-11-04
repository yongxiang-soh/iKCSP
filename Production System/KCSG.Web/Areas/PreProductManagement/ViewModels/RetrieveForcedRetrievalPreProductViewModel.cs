using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System.Web.Mvc;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class RetrieveForcedRetrievalPreProductViewModel
    {
        public Grid Grid { get; set; }
        public Constants.GroupName GroupName { get; set; }
        public List<SelectListItem> GroupNameList { get; set; } 
    }
}