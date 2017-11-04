(function (window, $, undefined) {

    var EDITGRID = "EditGrid",
        EDITGRID_DATA_KEY = EDITGRID,
        EDITGRID_ROW_DATA_KEY = "EditGridItem",
        EDITGRID_CELL_EDITOR_KEY = "EditGridCellEditor";



    var defaultController = {
        loadData: $.noop
    };
    var getOrApply = function (value, context) {
        if ($.isFunction(value)) {
            return value.apply(context, $.makeArray(arguments).slice(2));
        }
        return value;
    };

    function Grid(element, config) {
        var $element = $(element);

        $element.data(EDITGRID_DATA_KEY, this);

        this._container = $element;

        this.data = [];
        this.fields = [];

        this._init(config);
        this._lastRowIndex = this.data.length;
        this.render();
    }

    Grid.prototype = {
        width: "auto",
        loadIndicationDelay: 500,
        loadMessage: "Please, wait...",
        loadShading: true,
        heading: true,
        containerClass: "editGrid",
        gridHeaderClass: "jsgrid-grid-header",
        tableClass: "jsgrid-table",
        gridBodyClass: "jsgrid-grid-body",
        headerRowClass: "jsgrid-header-row",
        oddRowClass: "jsgrid-row",
        evenRowClass: "jsgrid-alt-row",
        editControlClass: "editgrid-edit-control-",
        editFieldClass: "editgrid-edit-field-",
        invalidClass: "jsgrid-invalid",
        noDataContent: "There is no item in this view.",
        noDataRowClass: "jsgrid-nodata-row",
        autoload: false,
        _init: function (config) {
            $.extend(this, config);
            this._initLoadStrategy();
            this._initController();
            this._initFields();
        },

        _initController: function () {
            this._controller = $.extend({}, defaultController, getOrApply(this.controller, this));
        },

        _refreshHeight: function () {
            var container = this._container,
                //pagerContainer = this._pagerContainer,
                height = this.height,
                nonBodyHeight;

            container.height(height);

            if (height !== "auto") {
                height = container.height();

                nonBodyHeight = this._header.outerHeight(true);
                //if (pagerContainer.parents(container).length) {
                //    nonBodyHeight += pagerContainer.outerHeight(true);
                //}

                this._body.outerHeight(height - nonBodyHeight);
            }
        },


        _refreshWidth: function () {
            var $headerGrid = this._headerGrid,
                $bodyGrid = this._bodyGrid,
                width = this.width;
            if (width === "auto") {
                $headerGrid.width("auto");
                width = $headerGrid.outerWidth();
            }
            $headerGrid.width("");
            $bodyGrid.width("");
            this._container.width(width);
            width = $headerGrid.outerWidth();
            $bodyGrid.width(width);
        },
        _initFields: function () {
            var self = this;
            self.fields = $.map(self.fields, function (field) {
                if ($.isPlainObject(field)) {
                    var fieldConstructor = editGrid.Field;
                    var fieldEditor = new fieldConstructor(field);
                    fieldEditor._field = field;
                    field = fieldEditor;
                }
                field._grid = self;
                return field;
            });
        },

        loadStrategy: function () {
            return new editGrid.loadStrategies.DirectLoadingStrategy(this);
        },
        _initLoadStrategy: function () {
            this._loadStrategy = getOrApply(this.loadStrategy, this);
        },

        _createBody: function () {
            var $content = this._content = $("<tbody>");

            var $bodyGrid = this._bodyGrid = $("<table>").addClass(this.tableClass)
                .append($content);

            var $body = this._body = $("<div>").addClass(this.gridBodyClass)
                .append($bodyGrid)
                .on("scroll", $.proxy(function (e) {
                    this._header.scrollLeft(e.target.scrollLeft);
                }, this));

            return $body;
        },


        validation: function (config) {
            return editGrid.Validation && new editGrid.Validation(config);
        },

        _createValidation: function () {
            return getOrApply(this.validation, this);
        },
        _renderGrid: function () {
            this._clear();

            this._container.addClass(this.containerClass)
                .css("position", "relative")
                .append(this._createHeader())
                .append(this._createBody());

            this._loadIndicator = this._createLoadIndicator();
            this._validation = this._createValidation();
        },
        _eachField: function (callBack) {
            var self = this;
            $.each(this.fields, function (index, field) {
                if (field.visible) {
                    callBack.call(self, field, index);
                }
            });
        },

        _prepareCell: function (cell, field, cssprop) {
            var $cell = $(cell)
                .css("width", field.width)
                .addClass((cssprop && field[cssprop]) || field.css);
            if (cssprop == 'editcss') {
                $cell.addClass(field.type ? (this.editControlClass + field.type) : "");
                $cell.addClass(field.name ? (this.editFieldClass + field.name) : "");
                $cell.addClass(field.align ? ("jsgrid-align-" + field.align) : "");
            }
            return $cell;
        },

        _scrollBarWidth: (function () {
            var result;

            return function () {
                if (result === undefined) {
                    var $ghostContainer = $("<div style='width:50px;height:50px;overflow:hidden;position:absolute;top:-10000px;left:-10000px;'></div>");
                    var $ghostContent = $("<div style='height:100px;'></div>");
                    $ghostContainer.append($ghostContent).appendTo("body");
                    var width = $ghostContent.innerWidth();
                    $ghostContainer.css("overflow-y", "auto");
                    var widthExcludingScrollBar = $ghostContent.innerWidth();
                    $ghostContainer.remove();
                    result = width - widthExcludingScrollBar;
                }
                return result;
            };
        })(),

        _createHeader: function () {
            var $headerRow = this._headerRow = this._createHeaderRow();

            var $headerGrid = this._headerGrid = $("<table>").addClass(this.tableClass)
                .append($headerRow);

            var $header = this._header = $("<div>").addClass(this.gridHeaderClass)
                .addClass(this._scrollBarWidth() ? "jsgrid-header-scrollbar" : "")
                .append($headerGrid);

            return $header;
        },
        _createHeaderRow: function () {
            if ($.isFunction(this.headerRowRenderer))
                return $(this.headerRowRenderer());

            var $result = $("<tr>").addClass(this.headerRowClass);

            this._eachField(function (field, index) {
                this._prepareCell("<th>", field, "headercss")
                    .append(field.headerTemplate ? field.headerTemplate() : "")
                    .appendTo($result);
            });

            return $result;
        },

        loadIndicator: function (config) {
            return new editGrid.LoadIndicator(config);
        },

        _createLoadIndicator: function () {
            return getOrApply(this.loadIndicator, this, {
                message: this.loadMessage,
                shading: this.loadShading,
                container: this._container
            });
        },


        _showLoading: function () {
            clearTimeout(this._loadingTimer);

            this._loadingTimer = setTimeout($.proxy(function () {
                this._loadIndicator.show();
            }, this), this.loadIndicationDelay);
        },

        _hideLoading: function () {
            clearTimeout(this._loadingTimer);
            this._loadIndicator.hide();
        },
        _clear: function () {
            clearTimeout(this._loadingTimer);

            this._pagerContainer && this._pagerContainer.empty();

            this._container.empty()
                .css({ position: "", width: "", height: "" });
        },


        _callEventHandler: function (handler, eventParams) {
            handler.call(this, $.extend(eventParams, {
                grid: this
            }));

            return eventParams;
        },

        _controllerCall: function (method, param, isCanceled, doneCallback) {
            if (isCanceled)
                return $.Deferred().reject().promise();

            this._showLoading();

            var controller = this._controller;
            if (!controller || !controller[method]) {
                throw Error("controller has no method '" + method + "'");
            }

            return $.when(controller[method](param))
                .done($.proxy(doneCallback, this))
                //.fail($.proxy(this._errorHandler, this))
                .always($.proxy(this._hideLoading, this));
        },

        loadData: function (filter) {
            filter = filter || (this.filtering ? this.getFilter() : {});

            $.extend(filter);

            return this._controllerCall("loadData", filter, false, function (loadedData) {
                if (!loadedData)
                    return;
                this._lastRowIndex = loadedData.length;
                this._loadStrategy.finishLoad(loadedData);
            });
        },
        removeRow: function (rowName) {
            var $row = this._container.find("tr[name='" + rowName + "']");
            if ($row) {
                var item = $row.data(EDITGRID_ROW_DATA_KEY);
                var itemIndex = this.data.indexOf(item);
                this.data.splice(itemIndex, 1);
                $row.remove();
            }
        },
        addNewRow: function (item) {
            if (item == undefined) {
                item = {
                };
            }
            var row = this._createRow(item, this._lastRowIndex);
            this._content.append(row);
            this.data.push(item);
            this._lastRowIndex++;
            return row;
        },
        updateRow: function (rowName, item) {
            if (item == undefined) {
                item = {
                };
            }

            var $row = this._container.find("tr[name='" + rowName + "']");
            if ($row) {
                var oldItem = $row.data(EDITGRID_ROW_DATA_KEY);
                var itemIndex = this.data.indexOf(oldItem);
                this.data.splice(itemIndex, 1, item);
                var $newRows = this._createRow(item, itemIndex);
                $row.after($newRows);
                $row.remove();
            }
        },
        setData: function (data) {
            if (data == undefined) {
                data = [];
            }
            this.data = data;
            this._refreshContent();
        },
        _setItemFieldValue: function (item, field, value) {
            var props = field.name.split('.');
            var current = item;
            var prop = props[0];

            while (current && props.length) {
                item = current;
                prop = props.shift();
                current = item[prop];
            }

            if (!current) {
                while (props.length) {
                    item = item[prop] = {};
                    prop = props.shift();
                }
            }

            item[prop] = value;
        },
        _updateItem: function ($row) {
            var item = $row.data(EDITGRID_ROW_DATA_KEY);
            var self = this;
            this._eachFieldEditor($row, function (field, index) {
                if (field.editing) {
                    self._setItemFieldValue(item, field, field.editValue());
                }
            });
            //$cells.each(function(cellIndex, cell) {
            //    var $cell = $(cell);
            //    var field = $cell.data(EDITGRID_CELL_EDITOR_KEY);
            //    if (field.editing && field.visible) {
            //        self._setItemFieldValue(item, field, field.editValue());
            //    }
            //});
            return item;
        },
        _eachFieldEditor: function ($row, callBack) {
            var $cells = $row.find("td");
            var self = this;
            $cells.each(function (index, cell) {
                var $cell = $(cell);
                var field = $cell.data(EDITGRID_CELL_EDITOR_KEY);
                if (field.editing && field.visible) {
                    callBack.call(self, field, index);
                }
            });
        },
        getItemByRowName: function (rowName) {
            var $row = this._container.find("tr[name='" + rowName + "']");
            return this._updateItem($row);
        },
        _validateItem: function (row) {
            var $row = $(row);
            var item = this._updateItem($row);

            var args = {
                item: item,
                row: $row
            };
            var validationErrors = [];
            this._eachFieldEditor($row, function (field, index) {
                if (!field.validate)
                    return;
                var errors = this._validation.validate($.extend({
                    value: this._getItemFieldValue(item, field),
                    rules: field.validate
                }, args));
                this._setCellValidity($row.children().eq(index), errors);
                if (errors.length > 0) {
                    validationErrors.push.apply(validationErrors,
                        $.map(errors, function (message) {
                            return {
                                field: field,
                                message: message,
                                row: $row
                            };
                        }));
                }
            });
            return validationErrors;
        },
        validateAndGetData: function () {
            var validationErrors = [];
            var $rows = this._content.find("tr");
            var self = this;
            $rows.each(function (rowIndex, row) {
                validationErrors.push.apply(validationErrors, self._validateItem(row));
            });
            return {
                data: this.data,
                errors: validationErrors
            }
        },

        _setCellValidity: function ($cell, errors) {
            $cell
                .toggleClass(this.invalidClass, !!errors.length)
                .attr("title", errors.join("\n"));
        },
        render: function () {
            this._renderGrid();
            return this.autoload ? this.loadData() : $.Deferred().resolve().promise();
        },

        _handleOptionChange: function (name, value) {
            this[name] = value;
            ;
            switch (name) {
                case "data":
                    this.refresh();
                    break;
                default:
                    this.render();
                    break;
            }
        },
        refresh: function () {
            this._refreshHeading();
            this._refreshContent();
            this._refreshWidth();
        },
        _createNoDataRow: function () {
            var noDataContent = getOrApply(this.noDataContent, this);

            var amountOfFields = 0;
            this._eachField(function () {
                amountOfFields++;
            });

            return $("<tr>").addClass(this.noDataRowClass)
                .append($("<td>").attr("colspan", amountOfFields).append(noDataContent));
        },
        _refreshContent: function () {
            var $content = this._content;
            $content.empty();

            if (!this.data.length) {
                $content.append(this._createNoDataRow());
                return this;
            }

            for (var itemIndex = 0; itemIndex < this.data.length; itemIndex++) {
                var item = this.data[itemIndex];
                $content.append(this._createRow(item, itemIndex));
            }
        },

        _refreshHeading: function () {
            this._headerRow.toggle(this.heading);
        },
        option: function (key, value) {

            if (arguments.length === 1)
                return this[key];

            var optionChangingEventArgs = {
                option: key,
                oldValue: this[key],
                newValue: value
            };

            this._handleOptionChange(optionChangingEventArgs.option, optionChangingEventArgs.newValue);
        },

        _getItemFieldValue: function (item, field) {
            var props = field.name.split('.');
            var result = item[props.shift()];

            while (result && props.length) {
                result = result[props.shift()];
            }

            return result;
        },

        _getRowClasses: function (item, itemIndex) {
            var classes = [];
            classes.push(((itemIndex + 1) % 2) ? this.oddRowClass : this.evenRowClass);
            classes.push(getOrApply(this.rowClass, this, item, itemIndex));
            return classes.join(" ");
        },

        _createEditRow: function (item, itemIndex) {
            var $result = $("<tr>").addClass(this.editRowClass).attr('name', this._container.attr('id') + '-editgrid-row-' + itemIndex);
            this._eachField(function (field) {
                var orgField = field._field;
                var fieldConstructor = (orgField.type && editGrid.fields[orgField.type]) || editGrid.Field;
                var editField = new fieldConstructor(orgField);
                var fieldValue = this._getItemFieldValue(item, editField);
                var editTemplate = orgField.itemTemplate ? orgField.itemTemplate(fieldValue, item) : (editField.editTemplate ? editField.editTemplate(fieldValue, item) : "");
                var $cell = this._prepareCell("<td>", editField, "editcss");
                $cell.data(EDITGRID_CELL_EDITOR_KEY, editField);

                $cell.append(editTemplate)
                    .appendTo($result);
            });
            $result.data(EDITGRID_ROW_DATA_KEY, item);
            return $result;
        },

        _createRow: function (item, itemIndex) {
            return this._createEditRow(item, itemIndex);
        },
    };

    $.fn.editGrid = function (config) {
        var result = this,
            args = $.makeArray(arguments),
            methodArgs = args.slice(1);

        this.each(function () {
            var $element = $(this),
                instance = $element.data(EDITGRID_DATA_KEY),
                methodResult;

            if (instance) {
                methodResult = instance[config].apply(instance, methodArgs);
                if (methodResult !== undefined && methodResult !== instance) {
                    result = methodResult;
                    return false;
                }
            } else {
                new Grid($element, config);
            }
        });

        return result;
    };


    var fields = {};

    window.editGrid = {
        Grid: Grid,
        fields: fields,
    };
})(window, jQuery);
(function (editGrid, $, undefined) {

    function DirectLoadingStrategy(grid) {
        this._grid = grid;
    }

    DirectLoadingStrategy.prototype = {

        firstDisplayIndex: function () {
            var grid = this._grid;
            return grid.option("paging") ? (grid.option("pageIndex") - 1) * grid.option("pageSize") : 0;
        },

        lastDisplayIndex: function () {
            var grid = this._grid;
            var itemsCount = grid.option("data").length;

            return grid.option("paging")
                ? Math.min(grid.option("pageIndex") * grid.option("pageSize"), itemsCount)
                : itemsCount;
        },

        itemsCount: function () {
            return this._grid.option("data").length;
        },

        openPage: function (index) {
            this._grid.refresh();
        },

        loadParams: function () {
            return {};
        },

        sort: function () {
            this._grid._sortData();
            this._grid.refresh();
            return $.Deferred().resolve().promise();
        },

        finishLoad: function (loadedData) {
            this._grid.option("data", loadedData);
        },

        finishInsert: function (insertedItem) {
            var grid = this._grid;
            grid.option("data").push(insertedItem);
            grid.refresh();
        },

        finishDelete: function (deletedItem, deletedItemIndex) {
            var grid = this._grid;
            grid.option("data").splice(deletedItemIndex, 1);
            grid.reset();
        }
    };
    editGrid.loadStrategies = {
        DirectLoadingStrategy: DirectLoadingStrategy,
    };
}(editGrid, jQuery));



