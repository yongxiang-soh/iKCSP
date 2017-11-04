using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.EndUser
{
    public class EndUserItem : TM10_EndUsr
    {
        public bool IsCreate { get; set; }
    }
}
