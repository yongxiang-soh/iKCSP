using System.Collections.Generic;
using System.Linq;
using System.Text;
using KCSG.jsGrid.MVC.Enums;

namespace KCSG.jsGrid.MVC
{
    public class Field
    {
        private readonly string _fieldName;

        //internal string ColumnName
        //{
        //    get { return _colName; }
        //}

        public Field(string fieldName)
        {
            _fieldName = fieldName;
        }

        private ColumnType? _columnType;
        //internal ColumnType? ColumnType { get { return _columnType; } }
        /// <summary>
        /// type is a string key of field ("text"|"number"|"checkbox"|"select"|"textarea"|"control") in fields registry jsGrid.fields (the registry can be easily extended with custom field types).
        /// </summary>
        /// <param name="columnType"></param>
        /// <returns></returns>
        public Field SetColumnType(ColumnType columnType)
        {
            _columnType = columnType;
            return this;
        }

        private string _title;
        //internal string Title { get { return _title; } }
        /// <summary>
        /// title is a text to be displayed in the header of the column. If title is not specified, the name will be used instead.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public Field SetTitle(string title)
        {
            _title = title;
            return this;
        }

        private Align? _align;

        /// <summary>
        /// align is alignment of text in the cell. Accepts following values "left"|"center"|"right".
        /// </summary>
        /// <param name="align"></param>
        /// <returns></returns>
        public Field SetAlign(Align align)
        {
            _align = align;
            return this;
        }

        private int? _width;

        /// <summary>
        /// width is a width of the column.
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        public Field SetWidth(int width)
        {
            _width = width;
            return this;
        }

        private int? _widthPercent;

        /// <summary>
        /// width percent is a width percent of the column.
        /// </summary>
        /// <param name="widthPercent"></param>
        /// <returns></returns>
        public Field SetWidthPercent(int widthPercent)
        {
            _widthPercent = widthPercent;
            return this;
        }

        private bool? _visible;

        /// <summary>
        /// visible is a boolean specifying whether to show a column or not. (version added: 1.3).
        /// </summary>
        /// <param name="visible"></param>
        /// <returns></returns>
        public Field SetVisible(bool visible)
        {
            _visible = visible;
            return this;
        }

        private bool? _inserting;

        /// <summary>
        /// inserting is a boolean specifying whether or not column has inserting (insertTemplate() is rendered and insertValue() is included in inserting item).
        /// </summary>
        /// <param name="inserting"></param>
        /// <returns></returns>
        public Field SetInserting(bool inserting)
        {
            _inserting = inserting;
            return this;
        }

        private bool? _sorting;

        /// <summary>
        /// sorting is a boolean specifying whether or not column has sorting ability.
        /// </summary>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public Field SetSorting(bool sorting)
        {
            _sorting = sorting;
            return this;
        }

        private bool? _editing;

        /// <summary>
        /// editing is a boolean specifying whether or not column has editing (editTemplate() is rendered and editValue() is included in editing item).
        /// </summary>
        /// <param name="editing"></param>
        /// <returns></returns>
        public Field SetEditing(bool editing)
        {
            _editing = editing;
            return this;
        }

        private string _itemTemplate;

        /// <summary>
        /// itemTemplate is a function to create cell content. It should return markup as string, DomNode or jQueryElement. The function signature is function(value, item), where value is a value of column property of data item, and item is a row data item.
        /// </summary>
        /// <param name="itemTemplate"></param>
        /// <returns></returns>
        public Field SetItemTemplate(string itemTemplate)
        {
            _itemTemplate = itemTemplate;
            return this;
        }

        private string _formatEditControl;
        /// <summary>
        /// specific format for edit control
        /// </summary>
        /// <param name="formatEditControl"></param>
        /// <returns></returns>
        public Field SetFormatEditControl(string formatEditControl)
        {
            _formatEditControl = formatEditControl;
            return this;
        }

        private string _sorter;
        private bool _íBuiltInSorter;
        /// <summary>
        /// sorter is a string or a function specifying how to sort item by the field. The string is a key of sorting strategy in the registry jsGrid.sortStrategies (the registry can be easily extended with custom sorting functions). Sorting function has the signature function(value1, value2) { return -1|0|1; }.
        /// </summary>
        /// <param name="sorter">is built-in or custom sort function</param>
        /// <param name="isBuiltIn">true if is built-in sorter. Refer jsGrid.MVC.Enums.BuiltInSortingStrategy</param>
        /// <returns></returns>
        public Field SetSorter(string sorter, bool isBuiltIn)
        {
            _sorter = sorter;
            _íBuiltInSorter = isBuiltIn;
            return this;
        }

        private string _insertTemplate;

