using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using KCSG.Core.Models;

namespace KCSG.Core.Helper
{
    public static class KCSGHelper
    {
        public static GetPagingRequest GetDataOfPagingRequest(HttpRequestBase request)
        {
            var draw = request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = request.Form.GetValues("start").FirstOrDefault();
            var length = request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = request.Form.GetValues("columns[" + request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var keyWord = request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            var model = new GetPagingRequest()
            {
                Draw = draw,
                Start = start,
                Length = length,
                SortColumn = sortColumn,
                SortColumnDir = sortColumnDir,
                Keyword = keyWord
            };

            return model;
        }
    }
}
