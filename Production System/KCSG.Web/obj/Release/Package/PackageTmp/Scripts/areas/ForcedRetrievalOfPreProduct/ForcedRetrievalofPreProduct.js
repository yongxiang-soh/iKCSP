
$(document).ready(function () {
    // trigger checked default
    var $radios = $('input[name="GroupName"]');
    $radios.filter('[value="0"]').prop('checked', true);

    $("#btnRetrieval").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#ForcedRetrievalGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnRetrieval').prop("disabled", false) : $('#btnRetrieval').prop("disabled", true);
    });
});

//	If the selected criteria = “Not Command”, the system will show the confirmation message as MSG 31. 
//	The user clicks on [OK] button to continue retrieving data as 
//	The user clicks on [Cancel] button, the message close. Nothing changes.
//	If the selected criteria = “Tabletised”, the system will show the confirmation message as MSG 18.
//	The user clicks on [OK] button to continue retrieving data as 
//	The user clicks on [Cancel] button, the message close. Nothing changes.

function ChangeGrid() {
    var groupNameValue = $("input:radio[name=GroupName]:checked").val();
    var request = {
        groupNameValue: groupNameValue
    }
    gridHelper.searchObject("ForcedRetrievalGrid", request);
}
function Retrieval() {
    var groupName = $('input[name=GroupName]:checked').val();
    if (groupName === "0") {
        if (!confirm("Ready to retrieve this lot of pre-product as out-of-spec's?"))
            return;
    } else {
        if (!confirm("Ready to retrieve?"))
            return;
    }
    var commandNo = gridHelper.getSelectedItem("ForcedRetrievalGrid").F42_KndCmdNo;
    var lotNo = gridHelper.getSelectedItem("ForcedRetrievalGrid").F42_PrePdtLotNo;
    var containerCode = gridHelper.getSelectedItem("ForcedRetrievalGrid").F49_ContainerCode;
    var containerNo = gridHelper.getSelectedItem("ForcedRetrievalGrid").F37_ContainerNo;
    var shelfNo = gridHelper.getSelectedItem("ForcedRetrievalGrid").F37_ShelfNo;
    $.ajax({
        url: formUrl.urlRetrieval,
        type: 'POST',
        data: { commandNo: commandNo, lotNo: lotNo, containerCode: containerCode, containerNo: containerNo, shelfNo: shelfNo, groupName:groupName },
        success: function (data) {
            
            if (data.Success) {
                // Block the UI.
                blockScreenAccess('Waiting for messages from C2.');
            } else {
                
                showNormanMessageCustom(data, "ForcedRetrievalGrid");
            }
            
        }
    });
}

/**
 * Find the selected item of force retrieval pre-product item.
 * @returns {} 
 */
findSelectedForceRetrievalPreProduct = function () {
    return gridHelper.findSelectedRadioItem("ForcedRetrievalGrid");
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

        if (message.PictureNo != 'TCIP061F')
            return;
        var isNotCommand =  $("input:radio[name=GroupName]:checked").val() ==="0";
        // Unblock UI.
        $.unblockUI();
        var shelfNo = gridHelper.getSelectedItem("ForcedRetrievalGrid").F37_ShelfNo;
        // Find the selected pre-product code.
        var item = findSelectedForceRetrievalPreProduct();
        if (item == null)
            return;

        $.ajax({
            url: "/ForcedRetrievalOfPreProduct/ReplySecondCommunication",
            type: "post",
            data: {
                preProductCode: item.F42_PreProductCode,
                commandNo: item.F42_KndCmdNo,
                commandLotNo: item.F42_PrePdtLotNo,
                containerCode: item.F49_ContainerCode,
                isNotCommand: isNotCommand, shelfNo: shelfNo
            },
            success: function (response) {
                findC2ResponseInformation(response);
                $('#btnRetrieval').prop("disabled", true);
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();
});
