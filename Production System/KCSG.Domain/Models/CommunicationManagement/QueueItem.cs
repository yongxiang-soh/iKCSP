using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;

namespace KCSG.Domain.Models
{
    public class QueueItem
    {
        private string _status;
        public string CommandNo { get; set; }
        public string CommandSeqNo { get; set; }
        public string CommandType { get; set; }
        public string StrRtrTypeMaterial { get; set; }
        public string StrRtrTypePreProduct { get; set; }
        public string StrRtrTypeProduct { get; set; }

        public string StrRtrType
        {
            get
            {
                if (!string.IsNullOrEmpty(StrRtrTypeMaterial))
                {
                    return StrRtrTypeMaterial;
                    //  EnumsHelper.GetDescription<Constants.TX34_StrRtrType>(ConvertHelper.ToInteger(StrRtrTypeMaterial));
                }
                if (!string.IsNullOrEmpty(StrRtrTypeProduct))
                {
                    return StrRtrTypeProduct;
                        //EnumsHelper.GetDescription<Constants.F50_StrRtrType>(ConvertHelper.ToInteger(StrRtrTypePreProduct));
                }
                if (!string.IsNullOrEmpty(StrRtrTypePreProduct))
                {
                    return StrRtrTypePreProduct;
                        //EnumsHelper.GetDescription<Constants.F47_StrRtrType>(ConvertHelper.ToInteger(StrRtrTypeProduct));
                }
                return "";

            }
        }

        public string Status
        {
            get;set;}
        public int Priority { get; set; }
        public string PalletNo { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? CommandSendDate { get; set; }
        public DateTime? CommandEndDate { get; set; }
        public string TerminalNo { get; set; }
        public string PictureNo { get; set; }
        public string AbnormalCode { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? RetryCount { get; set; }

    }
}
