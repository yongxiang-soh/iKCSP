function onSuccess(data, status, xhr) {
    debugger;
    if (data.Success) {
        blockScreenAccess('Waiting messages from C3...');
        return;
    }

    showMessage(data, "", "");
}

//$().ready(function () {
//        $("#txtFromNumber").attr("onkeypress", "return isNumberKey(event);");
//        $("#txtToNumber").attr("onkeypress", "return isNumberKey(event);");

//    });

/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        if (message.PictureNo != 'TCPR081F')
            return;

        $.ajax({
            url: '/ProductManagement/InterFloorMovementOfProduct/ProcessThirdCommunicationData',
            type: 'post',
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
                    toastr["info"](initiateInterfloorThirdCommunicationResponseMessage(response[i]));
            }
        });

    }

    // Start hub connection.
    signalrStartHubConnection();
});


/**
 * From data returned back from service.
 * @returns {} 
 */
initiateInterfloorThirdCommunicationResponseMessage = function (pdtWarehouseCommand) {

    var htmlMessage = '';
    htmlMessage += '<ul>';
    htmlMessage += '<li>Warehouse: Product</li>';
    htmlMessage += '<li>Move from ' + pdtWarehouseCommand.F47_From + ' to ' + pdtWarehouseCommand.F47_To + '</li>';
    htmlMessage += '<li>Product Code: 0</li>';
    htmlMessage += '<li>Pallet No: ' + pdtWarehouseCommand.F47_PalletNo + '</li>';

    if (pdtWarehouseCommand.OldStatus == '6')
        htmlMessage += '<li>Status: success.</li>';
    else if (pdtWarehouseCommand.OldStatus == '7')
        htmlMessage += '<li>Status: cancel.</li>';
    else if (pdtWarehouseCommand.OldStatus == '9')
        htmlMessage += '<li>Status: two time storage.</li>';

    htmlMessage += '</ul>';

    return htmlMessage;
}

function clickOK() {

    $.ajax({
        url: "/InterFloorMovementOfProduct/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            
            //$.unblockUI();
            if (!data.Success) {
                setTimeout(function () { showMessage(data, "", ""); }, 500);
                //if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                //    setTimeout(function () {
                //        var errorMessage2 = { Success: false, Message: data.Message2 }
                //        showMessage(errorMessage2, "", "");
                //    }, 4000);
                //}
                //return;

            } else {
                $.unblockUI();
                setTimeout(function() {
                    if (confirm("Ready to transfer between floors?")) {
                        $("#div-tranfer-interfloor").submit();
                    }
                },500);

            }
        }
    });
}