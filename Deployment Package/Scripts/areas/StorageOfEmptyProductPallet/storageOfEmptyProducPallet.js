$(document).ready(function() {

    $('#PalletNumber').keyup(function() {
        $("#errorList").text("").hide();
    });
})

function Storage() {
    $.ajax({
        url: "/StorageOfEmptyProductPallet/CheckStatus",
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
                if (!$("#restorageProduct").valid())
                    return;
                var palletNumber = $('#PalletNumber').val();
                if (palletNumber === null || palletNumber === "") {
                    $("#errorList").text(message.MSG2).show();
                    return;
                }

                if (parseInt(palletNumber) < 0) {
                    $("#errorList").text(message.MSG14).show();
                    return;
                }
                $.ajax({
                    url: formUrl.urlChecked,
                    type: 'POST',
                    data: { palletNumber: palletNumber },
                    success: function (data) {
                        $.unblockUI();
                        setTimeout(function () {
                            if (!data.Success) {
                                showMessage(data, "", "");
                                return;
                            } else {
                                if (!confirm("Ready to store?")) {
                                    return;
                                }
                                $('#restorageProduct').submit();
                            }
                        }, 500);

                    }
                });
            }
        }
    });       
}
function onSuccess(data, status, xhr) {
    showMessage(data, "", "");

    if (data.Success)
        blockScreenAccess('Waiting messages from C3');
}



/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR121F')
            return;

        // Unblock the UI.
        $.unblockUI();

        // Respond from C3.
        $.ajax({
            url: '/StorageOfEmptyProductPallet/ReplyMessageC3',
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
                $("#PalletNumber").val("1");
                for (var i = 0; i < response.length; i++)
                    toastr["info"](initiateThirdCommunicationResponseMessage(response[i]));
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});
