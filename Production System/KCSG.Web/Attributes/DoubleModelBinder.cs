using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Attributes
{
    public class DoubleModelBinder:DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
             var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            try
            {
               
                return Convert.ToDouble(valueProviderResult.AttemptedValue.Trim(','));
            }
            catch (Exception)
            {
                return Convert.ToDouble(0);
            }
           
        }
    }
}