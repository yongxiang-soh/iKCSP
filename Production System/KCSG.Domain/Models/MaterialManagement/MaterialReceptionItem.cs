using System;
using System.Web.Mvc.Html;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class MaterialReceptionItem : TX30_Reception
    {
        public bool IsCreate { get; set; }
        public string Name { get; set; }
        public string AcceptClass { get
        {
            return EnumsHelper.GetEnumDescription((Constants.TX30_Reception) Int32.Parse(this.F30_AcceptClass));
        } }
    }
}