        /// <summary>
        /// insertTemplate is a function to create insert row cell content. It should return markup as string, DomNode or jQueryElement.
        /// </summary>
        /// <param name="insertTemplate"></param>
        /// <returns></returns>
        public Field SetInsertTemplate(string insertTemplate)
        {
            _insertTemplate = insertTemplate;
            return this;
        }

        private string _editTemplate;

        /// <summary>
        /// insertTemplate is a function to create insert row cell content. It should return markup as string, DomNode or jQueryElement.
        /// </summary>
        /// <param name="editTemplate"></param>
        /// <returns></returns>
        public Field SetEditTemplate(string editTemplate)
        {
            _editTemplate = editTemplate;
            return this;
        }

        private Validator[] _validators;
        /// <summary>
        /// validate is a string as validate rule name or validation function or a validation configuration object or an array of validation configuration objects. Read more details about validation in the Validation section.
        /// </summary>
        /// <param name="validators"></param>
        /// <returns></returns>
        public Field SetValidators(params Validator[] validators)
        {
            _validators = validators;
            return this;
        }

        private FieldEvent[] _fieldEvents;
        /// <summary>
        /// Set event for editting control
        /// </summary>
        /// <param name="fieldEvents"></param>
        /// <returns></returns>
        public Field SetEvents(params FieldEvent[] fieldEvents)
        {
            _fieldEvents = fieldEvents;
            return this;
        }

        private string _suggestionUrl;
        /// <summary>
        /// set url to get suggestion result
        /// </summary>
        /// <param name="suggestionUrl"></param>
        /// <returns></returns>
        public Field SetSuggestionUrl(string suggestionUrl)
        {
            _suggestionUrl = suggestionUrl;
            return this;
        }

        private string _headerTemplate;
        /// <summary>
        /// is a function to create column header content. It should return markup as string, DomNode or jQueryElement.
        /// </summary>
        /// <param name="headerTemplate"></param>
        /// <returns></returns>
        public Field SetHeaderTemplate(string headerTemplate)
        {
            _headerTemplate = headerTemplate;
            return this;
        }

        private string _additionalParam;
        /// <summary>
        /// set additional param for suggestion
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public Field SetAdditionalParam(string functionName)
        {
            _additionalParam = functionName;
            return this;
        }

        private string _onAdd;
        /// <summary>
        /// callback function after select item
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public Field SetOnAdd(string functionName)
        {
            _onAdd = functionName;
            return this;
        }

        private string _onDelete;
        /// <summary>
        /// callback function after delete item
        /// </summary>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public Field SetOnDelete(string functionName)
        {
            _onDelete = functionName;
            return this;
        }

        private IEnumerable<object> _localData;
        /// <summary>
        /// Local Data for suggestion
        /// </summary>
        /// <param name="localData"></param>
        /// <returns></returns>
        public Field SetLocalData(IEnumerable<object> localData)
        {
            _localData = localData;
            return this;
        }

        private NumberOption _numberOption;
        /// <summary>
        /// Set number option for Nunmer Field
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public Field SetNumberOption(NumberOption option)
        {
            _numberOption = option;
            return this;
        }


