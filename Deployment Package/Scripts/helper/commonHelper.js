messagesList = {
    INVALID_CONVEYOR_STATUS: "Conveyor status error!",
    INVALID_PRODUCT_SHELF_STATUS: "Invalid product shelf status",
    INVALID_WAREHOUSE_STATUS: "Warehouse status error!",
    RECORD_BEING_MODIFIED: "This record was modified by others, please retry!",
    MATCHED_PALLET_NOT_FOUND: "There is not matched pallet!",
    INVALID_SHELF_NO: "Invalid shelf no",
    INVALID_PALLET_NO: "Invalid pallet no",
    INVALID_MATERIAL_STATUS: "Invalid material status",
    INVALID_MATERIAL_SHELF_STATUS: "Invalid material shelf status",
    INVALID_KNEADING_COMMAND: "Invalid kneading command",
    NO_EMPTY_LOCATION_AVAILABLE_IN_WAREHOUSE: "There is no empty location available in the warehouse now!",
    INVALID_DEVICE_AVAILABILITY: "Warehouse status error!",
    INPUT_TABLET_NOTMATCH_DATABASE: "The input tablet line does not match the tablet line in database!",
    INVALID_INSERT_TX47: "",
    NO_CORRESPONDING_IN_DATABASE: "Cannot find corresponding record in the Database."
}

commonHelper = {
    htmlEncode: function(value) {
        return $('<div/>').text(value).html();
    },
    htmlDecode: function(value) {
        return $('<div/>').html(value).text();
    },
    urlEncode: function (value) {
        return encodeURI(value);
    },
    urlDecode: function (value) {
        return decodeURI(value);
    },
}

function setMaxLength($fields, optsize, optoutputdiv) {
    var $ = jQuery;
    $fields.each(function (i) {
        var $field = $(this);
        $field.data("maxsize", optsize || parseInt($field.attr("maxlength"))); //max character limit
        var statusdivid = optoutputdiv || $field.attr("data-output"); //id of DIV to output status
        $field.data("$statusdiv", $("#" + statusdivid).length === 1 ? $("#" + statusdivid) : null);
        $field.unbind("keypress.restrict")
			.bind("keypress.restrict",
				function (e) {
				    setMaxLength.restrict($field, e);
				});
        $field.unbind("keyup.show")
			.bind("keyup.show",
				function (e) {
				    setMaxLength.showlimit($field);
				});
        setMaxLength.showlimit($field); //show status to start
    });
}
var uncheckedkeycodes = /(8)|(13)|(16)|(17)|(18)/; //keycodes that are not checked, even when limit has been reached.
setMaxLength.restrict = function ($field, e) {
    var keyunicode = e.charCode || e.keyCode;
    if (!uncheckedkeycodes.test(keyunicode)) {
        if ($field.val().length >= $field.data('maxsize')) { //if characters entered exceed allowed
            if (e.preventDefault) {
                e.preventDefault();
            }
            return;
        }
    }
}

setMaxLength.showlimit = function ($field) {
    if ($field.val().length > $field.data("maxsize")) {
        var trimmedtext = $field.val().substring(0, $field.data("maxsize"));
        $field.val(trimmedtext);
    }
    if ($field.data("$statusdiv")) {
        $field.data("$statusdiv").css("color", "").html($field.val().length);
        //calculate chars remaining in terms of percentage
        var pctremaining = ($field.data("maxsize") - $field.val().length) / $field.data("maxsize") * 100;
        for (var i = 0; i < thresholdcolors.length; i++) {
            if (pctremaining <= parseInt(thresholdcolors[i][0])) {
                $field.data("$statusdiv").css("color", thresholdcolors[i][1]);
                break;
            }
        }
    }
}

/**
 * Analyze exception message from service and display it to screen.
 * @returns {} 
 */
analyzeExceptionMessage = function(messageResult) {
    
    // Invalid message result.
    if (messageResult == null)
        return;

    // No message has been thrown back.
    if (messageResult.Message == null || messageResult.Message.length < 1)
        return;

    // Find actual message which should be displayed on screen.
    var message = messagesList[messageResult.Message];

    // No message should be displayed.
    if (message == null || message.length < 1)
        return;

    // Update message result.
    messageResult.Message = message;

    // Display message.
    showMessage(messageResult);
}

parseShelfNo = function(shelfNo) {
    
    // Regex of slashed shelf no.
    var slashedShelfNoRegex = /^\d{2}\-\d{2}\-\d{2}$/;
    if (slashedShelfNoRegex.test(shelfNo)) {
        // Split the string.
        return shelfNo.split('-');
    }

    var nonSlashedRegex = /^\d{6}$/;
    if (nonSlashedRegex.test(shelfNo)) {
        var items = [];

        items.push(nonSlashedRegex.substring(0, 2));
        items.push(nonSlashedRegex.substring(2, 2));
        items.push(nonSlashedRegex.substring(4, 2));

        return items;
    }

    return null;
}