(function (editGrid, $, undefined) {

    function Field(config) {
        $.extend(true, this, config);
    }

    Field.prototype = {
        name: "",
        title: null,
        //css: "",
        align: "",
        width: 100,

        visible: true,
        editing: true,

        headerTemplate: function () {
            return (this.title === undefined || this.title === null) ? this.name : this.title;
        },
        itemTemplate: function (value, item) {
            return value;
        },
        editValue: function () {
            return this.editControl.val();
        },
        setEvents: function() {
            if (this.editControl && this.events) {
                for (var index = 0; index < this.events.length; index++) {
                    this.editControl.on(this.events[index].EventName, this.events[index].CallbackFunction);
                }
            }
        },
        formatControl: function(item) {
            if ($.isFunction(this.formatEditControl)) {
                this.formatEditControl(this.editControl, item);
            }
        }
    };

    editGrid.Field = Field;

}(editGrid, jQuery));


(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function TextField(config) {
        Field.call(this, config);
    }

    TextField.prototype = new Field({
        editTemplate: function (value, item) {
            if (!this.editing) {
                return this.itemTemplate(value);
            }
            var $result = this.editControl = this._createTextBox();
            $result.val(value);
            this.formatControl(item);
            this.setEvents();
            return $result;
        },
        _createTextBox: function () {
            return $("<input>").attr("type", "text")
                    .addClass("form-control")
                .prop("readonly", !!this.readOnly);
        }
    });

    editGrid.fields.text = editGrid.TextField = TextField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var TextField = editGrid.TextField;

    function TextAreaField(config) {
        TextField.call(this, config);
    }

    TextAreaField.prototype = new TextField({

        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);

            var $result = this.editControl = this._createTextArea();
            $result.val(value);
            this.formatControl(item);
            this.setEvents();
            return $result;
        },

        _createTextArea: function () {
            return $("<textarea>")
                    .addClass("form-control");
        }
    });

    editGrid.fields.textarea = editGrid.TextAreaField = TextAreaField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function CheckboxField(config) {
        Field.call(this, config);
    }

    CheckboxField.prototype = new Field({
        align: "center",
        itemTemplate: function (value) {
            return this._createCheckbox().prop({
                checked: value,
                disabled: true
            });
        },
        editValue: function () {
            return this.editControl.prop("checked");
        },
        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);

            var $result = this.editControl = this._createCheckbox();
            $result.prop("checked", value);
            this.formatControl(item);
            this.setEvents();
            return $result;
        },

        _createCheckbox: function () {
            return $("<input>").attr("type", "checkbox");
        }
    });

    editGrid.fields.checkbox = editGrid.CheckboxField = CheckboxField;

}(editGrid, jQuery));


