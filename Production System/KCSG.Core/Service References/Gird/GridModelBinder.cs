using System;
using System.Web;
using System.Web.Mvc;
using KCSG.jsGrid.MVC.Enums;

namespace KCSG.jsGrid.MVC
{
    public class GridModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                HttpRequestBase request = controllerContext.HttpContext.Request;
                return new GridSettings()
                {
                    PageIndex = int.Parse(request["pageIndex"] ?? "1"),
                    PageSize = int.Parse(request["pageSize"] ?? "30"),
                    SortField = (request["sortField"] ?? string.Empty),
                    SortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), (request["sortOrder"] ?? "asc"), true)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
