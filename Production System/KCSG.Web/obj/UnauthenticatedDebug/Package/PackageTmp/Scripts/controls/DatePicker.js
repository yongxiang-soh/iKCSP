//************* Public function *************
function DatePicker_Init(controlId) {
    // update validate date format
    $.validator.methods.date = function (value, element) {
        var dateFormat = /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/;
        return this.optional(element) || value.match(dateFormat);
    };
    var dateValue = $('input[name=' + controlId + '].customDatePicker').val();
    if (dateValue != "") {
        dateValue = dateValue.substring(0, 10);
    }
    $('input[name=' + controlId + '].customDatePicker').val(dateValue);
    $('input[name=' + controlId + '].customDatePicker').datepicker({
        onSelect: function (date) {
            $(this).trigger("change");
        },
        dateFormat: 'dd/mm/yy',
        showOn: "button",
        buttonImage: "/images/calendar.png",
        buttonImageOnly: true,
        showButtonPanel: true
    });
    //$('input[name=' + controlId + '].customDatePicker').prop('readonly', true);

}
//************* End Public function *************
function getFormattedDate(date) {
    var year = date.getFullYear();
    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;
    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;
    return day + '/' + month + '/' + year;
}

//TienVN custom v2 for INIR
function DatePicker_Init_V2(controlId) {
    // update validate date format
    //$.validator.methods.date = function (value, element) {
    //    var dateFormat = /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/;
    //    return this.optional(element) || value.match(dateFormat);
    //}
    var dateValue = $('input[name=' + controlId + '].customDatePicker').val();
    if (dateValue != "") {
        dateValue = dateValue.substring(0, 11);
    }
    $('input[name=' + controlId + '].customDatePicker').val(dateValue);

    $('input[name=' + controlId + '].customDatePicker').datepicker({
        dateFormat: "dd M yy",
        showOn: "button",
        buttonImage: "/images/calendar.png",
        buttonImageOnly: true,
        showButtonPanel: true,
        onSelect: function (date) {
            $(this).trigger("change");
        }
    });
    //$('input[name=' + controlId + '].customDatePicker').prop('readonly', true);

}

function DatePickerNewFormat_Init(controlId, formattedValue) {
    // update validate date format
    $.validator.addMethod('date',
    function (value, element) {
        if (this.optional(element)) {
            return true;
        }
        try {
            $.datepicker.parseDate('dd M yy', value);
            return true;
        }
        catch (err) {
            return false;
        }
    });
    
    $("input[name='" + controlId + "'].customDatePicker").val(formattedValue);

    $("input[name='" + controlId + "'].customDatePicker").datepicker({
        dateFormat: 'dd M yy',
        showOn: "button",
        buttonImage: "/images/calendar.png",
        buttonImageOnly: true,
        showButtonPanel: true,
        onSelect: function () {
            $(this).trigger("change");
        },
    });
    $("input[name='" + controlId + "'].customDatePicker").trigger("change");


}
