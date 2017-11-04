/**
 * This function is for requesting server to analyze supplier pallet and retrieve 'em.
 * @returns {} 
 */

supplierPalletValidate = function () {
    return $("#supplierRetrievalForm").validate().form();
}

analyzeValidationmessages = function (responseJson) {
    var keys = Object.keys(responseJson);
    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        for (var j = 0; j < responseJson[key].length; j++) {
            var validationMessage = responseJson[key][j];
            $('span[data-valmsg-for="' + key + '"]')
                .removeClass("field-validation-valid")
                .addClass("field-validation-error")
                .text(validationMessage);

            $('input[name="' + key + '"]')
                .removeClass("field-validation-valid")
                .addClass("field-validation-error");

            break;
        }
    }
}

FindPossibleRetrievalQuantity = function () {
    var supplierCode = $('#txtSupplierCode').val();
    $.ajax({
        url: "/MaterialManagement/RetrievalOfSupplierPallet/FindPossibleRetrievalQuantity",
        data: { supplierCode: supplierCode },
        type: "post",
        success: function (data) {
            $('#PossibleRetrievalQuantity').val(data.possibleRetrievalQuantity);
            $('#PossibleRetrievalQuantityOfPallet').val(data.numberOfRecord);
            $('#RequestedRetrievalQuantity').val(data.possibleRetrievalQuantity);
        }

    });
}

function retrieveSupplierPallet() {

    // Check whether form is valid or not.
    var isSupplierPalletValid = supplierPalletValidate();

    // Input information is invalid. Prevent form submission.
    if (!isSupplierPalletValid)
        return;
    var no1 = $("#RequestedRetrievalQuantity").val();
    var no2 = $("#PossibleRetrievalQuantity").val();

    if (parseFloat(no1) > parseFloat(no2)) {
        $("#RequestRetrievalQuantityError").text('The retrieval quantity cannot be more than the possible retrieval quantity!').show();
        return;
    }

    if (!confirm("Are you sure you want to retrieve?"))
        return;

    $.ajax({
        //url: formUrlPdtPln.urlEdit,
        url: "/MaterialManagement/RetrievalOfSupplierPallet/Retrieve",
        type: 'POST',
        data: $("#supplierRetrievalForm").serialize(),
        success: function (data) {
            
            if (!data.Success) {
                showMessage(data, "", "");
                return;
            }
            blockScreenAccess('Waiting for message from C1.');
        }
    });
}

//#region Signalr communication

$(function () {
    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCRM131F')
            return;

        $.ajax({
            url: '/MaterialManagement/RetrievalOfSupplierPallet/UpdateProductCommand',
            type: 'POST',
            success: function (response) {
                if (response == null || !(response instanceof Array))
                    return;

                for (var index = 0; index < response.length; index++) {
                    toastr["info"](initiateFirstCommunicationMessage(response[index]));
                }
            }
        });
    }

    // Start connection to hub.
    signalrStartHubConnection();
});

//#endregion