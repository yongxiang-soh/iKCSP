using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Helper;

namespace KCSG.Web.Attributes
{
    public class IntModelBinder:DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
             var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            try
            {
                return ConvertHelper.ToInteger(valueProviderResult.AttemptedValue.Trim(','));
            }
            catch (Exception ex)
            {
                return Convert.ToInt32(0);
            }
           
        }
    }
}