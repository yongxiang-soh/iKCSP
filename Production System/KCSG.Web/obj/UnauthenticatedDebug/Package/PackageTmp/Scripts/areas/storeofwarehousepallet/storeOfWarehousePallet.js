
function onSuccess(data, status, xhr) {
    if (data.Success) {
        blockScreenAccess('Waiting message from C1');
    }
}


$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c1);

    signalrConnection.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM101F')
            return;

        $.unblockUI();

        // Data hasn't been selected.
      
        $.ajax({
            url: '/MaterialManagement/StorageOfWarehousePallet/ProcessFirstCommunicationData',
            type: 'post',
            success: function (response) {
                if (response == null || !(response instanceof Array))
                    return;

                for (var index = 0; index < response.length; index++)
                    toastr["info"](initiateFirstCommunicationMessage(response[index]));
             }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});