        private IEnumerable<object> _items;
        private string _valueField;
        private string _textField;
        /// <summary>
        /// set items for select
        /// </summary>
        /// <param name="items">an array of items for select</param>
        /// <param name="valueField">name of property of item to be used as value</param>
        /// <param name="textField">name of property of item to be used as displaying value</param>
        /// <returns></returns>
        public Field SetItems(IEnumerable<object> items, string valueField, string textField)
        {
            _items = items;
            _valueField = valueField;
            _textField = textField;
            return this;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine(string.Format("name: \"{0}\",", this._fieldName));

            if (_columnType.HasValue)
            {
                stringBuilder.AppendLine(string.Format("type: \"{0}\",", this._columnType.Value.ToString().ToLower()));
            }

            if (!string.IsNullOrEmpty(_title))
            {
                stringBuilder.AppendLine(string.Format("title: \"{0}\",", this._title));
            }

            if (_align.HasValue)
            {
                stringBuilder.AppendLine(string.Format("align: \"{0}\",", this._align.Value.ToString().ToLower()));
            }

            if (_width.HasValue)
            {
                stringBuilder.AppendLine(string.Format("width: {0},", this._width.Value));
            }
            else if (_widthPercent.HasValue)
            {
                stringBuilder.AppendLine(string.Format("width: '{0}%',", this._widthPercent.Value));
            }

            if (_visible.HasValue)
            {
                stringBuilder.AppendLine(string.Format("visible: {0},", this._visible.Value.ToString().ToLower()));
            }

            if (_inserting.HasValue)
            {
                stringBuilder.AppendLine(string.Format("inserting: {0},", this._inserting.Value.ToString().ToLower()));
            }

            if (_editing.HasValue)
            {
                stringBuilder.AppendLine(string.Format("editing: {0},", this._editing.Value.ToString().ToLower()));
            }

            if (_sorting.HasValue)
            {
                stringBuilder.AppendLine(string.Format("sorting: {0},", this._sorting.Value.ToString().ToLower()));
            }

            if (_sorting.HasValue)
            {
                stringBuilder.AppendLine(string.Format("sorting: {0},", this._sorting.Value.ToString().ToLower()));
            }

            if (!string.IsNullOrEmpty(_sorter))
            {
                if (_íBuiltInSorter)
                {
                    stringBuilder.AppendLine(string.Format("sorter: \"{0}\",", this._sorter));
                }
                else
                {
                    stringBuilder.AppendLine(string.Format("sorter: {0},", this._sorter));
                }
            }

            if (!string.IsNullOrEmpty(_itemTemplate))
            {
                stringBuilder.AppendLine(string.Format("itemTemplate: {0},", this._itemTemplate));
            }

            if (!string.IsNullOrEmpty(_insertTemplate))
            {
                stringBuilder.AppendLine(string.Format("insertTemplate: {0},", this._insertTemplate));
            }

            if (!string.IsNullOrEmpty(_editTemplate))
            {
                stringBuilder.AppendLine(string.Format("editTemplate: {0},", this._editTemplate));
            }

            if (_validators != null && _validators.Length > 0)
            {
                stringBuilder.AppendLine(string.Format("validate: [{0}],", string.Join("\r\n", _validators.ToList())));
            }

            if (_items != null)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(_items);

                stringBuilder.AppendLine(string.Format("items: {0},", json));
            }

            var numberOptions = new List<string>()
                {
                    "aSep: ''",
                    "aDec: '.'"
                };
            if (_numberOption != null)
            {

                if (_numberOption.Min.HasValue)
                {
                    numberOptions.Add(string.Format("vMin: '{0}'", _numberOption.Min.Value));
                }
                if (_numberOption.Max.HasValue)
                {
                    numberOptions.Add(string.Format("vMax: '{0}'", _numberOption.Max.Value));
                }
                if (!string.IsNullOrEmpty(_numberOption.ASign))
                {
                    numberOptions.Add(string.Format("aSign: '{0}'", _numberOption.ASign));
                }
                if (!string.IsNullOrEmpty(_numberOption.PSign))
                {
                    numberOptions.Add(string.Format("pSign: '{0}'", _numberOption.PSign));
                }
                if (_numberOption.NumberOfDecimal.HasValue)
                {
                    numberOptions.Add(string.Format("mDec : {0}", _numberOption.NumberOfDecimal.Value));
                }
            }

            var numberOptionsStr = string.Join(", ", numberOptions);
            stringBuilder.AppendLine(string.Format("numberOption: {{{0}}},", numberOptionsStr));

            if (!string.IsNullOrEmpty(_valueField))
            {
                stringBuilder.AppendLine(string.Format("valueField: \"{0}\",", _valueField));
            }

            if (!string.IsNullOrEmpty(_textField))
            {
                stringBuilder.AppendLine(string.Format("textField: \"{0}\",", _textField));
            }

            if (!string.IsNullOrEmpty(_suggestionUrl))
            {
                stringBuilder.AppendLine(string.Format("suggestionUrl: \"{0}\",", _suggestionUrl));
            }

            if (!string.IsNullOrEmpty(_additionalParam))
            {
                stringBuilder.AppendLine(string.Format("additionalParam: {0},", _additionalParam));
            }

            if (!string.IsNullOrEmpty(_onAdd))
            {
                stringBuilder.AppendLine(string.Format("onAdd: {0},", _onAdd));
            }

            if (!string.IsNullOrEmpty(_onDelete))
            {
                stringBuilder.AppendLine(string.Format("onDelete: {0},", _onDelete));
            }

            if (!string.IsNullOrEmpty(_formatEditControl))
            {
                stringBuilder.AppendLine(string.Format("formatEditControl: {0},", _formatEditControl));
            }

            if (_fieldEvents != null)
            {
                stringBuilder.AppendLine(string.Format("events: [{0}],", string.Join("\r\n", (object[]) _fieldEvents)));
            }

            if (_localData != null)
            {
                stringBuilder.AppendLine(string.Format("local_data: {0},", Newtonsoft.Json.JsonConvert.SerializeObject(_localData)));
            }
            if (!string.IsNullOrEmpty(_headerTemplate))
            {
                stringBuilder.AppendLine(string.Format("headerTemplate : {0},", _headerTemplate));
            }

            stringBuilder.AppendLine("},");
            return stringBuilder.ToString();
        }
    }
}
