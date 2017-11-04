using KCSG.Core.Constants;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class StoragePreProductIntoWarehouseViewModel
    {
        public string LsRow { get; set; }

        public string LsBay { get; set; }

        public string LsLevel { get; set; }

        public string ContainerNo { get; set; }

        public string ContainerCode { get; set; }

        public Constants.KndLine KneadingLine { get; set; }
        
        public string ColorClass { get; set; }
    }
}