(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function RadioField(config) {
        Field.call(this, config);
    }

    RadioField.prototype = new Field({
        align: "center",
        itemTemplate: function (value) {
            return this._createRadioField().prop({
                checked: value,
                disabled: true
            });
        },
        editValue: function () {
            return this.editControl.prop("checked");
        },
        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);

            var $result = this.editControl = this._createRadioField();
            $result.prop("checked", value);
            $result.attr("name", this.name);
            this.formatControl(item);
            this.setEvents();
            return $result;
        },

        _createRadioField: function () {
            return $("<input>").attr("type", "radio");
        }
    });

    editGrid.fields.radio = editGrid.RadioField = RadioField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var TextField = editGrid.TextField;

    function NumberField(config) {
        TextField.call(this, config);
    }

    NumberField.prototype = new TextField({
        align: "right",
        readOnly: false,

        editValue: function () {
            return this.editControl.autoNumeric('get');
        },
        _createTextBox: function () {
            var $input = $("<input>")
                    .addClass("form-control")
                .prop("readonly", !!this.readOnly);
            $input.autoNumeric('init', this.numberOption);
            return $input;
        }
    });

    editGrid.fields.number = editGrid.NumberField = NumberField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function SelectField(config) {
        this.items = [];
        this.selectedIndex = -1;
        this.valueField = "";
        this.textField = "";
        Field.call(this, config);
    }

    SelectField.prototype = new Field({
        itemTemplate: function (value) {
            var items = this.items,
                valueField = this.valueField,
                textField = this.textField,
                resultItem;

            if (valueField) {
                resultItem = $.grep(items, function (item, index) {
                    return item[valueField] === value;
                })[0] || {};
            }
            else {
                resultItem = items[value];
            }

            var result = (textField ? resultItem[textField] : resultItem);

            return (result === undefined || result === null) ? "" : result;
        },

        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);
            var $result = this.editControl = this._createSelect();
            (value !== undefined) && $result.val(value);
            this.formatControl(item);
            this.setEvents();
            return $result;
        },

        _createSelect: function () {
            var $result = $("<select>").addClass("form-control"),
                valueField = this.valueField,
                textField = this.textField,
                selectedIndex = this.selectedIndex;

            $.each(this.items, function (index, item) {
                var value = valueField ? item[valueField] : index,
                    text = textField ? item[textField] : item;

                var $option = $("<option>")
                    .attr("value", value)
                    .text(text)
                    .appendTo($result);

                $option.prop("selected", (selectedIndex === index));
            });

            return $result;
        }
    });

    editGrid.fields.select = editGrid.SelectField = SelectField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    function LoadIndicator(config) {
        this._init(config);
    }

    LoadIndicator.prototype = {

        container: "body",
        message: "Loading...",
        shading: true,

        zIndex: 1000,
        shaderClass: "jsgrid-load-shader",
        loadPanelClass: "jsgrid-load-panel",

        _init: function (config) {
            $.extend(true, this, config);

            this._initContainer();
            this._initShader();
            this._initLoadPanel();
        },

        _initContainer: function () {
            this._container = $(this.container);
        },

        _initShader: function () {
            if (!this.shading)
                return;

            this._shader = $("<div>").addClass(this.shaderClass)
                .hide()
                .css({
                    position: "absolute",
                    top: 0,
                    right: 0,
                    bottom: 0,
                    left: 0,
                    zIndex: this.zIndex
                })
                .appendTo(this._container);
        },

        _initLoadPanel: function () {
            this._loadPanel = $("<div>").addClass(this.loadPanelClass)
                .text(this.message)
                .hide()
                .css({
                    position: "absolute",
                    top: "50%",
                    left: "50%",
                    zIndex: this.zIndex
                })
                .appendTo(this._container);
        },

        show: function () {
            var $loadPanel = this._loadPanel.show();

            var actualWidth = $loadPanel.outerWidth();
            var actualHeight = $loadPanel.outerHeight();

            $loadPanel.css({
                marginTop: -actualHeight / 2,
                marginLeft: -actualWidth / 2
            });

            this._shader.show();
        },

        hide: function () {
            this._loadPanel.hide();
            this._shader.hide();
        }

    };

    editGrid.LoadIndicator = LoadIndicator;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    function Validation(config) {
        this._init(config);
    }

    Validation.prototype = {

        _init: function (config) {
            $.extend(true, this, config);
        },

        validate: function (args) {
            var errors = [];

            $.each(this._normalizeRules(args.rules), function (_, rule) {
                if (rule.validator(args.value, args.item, rule.param))
                    return;

                var errorMessage = $.isFunction(rule.message) ? rule.message(args.value, args.item) : rule.message;
                errors.push(errorMessage);
            });

            return errors;
        },

        _normalizeRules: function (rules) {
            if (!$.isArray(rules))
                rules = [rules];

            return $.map(rules, $.proxy(function (rule) {
                return this._normalizeRule(rule);
            }, this));
        },

        _normalizeRule: function (rule) {
            if (typeof rule === "string")
                rule = { validator: rule };

            if ($.isFunction(rule))
                rule = { validator: rule };

            if ($.isPlainObject(rule))
                rule = $.extend({}, rule);
            else
                throw Error("wrong validation config specified");

            if ($.isFunction(rule.validator))
                return rule;

            return this._applyNamedValidator(rule, rule.validator);
        },

        _applyNamedValidator: function (rule, validatorName) {
            delete rule.validator;

            var validator = validators[validatorName];
            if (!validator)
                throw Error("unknown validator \"" + validatorName + "\"");

            if ($.isFunction(validator)) {
                validator = { validator: validator };
            }

            return $.extend({}, validator, rule);
        }
    };

    editGrid.Validation = Validation;


    var validators = {
        required: {
            message: "Field is required",
            validator: function (value) {
                return value !== undefined && value !== null && value.length !== 0;
            }
        },

        minLength: {
            message: "Field value is too long",
            validator: function (value, _, param) {
                return value.length >= param;
            }
        },

        maxLength: {
            message: "Field value is too short",
            validator: function (value, _, param) {
                return value.length <= param;
            }
        },

        min: {
            message: "Field value is too large",
            validator: function (value, _, param) {
                return value >= param;
            }
        },

        max: {
            message: "Field value is too small",
            validator: function (value, _, param) {
                return value <= param;
            }
        }
    };

    editGrid.validators = validators;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function SingleSuggestionField(config) {
        Field.call(this, config);
    }

    SingleSuggestionField.prototype = new Field({

        itemTemplate: function (value) {
            return (value != null) ? value.Name : "";
        },

        editValue: function () {
            var $input = this.editControl.children('input').first();
            var tokens = $input.tokenInput('get');
            if (tokens.length > 0) {
                return tokens[0];
            } else {
                return null;
            }
        },

        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);
            var $result = this.editControl = this._createSingleSuggestion(value);
            this.formatControl(item);
            return $result;
        },

        _createSingleSuggestion: function (value) {
            var $div = $("<div>");
            var $result = $("<input>");
            $result.appendTo($div);
            $result.tokenInput(this.suggestionUrl,
                {
                    theme: "facebook",
                    tokenLimit: 1,
                    method: 'POST',
                    queryParam: 'query',
                    tokenValue: 'Id',
                    propertyToSearch: 'Name',
                    minChars: 0,
                    prePopulate: value ? [value] : null,
                    local_data: this.local_data,
                    additionalParam: this.additionalParam,
                    onAdd: this.onAdd,
                    onDelete: this.onDelete,
                });
            return $div;
        }
    });

    editGrid.fields.singlesuggestion = editGrid.SingleSuggestionField = SingleSuggestionField;

}(editGrid, jQuery));

