using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using KCSG.jsGrid.MVC.Enums;

namespace KCSG.jsGrid.MVC
{
    public class Grid : IHtmlString
    {
        private string _id;
        public Grid(string id)
        {
            this._id = id;
        }

        private readonly List<Field> _fields = new List<Field>();
        public Grid SetFields(params Field[] field)
        {
            this._fields.AddRange(field);
            return this;
        }

        private IEnumerable<object> _data;
        public Grid SetData<T>(IEnumerable<T> data)
        {
            _data = data as IEnumerable<object>;
            return this;
        }

        private string _sortField;
        private SortOrder? _sortOrder; 
        public Grid SetDefaultSorting(string sortField, SortOrder sortOrder)
        {
            _sortField = sortField;
            _sortOrder = sortOrder;
            return this;
        }

        private bool? _selected;
        public Grid SetSelected(bool selected)
        {
            _selected = selected;
            return this;
        }

        private bool? _autoload;
        /// <summary>
        /// A boolean value specifying whether controller.loadData will be called when grid is rendered.
        /// </summary>
        /// <param name="autoload"></param>
        /// <returns></returns>
        public Grid SetAutoload(bool autoload)
        {
            _autoload = autoload;
            return this;
        }

        private string _onDataLoaded;
        /// <summary>
        /// Fires after data loading.
        /// Has the following arguments:
        ///{
        ///    grid                // grid instance
        ///    data                // load result (array of items or data structure for loading by page scenario) 
        ///}
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public Grid OnDataLoaded(string functionName)
        {
            _onDataLoaded = functionName;
            return this;
        }

        private string _headerRowRenderer;
        /// <summary>
        /// A function to customize grid header row. The function should return markup as a string, jQueryElement or DomNode representing table row tr
        /// </summary>
        /// <param name="_headerRowRenderer"></param>
        /// <returns></returns>
        public Grid HeaderTemplate(string headerRowRenderer)
        {
            _headerRowRenderer = headerRowRenderer;
            return this;
        }
        //private string _controller;
        ///// <summary>
        ///// An object or function returning an object with the following structure:
        ///// - loadData is a function returning an array of data or jQuery promise that will be resolved with an array of data (when pageLoading is true instead of object the structure { data: [items], itemsCount: [total items count] } should be returned). Accepts filter parameter including current filter options and paging parameters when pageLoading is true.
        ///// - insertItem is a function returning inserted item or jQuery promise that will be resolved with inserted item. Accepts inserting item object.
        ///// - updateItem is a function returning updated item or jQuery promise that will be resolved with updated item. Accepts updating item object.
        ///// - deleteItem is a function deleting item. Returns jQuery promise that will be resolved when deletion is completed. Accepts deleting item object.
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <returns></returns>
        //public Grid SetController(string controller)
        //{
        //    _controller = controller;
        //    return this;
        //}

        private string _searchUrl;
        /// <summary>
        /// set search url to search
        /// </summary>
        /// <param name="searchUrl"></param>
        /// <returns></returns>
        public Grid SetSearchUrl(string searchUrl)
        {
            _searchUrl = searchUrl;
            return this;
        }

        private string _width;
        /// <summary>
        /// Specifies the overall width of the grid. Accepts all value types accepting by jQuery.width.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public Grid SetWidth(string width)
        {
            _width = width;
            return this;
        }

        private string _height;
        /// <summary>
        /// Specifies the overall height of the grid including the pager. Accepts all value types accepting by jQuery.height.
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        public Grid SetHeight(string height)
        {
            _height = height;
            return this;
        }

        private bool? _heading;
        /// <summary>
        /// A boolean value specifies whether to show grid header or not.
        /// </summary>
        /// <param name="heading"></param>
        /// <returns></returns>
        public Grid SetHeading(bool heading)
        {
            _heading = heading;
            return this;
        }

        private bool? _sorting;
        /// <summary>
        /// A boolean value specifies whether sorting is allowed.
        /// </summary>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public Grid SetSorting(bool sorting)
        {
            _sorting = sorting;
            return this;
        }

        private bool? _paging;
        /// <summary>
        /// A boolean value specifies whether data is displayed by pages.
        /// </summary>
        /// <param name="paging"></param>
        /// <returns></returns>
        public Grid SetPaging(bool paging)
        {
            _paging = paging;
            return this;
        }

        private bool? _pageLoading;
        /// <summary>
        /// A boolean value specifies whether to load data by page. When pageLoading is true the loadData method of controller accepts filter parameter with two additional properties pageSize and pageIndex.
        /// </summary>
        /// <param name="pageLoading"></param>
        /// <returns></returns>
        public Grid SetPageLoading(bool pageLoading)
        {
            _pageLoading = pageLoading;
            return this;
        }

        private int? _pageSize;
        /// <summary>
        /// An integer value specifying the amount of items on the page. Applied only when paging is true.
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Grid SetPageSize(int pageSize)
        {
            _pageSize = pageSize;
            return this;
        }

