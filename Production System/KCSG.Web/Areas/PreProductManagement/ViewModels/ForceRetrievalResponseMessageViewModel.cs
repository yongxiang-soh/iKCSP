namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class ForceRetrievalResponseMessageViewModel
    {
        public string PreProductCode { get; set; }

        public string ContainerCode { get; set; }

        public string CommandNo { get; set; }

        public string CommandLotNo { get; set; }

        public string ShelfNo { get; set; }
        public bool isNotCommand { get; set; }
    }
}