
function Retrieval() {
    var txtPossibleRetrievalQuantity = $('#PossibleRetrievalQuantity').val();
    var txtRequestedRetrievalQuantity = $('#RequestedRetrievalQuantity').val();

    if (txtPossibleRetrievalQuantity == null ||
        txtPossibleRetrievalQuantity.length < 1 ||
        txtRequestedRetrievalQuantity == null ||
        txtRequestedRetrievalQuantity < 1) {

        $("#errorList").text(message.MSG40).show();
        return;
    }

    var possibleRetrievalQuantity = parseInt(txtPossibleRetrievalQuantity);
    var requestedRetrievalQuantity = parseInt(txtRequestedRetrievalQuantity);

    if (requestedRetrievalQuantity > possibleRetrievalQuantity) {
        $("#errorList").text(message.MSG40).show();
        return;
    }
    $.ajax({
        url: "/RetrievalOfEmptyProductPallet/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            
            //$.unblockUI();
            if (!data.Success) {
                setTimeout(function () { showMessage(data, "", ""); }, 500);                

            } else {
                $.unblockUI();
                // Display confirmation message.
                if (!confirm("Ready to retrieve ?"))
                    return;

                // Block the UI.
                //$('#retrievalOfProductPallet').submit();
                $.ajax({
                    url: '/RetrievalOfEmptyProductPallet/RetrieveTheEmptyPallet',
                    type: 'post',
                    success: function (data) {
                        onSuccess(data);
                    }
                });
            }
        }
    });    
}
function ChangeShowMessage()
{
    $("#errorList").text(message.MSG40).hide();
}

function onSuccess(data, status, xhr) {

    if (!data.Success) {
        //location.reload();
        showMessage(data, "", "");
        return;
    }

    if (data.Success) {
        var value = $("#PossibleRetrievalQuantity").val() - $("#RequestedRetrievalQuantity").val();
        $("#PossibleRetrievalQuantity").val(value);
        blockScreenAccess("Waiting messages from C3...");
        //$.ajax({
        //    url: "/ProductManagement/RetrievalOfEmptyProductPallet/GetRequestedRetrievalQuantity",
        //    type: 'POST',
        //    data: {
        //        id: null
        //    },
        //    success: function (data) {
        //        $("#RequestedRetrievalQuantity").val(data.result);

        //        blockScreenAccess("Waiting messages from C3...");
        //    }
        //});
    }
}

$(function () {

    // Initiate communication to c2 hub.
    var signalrCommunication = initiateHubConnection(hubList.c3);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC3 = function (message) {

        // Message is invalid.
        if (message == null)
            return;

        if (message['PictureNo'] == null)
            return;

        // Message is not targeted to the current screen.
        if (message.PictureNo != 'TCPR131F')
            return;

        // Unblock the loading UI.
        $.unblockUI();

        // Call the reply function.
        $.ajax({
            url: '/ProductManagement/RetrievalOfEmptyProductPallet/Reply',
            type: 'post',
            success: function(response) {
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
                    toastr["info"](initiateThirdCommunicationResponseMessage(response[i]));

                //setTimeout(function() {
                //        location.reload();
                //    },
                //    500);
            }
        })
    }

    // Start connection to hub.
    signalrStartHubConnection();


});