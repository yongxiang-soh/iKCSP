
function edit(from,to) {
    var from = $("#txtFromNumber").val().trim();
    var to = $("#txtToNumber").val().trim();
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: {
            from: from,
            to:to
        },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F01_MaterialDsp').focus();

            
        }
    });
}


function onSuccess(data, status, xhr) {
    showMessage(data, "MaterialGrid", "");
    $("#formPlaceholder").html("");
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);

    blockScreenAccess("Waiting for message from C1");
}


$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo !== 'TCRM091F')
            return;

        // Unblock UI.
        $.unblockUI();

        $.ajax({
            url: '/MaterialManagement/InterFloorMovementOfMaterial/Complete',
            type: 'post',
            success: function (items) {

                if (items == null || !(items instanceof Array))
                    return;

                for (var index = 0; index < items.length; index++) {
                    toastr["info"](initiateFirstCommunicationMessage(items[index]));
                }
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();
});
