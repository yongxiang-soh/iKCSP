var externalItem = null;

/**
 * Fired when document is rendered on browser.
 */
$(document).bind('DOMNodeInserted', function (e) {
    /**
     * Allow radio button is selected when grid row is clicked.
     */
    /**
     * Callback which is called when shipping command item is clicked.
     */
    $("#RetrievalOfExternaPreProductGrid")
        .find("tr")
        .removeAttr("onclick")
        .attr("onclick", "externalProductClick(event)");
});

/**
 * Callback which is called when external product is clicked.
 * @returns {} 
 */
externalProductClick = function () {

    // Find the current selected item on External product grid.
    var item = findSelectedExternalProduct();

    if (item == null)
        return;
    if (item.Line != "") {
        $("#txtTabletisingLine").val(item.Line);
        $(".input-group-addon").hide();

        $.ajax({
            url: "/RetrievalOfExternalPreProduct/RetrievalTabletisingLineName",
            type: "post",
            data: {
                Line: $("#txtTabletisingLine").val()
            },
            success: function (response) {
                $("#txtTabletisingName").val(response);
                }
        });

    } else {
        $("#txtTabletisingLine").val(item.Line);
        $("#txtTabletisingName").val("");
        $(".input-group-addon").show();
    }

    //$("#txtTableListingLine").val(item.F41_TableLine);

    // Enable retrieval button.
    $("#btnRetrieval").prop("disabled", false);

}

/**
 * Retrieval of external product.
 * @returns {} 
 */
retrievalExternalProduct = function () {
    // Lines don't match.
    // Find the current selected item on External product grid.
    var item = findSelectedExternalProduct();
        // Find the selected record.
    if (item == null)
        return;
    //var tableListingLine = $("#txtTabletisingLine").val();
    //if (tableListingLine != item.Line) {
    //    var notification = {
    //        Message: messagesList['INPUT_TABLET_NOTMATCH_DATABASE'],
    //        Success: false
    //    };

    //    showMessage(notification);
    //    return;
    //}




    if (!confirm("Ready to retrieve?"))
        return;

    externalItem = null;

    $.ajax({
        url: "/RetrievalOfExternalPreProduct/Retrieval",
        type: "post",
        data: {
            Line: $("#txtTabletisingLine").val(),
            F41_KndCmdNo: item.F41_KndCmdNo,
            F41_PrePdtLotNo: item.F41_PrePdtLotNo,
            F41_PreProductCode: item.F41_PreProductCode
        },
        success: function(response) {
            if (!response.Success) {
                var message = analyzeExceptionMessage(response);
                showMessage(message);
                return;
            }
            showMessage(response);

            externalItem = item;
            blockScreenAccess('Waiting for messages from C3');

        }
    });
}

findSelectedExternalProduct = function () {

    // Find the currently selected external product item.
    return gridHelper.findSelectedRadioItem("RetrievalOfExternaPreProductGrid");
}

/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        if (message.PictureNo != 'TCPR101F')
            return;

        var item = externalItem;
        if (item == null)
            return;
            
        $.unblockUI();
        
        $.ajax({
            url: '/ProductManagement/RetrievalOfExternalPreProduct/Reply',
            type: 'post',
            data: {
                preProductCode: item.F41_PreProductCode,
                kneadingCommandNo: item.F41_KndCmdNo,
                preProductLotNo: item.F41_PrePdtLotNo
            },
            success: function (response) {
                // Invalid response.
                if (response == null)
                    return;

                // Response is not an array of message.
                if (!(response instanceof Array))
                    return;

                // Messages list is empty.
                if (response.length < 1)
                    return;

                for (var i = 0; i < response.length; i++)
                    toastr["info"](initiateThirdCommunicationResponseMessage(response[i], null, "External Pre-product", null));

                $("#txtTabletisingName,#txtTabletisingLine").val("");
                gridHelper.ReloadGrid("RetrievalOfExternaPreProductGrid");
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});