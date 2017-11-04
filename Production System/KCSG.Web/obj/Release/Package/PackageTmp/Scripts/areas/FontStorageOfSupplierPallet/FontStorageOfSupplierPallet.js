$().ready(function() {
    $.validator.addMethod("Compare",
           function (value, element, params) {
               return true;

           },
           "Pallet quantity error !");
    $("#StorageQuantity").rules('add', 'Compare');
});



function save() {
    if ($("#fromStorage").valid()) {
        $.ajax({
            url: "/MaterialManagement/FontStorageOfSupplierPallet/Storage",
            data: $("#fromStorage").serialize(),
            type: "POST",
            async: false,
            success: function(data) {
                if (data.IsSuccess) {

                    blockScreenAccess('Waiting messages from C1.');
                } else {
                    showMessage(data, '', null);
                }
            }
        });
    }
}
$(function () {

    // Initiate communication to c2 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message == null)
            return;

        if (message['PictureNo'] == null)
            return;

        if (message.PictureNo != 'TCRM123F')
            return;
      
        // Unblock UI.
        $.unblockUI();
     

        $.ajax({
            url: "/FontStorageOfSupplierPallet/PostProcessMaterial",
            type: "post",
            data: {
                supplierCode: $("#txtSupplierCode").val(),
                storageQuantity: $("#StorageQuantity").val(),
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
