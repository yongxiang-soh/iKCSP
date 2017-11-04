namespace KCSG.Domain.Models.MaterialManagement
{
    /// <summary>
    /// Contains inforamation obtained from retrieval material searching.
    /// </summary>
    public class FindRetrievalMaterialResult
    {
        
            public string PalletNo { get; set; }

            public string MaterialCode { get; set; }

            public string ShelfRow { get; set; }

            public string ShelfBay { get; set; }

            public string ShelfLevel { get; set; }

            public string MaterialStatus { get; set; }
        
    }
}