(function (editGrid, $, undefined) {

    var Field = editGrid.Field;

    function MultiSuggestionField(config) {
        Field.call(this, config);
    }

    MultiSuggestionField.prototype = new Field({

        itemTemplate: function (value) {
            if (value != null) {
                var displayData = "";
                for (var index = 0; index < value.length; index++) {
                    if (index != 0) {
                        displayData += "<br />";
                    }
                    displayData += value.Name;
                }

                return displayData;
            }
            return "";
        },

        editValue: function () {
            var $input = this.editControl.children('input').first();
            return $input.tokenInput('get');
        },

        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate(value);
            var $result = this.editControl = this._createMultiSuggestion(value);
            this.formatControl(item);
            return $result;
        },

        _createMultiSuggestion: function (value) {
            var $div = $("<div>");
            var $result = $("<input>");
            $result.appendTo($div);
            $result.tokenInput(this.suggestionUrl,
                {
                    theme: "facebook",
                    preventDuplicates: true,
                    method: 'POST',
                    queryParam: 'query',
                    tokenValue: 'Id',
                    propertyToSearch: 'Name',
                    minChars: 0,
                    prePopulate: value ? value : null,
                    local_data: this.local_data,
                    additionalParam: this.additionalParam,
                    onAdd: this.onAdd,
                    onDelete: this.onDelete,
                });
            return $div;
        }
    });

    editGrid.fields.multisuggestion = editGrid.MultiSuggestionField = MultiSuggestionField;

}(editGrid, jQuery));
(function (jsGrid, $, undefined) {

    var Field = jsGrid.Field;

    function SingleSuggestionField(config) {
        Field.call(this, config);
    }

    SingleSuggestionField.prototype = new Field({
        itemTemplate: function (value) {
            return (value != null) ? value.Name : "";
        }
    });

    jsGrid.fields.singlesuggestion = jsGrid.SingleSuggestionField = SingleSuggestionField;

}(jsGrid, jQuery));
(function (jsGrid, $, undefined) {

    var Field = jsGrid.Field;

    function MultiSuggestionField(config) {
        Field.call(this, config);
    }

    MultiSuggestionField.prototype = new Field({
        itemTemplate: function (value) {
            if (value != null) {
                var displayData = "";
                for (var index = 0; index < value.length; index++) {
                    if (index != 0) {
                        displayData += "<br />";
                    }
                    displayData += value[index].Name;
                }

                return displayData;
            }
            return "";
        },
    });

    jsGrid.fields.multisuggestion = jsGrid.MultiSuggestionField = MultiSuggestionField;

}(jsGrid, jQuery));