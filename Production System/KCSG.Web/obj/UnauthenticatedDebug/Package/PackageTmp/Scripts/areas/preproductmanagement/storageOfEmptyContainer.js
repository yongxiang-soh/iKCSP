$(document).ready(function() {
    $('#ContainerNo').keyup(function () {
        $("#errorlst").text("").hide();
        $("#ContainerNo").removeClass('input-validation-error');
    });
})

function Assign() {
    if ($('#txtContainerType').valid() & $('#txtContainerName').valid()) {

        $.ajax({
            url: formUrl.urlAssign,
            type: 'POST',
            data: { id: null },
            success: function (data) {
                if (data != null && data!="") {
                    $('#txtShelfNo').val(data);
                } else {
                    alert("There is no empty container in stock.");
                }
            }
        });
    }
}

function CheckExistsTX50() {
    $.ajax({
        url: formUrl.urlCheckExistsTX50,
        type: 'OPTIONS',
        data: { id: null },
        success: function (data) {
            if (!data.Success) {
                alert(data.Message);
                $('#btnStorage').prop("disabled", "true");
                $.unblockUI();
            }
            $.unblockUI();

        }
    });
}


$(function () {

    // Initiate communication to c2 hub.
    var signalrCommunication = initiateHubConnection(hubList.c2);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC2 = function (message) {

        if (message == null)
            return;

        if (message['PictureNo'] == null)
            return;

        if (message.PictureNo != 'TCIP011F') 
            return;

        // Unblock UI.
        $.unblockUI();

        // Find information on screen which is for submission.
        var containerType = $("#txtContainerType").val();
        if (containerType == null || containerType.length < 1)
            return;

        var containerNo = $("#ContainerNo").val();
        if (containerNo == null || containerNo.length < 1)
            return;


        $.ajax({
            url: "/StorageOfEmptyContainer/ReceiveMessageFromC2",
            type: "post",
            data: {
                containerType: containerType,
                containerNo: containerNo
            },
            success: function(response) {
                findC2ResponseInformation(response);
            },
            error: function(response) {
                alert(response.Message);
            }
        });
        
    }

    // Start connection to hub.
    signalrStartHubConnection();

    
});

/**
 * Callback which is fired when storage button is clicked (Storage of empty container)
 * @returns {} 
 */
storageEmptyContainer = function() {
    // Check whether form is valid or not.
    var isFormValid = $("#addNewForm").valid();
    if (!isFormValid) {
        return;
    }

    var containerNo = parseInt($('#ContainerNo').val());

    if (containerNo < 1) {
        $("#errorlst").text("Please input data for this required field.").show();
        $("#ContainerNo").addClass('input-validation-error');
        return;
    }
    $.ajax({
        url: '/PreProductManagement/StorageOfEmptyContainer/InsertAndUpdate',
        type: 'post',
        data: {
            containerType: $('#txtContainerType').val(),
            containerName: $('#txtContainerName').val(),
            containerNo: $('#ContainerNo').val(),
            storageShelfNo: $('#txtShelfNo').val()
        },
        success: function (response) {
            showMessage(response);
            // Block the UI.
            if (response.Success) {
                blockScreenAccess('Waiting for messages from C2.');
            }
        }
    });
}