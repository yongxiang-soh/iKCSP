gridHelper = {
    searchForm: function (gridId, formId) {
        var searchData = $("#" + formId).serializeObject();
        $("#" + gridId).jsGrid("search", searchData);
    },
    searchObject: function (gridId, obj) {
        $("#" + gridId).jsGrid("search", obj);
    },
    ReloadGrid: function (gridId) {
        $("#" + gridId).jsGrid("loadData");
    },
    addNewRow: function (gridId, item) {
        return $("#" + gridId).editGrid("addNewRow", item);
    },
    updateRow: function (gridId, rowName, item) {
        return $("#" + gridId).editGrid("updateRow", rowName, item);
    },
    removeRow: function (gridId, rowName) {
        $("#" + gridId).editGrid("removeRow", rowName);
    },
    getEditData: function (gridId) {
        return $("#" + gridId).editGrid("validateAndGetData");
    },
    generateNameColumn: function (value) {
        if (value != null) {
            return document.createElement('a').appendChild(
                document.createTextNode(value)).parentNode.innerHTML;
        }
    },
    generateNameColumnFormat: function (value) {
        if (value != null) {
            return "<div class='text-right'>" + document.createElement('a').appendChild(
                document.createTextNode(value)).parentNode.innerHTML + "</div>";
        }
    },

    // Base on out of spec status to show out of spec text status.
    initOutOfSpecStatus: function (index) {
        switch (index) {
            case 'Non':
                return 'Yet';
            case 'OK':
                return 'Ok';
            case 'NG':
                return 'NG';
            case 'Normal':
                return 'StrgOK';
            case 'OutofSpec':
                return 'StrgNG';
            default:
                return '';
        }
    },
    displayWeighingMethod: function (value) {
        if (value != null) {
            return document.createElement('a').appendChild(
                document.createTextNode(value)).parentNode.innerHTML;
        }
    },
    getListingData: function (gridId) {
        return $("#" + gridId).jsGrid("option", "data");
    },
    setEditData: function (gridId, data) {
        return $("#" + gridId).editGrid("setData", data);
    },
    displayNumberFormat: function (value) {
        return value == null ? "" : "<div class='text-right'>" + parseFloat(value).toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        }).replace("-,","-") + "</div>";
    },
    displayNumberFormat2: function (value) {
        return value == null ? "" : "<div class='text-right'>" + parseFloat(value).toFixed(2).replace(/./g, function (c, i, a) {
            var result = i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
            var res = result.replace("-,", "-");
            return res;
        }) + "</div>";
    },
    displayNumber: function (value) {
        return value == null ? "" : "<div class='text-right'>" + parseFloat(value).toString().replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        }) + "</div>";
    },
    displayNumberT: function (value) {
        return value == null ? "" : "<div class='text-right'>" + value + "</div>";
    },
    numberFormat: function (value) {
        return value == null ? "" : "<div class='text-right'>" + parseFloat(value).toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,") + "</div>";
    },
    formatNumber: function(value) {
        return value == null ? "" : parseFloat(value).toFixed(2).replace(/./g, function(c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) && ((a.length-i)/3>1) ? ',' + c : c;
        });
    },
    displayDate: function (value) {
        if (value == null || value == undefined) {
            return "N/A";
        }
       var dateStr = value.replace("/Date(", "").replace(")/", "");
        var date = new Date(1 * dateStr);
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }

        return dd + "/" + mm + "/" + date.getFullYear();
    },
    displayDateFormat: function (value) {
        if (value == undefined) {
            //return '<div class="text-right">N/A</div>';
            return '<div class="text-right"></div>';
        }
        var dateStr = value.replace("/Date(", "").replace(")/", "");
        var date = new Date(1 * dateStr);
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }

        return '<div class="text-right">' + dd + "/" + mm + "/" + date.getFullYear() + '</div>';
    },
    displayMonthAndYear: function (value) {
        if (value == undefined) {
            return "N/A";
        }
        var dateStr = value.replace("/Date(", "").replace(")/", "");
        var date = new Date(1 * dateStr);
        var mm = date.getMonth() + 1;

        if (mm < 10) {
            mm = "0" + mm;
        }

        return "<div class='text-right'>" + mm + "/" + date.getFullYear() + "</div>";
    },
    displayDateTime: function (value) {
        if (value == undefined) {
            return "N/A";
        }

        var dateStr = value.replace("/Date(", "").replace(")/", "");
        var date = new Date(1 * dateStr);

        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var hoursformat = "";
        var minutesformat = "";
        var secondsformat = "";
        hoursformat = hours < 10 ? "0" + hours : hours;
        minutesformat = minutes < 10 ? "0" + minutes : minutes;
        secondsformat = seconds < 10 ? "0" + seconds : seconds;
        //var ampm = hours >= 12 ? "PM" : "AM";
        //hours = hours % 12;
        //hours = hours ? hours : 12; // the hour '0' should be '12'
        //minutes = minutes < 10 ? "0" + minutes : minutes;
        //var strTime = hours + ":" + minutes + " " + ampm;
        var dd = date.getDate();
        var mm = (date.getMonth() + 1);
        if (dd < 10) {
            dd = "0" + dd;
        }
        if (mm < 10) {
            mm = "0" + mm;
        }
        var strTime = hoursformat + ":" + minutesformat + ":" + secondsformat;
        return dd + "/" + mm + "/" + date.getFullYear() + "  " + strTime;
    },
    displayDateTimeOnly: function (value) {
        if (value == undefined) {
            return "N/A";
        }

        var dateStr = value.replace("/Date(", "").replace(")/", "");
        var date = new Date(1 * dateStr);

        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var hoursformat = "";
        var minutesformat = "";
        var secondsformat = "";
        hoursformat = hours < 10 ? "0" + hours : hours;
        minutesformat = minutes < 10 ? "0" + minutes : minutes;
        secondsformat = seconds < 10 ? "0" + seconds : seconds;
        //var ampm = hours >= 12 ? "PM" : "AM";
        //hours = hours % 12;
        //hours = hours ? hours : 12; // the hour '0' should be '12'
        //minutes = minutes < 10 ? "0" + minutes : minutes;
        //var strTime = hours + ":" + minutes + " " + ampm;
        var strTime = hoursformat + ":" + minutesformat;
        return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear() + "  " + strTime;
    },
    generateCheckbox: function (value, item) {
        var str = "<input type='checkbox' class='ckb' value='" + value + "' item-id='" + item.Id + "'";
        if (item.IsDeleted) {
            str += " disabled='disabled'";
        }
        str += " >";
        return str;
    },
    generateRadiobox: function (value, item) {

        var str = "<input type='radio' class='ckb'  name='" + $(this._grid._container).attr("id") + '-' + this.name + "'" + " value='" + value + "' item-id='" + item.Id + "'";
        if (item.IsDeleted) {
            str += " disabled='disabled'";
        }
        str += " >";
        return str;
    },
    generateNumericBox: function (value, item) {

        var str = "<input type='number' class='form-control'  name='" + $(this._grid._container).attr("id") + '-' + this.name + "'" + " value='" + value + "' item-id='" + item.Id + "'" + " onchange='updateNumeric(event)'" ;
        if (item.IsDeleted) {
            str += " disabled='disabled'";
        }
        str += " >";
        return str;
    },
    generateTextBox: function (value, item) {

        var str = "<input type='text' class='form-control'  name='" + $(this._grid._container).attr("id") + '-' + this.name + "'" + " value='" + value + "' item-id='" + item.Id + "'";
        if (item.IsDeleted) {
            str += " disabled='disabled'";
        }
        str += " >";
        return str;
    },
    getSearchCondition: function (gridId) {
        return $("#" + gridId).jsGrid("getSearchCondition");
    },
    getItemByRowName: function (gridId, rowName) {
        return $("#" + gridId).editGrid("getItemByRowName", rowName);
    },
    getItemByRowDom: function (gridId, row) {
        if ($("#" + gridId).hasClass("jsgrid")) {
            return $(row).data("JSGridItem");
        } else {
            return $(row).data("EditGridItem");
        }
    },
    getSelectedItem: function (gridId) {
        var selectedRow = $("#" + gridId + " .jsgrid-selected");
        if (selectedRow.length > 0) {
            return $("#" + gridId + " .jsgrid-selected").data("JSGridItem");
        } else {
            return null;
        }
    },
    refreshSize: function (gridId) {
        if ($("#" + gridId).hasClass("jsgrid")) {
            $("#" + gridId).jsGrid("refresh");
        } else {
            $("#" + gridId).editGrid("refresh");
        }
    },
    //==FUNC: DEFINE Name Column
    generateEmailIdColumn: function (value, item) {
        return "<a href='' data-toggle='modal' data-target='#modalView' onclick='emailTemplateModule.viewEmailTemplate(\""
            + item.Id + "\");'>" + commonHelper.htmlEncode(value) + "</a>";
    },

    generatePurposeColumn: function (value, item) {
        return commonHelper.htmlEncode(value);
    },

    generateCreateByColumn: function (value, item) {
        var href = emailTemplateModule.urlUser.replace("__id__", item.UserIdCreatedBy);
        return "<a href=" + href + ">" + value + "</a>";
    },

    generateModifiedByColumn: function (value, item) {
        var href = emailTemplateModule.urlUser.replace("__id__", item.UserIdModifiedBy);
        return "<a href=" + href + ">" + value + "</a>";
    },
    generateEditAction: function (value, item) {
        return "<button type='button' class='btn btn-gray' onclick='edit(\"" + value + "\");'><i class='fa fa-pencil' style='padding: 0 0 0 0;'></i></button>";

    },
    generateDeleteAction: function (value, item) {
        return "<button type='button' class='btn btn-danger' onclick='deleteItem(event);'><i class='glyphicon glyphicon-trash' style='padding: 0 0 0 0;'></i></button>";

    },

    generateKneadingCommandStatus: function (value, item) {

        if (value == '0')
            return "Yet";

        if (value == '1')
            return "Kneading";

        if (value == "2")
            return "Kneaded";

        if (value == "3")
            return "F Kneaded";

        if (value == "4")
            return "Stored";

        if (value == "5")
            return "Tabletised";

        if (value == "6")
            return "Commanding";

        if (value == "7")
            return "Tbt Commanded";

        if (value == "8")
            return "F Retrieving";

        if (value == "9")
            return "F Retrieved";

        return "";
    },
    initiateProductStatus: function (value, item) {

        if (value == '0')
            return "Yet";

        if (value == '1')
            return "Command";

        if (value == "2")
            return "Product";

        if (value == "3")
            return "End";

        if (value == "4")
            return "F End";

        if (value == "5")
            return "Stopped";

        if (value == "6")
            return "Stopping";

        if (value == "7")
            return "Tbt Command";

        return "";
    },
    findProductPalletShelfStatus: function (value, item) {

        if (item.F51_ShelfStatus == '0')
            return "Empty";

        if (item.F51_ShelfStatus == '1')
            return "Warehouse Use Pallets Stocked";

        if (item.F51_ShelfStatus == "3")
            return "Out-of-spec Pre-Products Stocked";

        return "";
    },
    onDataLoaded: function (grid) {
        if (grid.data.data.length === 0) {
            $("#btnExport").attr("disabled", "disabled");
        } else {
            $("#btnExport").removeAttr("disabled");
        }

        $("#gridEmailTemplateSearch .jsgrid-grid-body .ckb");

    },
    /**
 * Find single selected item on the grid by searching its radio button.
 * @param {} gridIndex 
 * @returns {} 
 */
    findSelectedRadioItem: function (gridIndex) {

        // Find item row index.
        var rowIndex = $("#" + gridIndex).find("td > input[type='radio']:checked").parent().parent().index();

        // Invalid row index.
        if (rowIndex < 0)
            return null;

        // Find items list which is shown on grid.
        var items = this.getListingData(gridIndex);

        // Items are invalid.
        if (items == null || items.length < 1)
            return null;

        // Index exceeds the items count.
        if (rowIndex > items.length - 1)
            return null;

        return items[rowIndex];
    }

}



/**
 * Fired when document is rendered on browser.
 */
$(document).bind('DOMNodeInserted', function (e) {
    /**
     * Allow radio button is selected when grid row is clicked.
     */
    $(".jsgrid-table").find("tr").click(function () {
        $(this).find("input[type='radio']:not(:disabled)").prop('checked', true).change();
    });
});
