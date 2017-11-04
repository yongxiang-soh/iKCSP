namespace KCSG.Domain.Models
{
    public class C2ResponseItem
    {
        #region Properties

        public int InOut { get; set; }

        public string OldStatus { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public int AiStock { get; set; }

        public string StrRtrType { get; set; }

        public string PreProductCode { get; set; }

        public string ContainerCode { get; set; }

        #endregion

        #region Costructors

        /// <summary>
        /// Initiate item without parameters.
        /// </summary>
        public C2ResponseItem()
        {
            
        }

        /// <summary>
        /// Initiate item with parameters.
        /// </summary>
        /// <param name="inOut"></param>
        /// <param name="oldStatus"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="aiStock"></param>
        /// <param name="strRtrType"></param>
        /// <param name="preProductCode"></param>
        /// <param name="containerCode"></param>
        public C2ResponseItem(int inOut, string oldStatus, string from, string to, int aiStock, string strRtrType, string preProductCode, string containerCode)
        {
            InOut = inOut;
            OldStatus = oldStatus;
            From = from;
            To = to;
            AiStock = aiStock;
            StrRtrType = strRtrType;
            PreProductCode = preProductCode;
            ContainerCode = containerCode;
        }

        #endregion
    }
}