        private string _onRefreshed;
        /// <summary>
        /// Fires after grid refresh.
        /// </summary>
        /// <param name="onRefreshed"></param>
        /// <returns></returns>
        public Grid SetOnRefreshed(string onRefreshed)
        {
            _onRefreshed = onRefreshed;
            return this;
        }

        private GridMode? _gridMode;

        /// <summary>
        /// set grid mode
        /// </summary>
        /// <param name="gridMode"></param>
        /// <returns></returns>
        public Grid SetMode(GridMode gridMode)
        {
            _gridMode = gridMode;
            return this;
        }

        public string ToHtmlString()
        {

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("<div id='{0}'></div>", _id));
            stringBuilder.AppendLine("<script type=\"text/javascript\">$(function(){");
            stringBuilder.AppendLine(string.Format("$('#{0}')", _id));
            if (_gridMode == GridMode.Editing)
            {
                stringBuilder.AppendLine(".editGrid({");
            }
            else
            {
                stringBuilder.AppendLine(".jsGrid({");
            }
            stringBuilder.AppendLine("pageButtonCount: 5,");
            stringBuilder.AppendLine("pagerFormat: '{first} {prev} {pages} {next} {last}',");
            if (_fields != null)
            {
                stringBuilder.AppendLine(string.Format("fields: [{0}],", string.Join("\r\n", _fields)));
            }

            if (_data != null)
            {
                stringBuilder.AppendLine(string.Format("data: {0},", Newtonsoft.Json.JsonConvert.SerializeObject(_data)));
            }
            if (_autoload.HasValue)
            {
                stringBuilder.AppendLine(string.Format("autoload: {0},", _autoload.Value.ToString().ToLower()));
            }

            if (_sorting.HasValue)
            {
                stringBuilder.AppendLine(string.Format("sorting: {0},", _sorting.Value.ToString().ToLower()));
            }

            if (_paging.HasValue)
            {
                stringBuilder.AppendLine(string.Format("paging: {0},", _paging.Value.ToString().ToLower()));
            }

            if (_pageLoading.HasValue)
            {
                stringBuilder.AppendLine(string.Format("pageLoading: {0},", _pageLoading.Value.ToString().ToLower()));
            }

            if (_pageSize.HasValue)
            {
                stringBuilder.AppendLine(string.Format("pageSize: {0},", _pageSize.Value));
            }

            if (!string.IsNullOrEmpty(_sortField))
            {
                stringBuilder.AppendLine(string.Format("_sortField: {{ name:'{0}'}},", _sortField));
            }

            if (!string.IsNullOrEmpty(_onDataLoaded))
            {
                stringBuilder.AppendLine(string.Format("onDataLoaded: {0},", _onDataLoaded));
            }

            if (_sortOrder.HasValue)
            {
                stringBuilder.AppendLine(string.Format("_sortOrder: '{0}',", _sortOrder.Value.ToString().ToLower()));
            }
            stringBuilder.AppendLine("rownumbers:true,");
            //if (!string.IsNullOrEmpty(_controller))
            //{
            //    stringBuilder.AppendLine(string.Format("controller: {0},", _controller));
            //}
            if (!string.IsNullOrEmpty(_searchUrl))
            {
                stringBuilder.AppendLine(
                    string.Format("controller: {{ loadData: function(filter) {{return $.ajax({{type: 'POST',url: '{0}',data: filter}});}}}},", _searchUrl));
                //"{0},", _searchUrl));
            }

            if (!string.IsNullOrEmpty(_width))
            {
                stringBuilder.AppendLine(string.Format("width: \"{0}\",", _width));
            }

            if (!string.IsNullOrEmpty(_onRefreshed))
            {
                stringBuilder.AppendLine(string.Format("onRefreshed: {0},", _onRefreshed));
            }

            if (!string.IsNullOrEmpty(_height))
            {
                stringBuilder.AppendLine(string.Format("height: \"{0}\",", _height));
            }

            if (_heading.HasValue)
            {
                stringBuilder.AppendLine(string.Format("heading: {0},", _heading.Value.ToString().ToLower()));
            }

            if (_selected.HasValue)
            {
                stringBuilder.AppendLine(string.Format("selected: {0},", _selected.Value.ToString().ToLower()));
            }

            if (!string.IsNullOrEmpty(_headerRowRenderer))
            {
                stringBuilder.AppendLine(string.Format("headerRowRenderer: {0},", _headerRowRenderer));
            }

            stringBuilder.AppendLine("});");
            if (_data != null && _data.Any())
            {
                if (_gridMode == GridMode.Editing)
                {
                    stringBuilder.AppendLine(string.Format("$('#{0}').editGrid('refresh');", _id));
                }
                else
                {
                    stringBuilder.AppendLine(string.Format("$('#{0}').jsGrid('refresh');", _id));
                }

            }
            stringBuilder.AppendLine("});</script>");

            return stringBuilder.ToString();
        }
    }
}
