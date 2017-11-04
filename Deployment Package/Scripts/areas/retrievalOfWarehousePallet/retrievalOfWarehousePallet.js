$(document).ready(function () {
 
    //$('#RequestRetrievalQuantity').keyup(function () {

    //    if (Number($('#RequestRetrievalQuantity').val())>0)
    //        $("#errorPONo").text("");
    //});

    $('#RequestRetrievalQuantity').keyup(function () {
        var hasValue = this.value.length > 0;
        $("#RequestRetrievalQuantityError").text("").hide();
    });
  

});

function Retrieval() {
    if (!CheckValidation())
        return;
    $('#frmRetrievalWh').submit();
    

}
function CheckValidation() {
    var checked = true;
    var requestRetrievalQuantiy = $('#RequestRetrievalQuantity').val();
    if (requestRetrievalQuantiy === null || requestRetrievalQuantiy === "") {
        $("#RequestRetrievalQuantityError").text("Please input data for this required field.").show();
        return false;
    }

    if (Number($('#RequestRetrievalQuantity').val()) <= 0) {
        $("#RequestRetrievalQuantityError").text(message.MSG39).show();
        return false;
    }

    if (Number($('#RequestRetrievalQuantity').val()) > Number($('#PossibleRetrievalQuantity').val())) {
        $("#RequestRetrievalQuantityError").text(message.MSG40).show();
        return false;
    }
    //if (checked === false)
    //    return false;
    return true;

}


function onSuccess(data) {
    if (data.Success) {
        var value = $("#PossibleRetrievalQuantity").val() - $("#RequestRetrievalQuantity").val();
        $("#PossibleRetrievalQuantity").val(value);
        blockScreenAccess("Waiting messages from C1...");
        return;
    } else {
        showMessage(data);
        return;
    }
    $("#RequestRetrievalQuantity").val("");

    // Unblock UI.
    $.unblockUI();
}
$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM111F')
            return;

        $.ajax({
            url: '/MaterialManagement/RetrievalOfWarehousePallet/RetrieveWarehousePalletMessageC1Reply',
            type: 'post',
            data: {
               // materialCode: materialCode
            },
            success: function (response) {

                if (response == null || !(response instanceof Array))
                    return;

                for (var index = 0; index < response.length; index++)
                    toastr["info"](initiateFirstCommunicationMessage(response[index]));
            }
        });
    }

    // Start connection to hub.
    signalrStartHubConnection();
});