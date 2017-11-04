
$(document).ready(function () {
    $.validator.unobtrusive.parse('#storageForm');
    $("#btnRetrieval").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#RetrievalGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnRetrieval').prop("disabled", false) : $('#btnRetrieval').prop("disabled", true);
    });
});

function Retrieval() {
    var commandNo = gridHelper.getSelectedItem("RetrievalGrid").F41_KndCmdNo;
    var preProductCode = gridHelper.getSelectedItem("RetrievalGrid").F41_PreproductCode;
    var lotNo = gridHelper.getSelectedItem("RetrievalGrid").F41_PrePdtLotNo;
    var line = gridHelper.getSelectedItem("RetrievalGrid").Line;
    var tabletisingLine = $('#txtTabletisingLine').val().substring($('#txtTabletisingLine').val().length-2);
    var quanity = gridHelper.getSelectedItem("RetrievalGrid").ThrowAmount;
    //    	If no record selected, the system will show the message as MSG 5.
    if ($("#storageForm").valid()) {
        if (commandNo == undefined) {
            alert("Please select at least one record.");
            return;
        }
        //	If (the value of [Line] is blank) or (the value of [Line] column is not blank
        //and the value of [Tabletising Line] is not equal the value of [Line] column of the selected record on the table),
        //the system will show message as MSG 26.
        if (line !== null && line !== tabletisingLine.replace("TAB0", "")) {
            var errorMessage26 = { Success: false, Message: "The input tablet line does not match the data in the Database." }
            showMessage(errorMessage26);
            return;
        }
        //if (quanity <= 0) {
        //    alert("Pack quantity must be more than zero.");
        //    return;
        //}
        //	The system will check the status of Conveyor and permission of Pre-product as BR 6.
        $.ajax({
            url: formUrl.urlCheckConveyor,
            type: 'POST',
            success: function(data) {
                if (data.Success) {
                    //	If all the above validation passed, the system will show confirmation message as MSG 18.
                    if (confirm("Ready to retrieve?")) {
                        $.ajax({
                            url: formUrl.urlRetrieval,
                            type: 'POST',
                            data: { commandNo: commandNo, preProductCode: preProductCode, lotNo: lotNo, tabletisingLine: tabletisingLine },
                            success: function(data) {

                                if (!data.Success) {

                                    var message = analyzeExceptionMessage(data);
                                    if (message != null)
                                        showMessage(message);
                                    else
                                        showMessage(data);
                                    return;
                                }
                                // Block the UI.
                                if (data.Success) {
                                    blockScreenAccess('Waiting for messages from C2.');
                                }
                            }
                        });
                    } else {
                        $.unblockUI();
                    }

                } else {
                    showMessage(data);
                }
            }
        });
    }
    //	If (the value of [Cont Qty] column of the selected record < 0 on the table, the system will show message as MSG 6.
   
    //todo	If the system has sent message to C2 and C2 has not responded, the system will show message as MSG 23.
    
  
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

        if (message.PictureNo != 'TCIP031F')
            return;

        var item = gridHelper.findSelectedRadioItem("RetrievalGrid");
        if (item == null)
            return;

        // Unblock UI.
        $.unblockUI();

        $.ajax({
            url: "/RetrievalOfPreProduct/ReceiveMessageFromC2",
            type: "post",
            data: {
                kndCmdNo: item.F41_KndCmdNo.trim(),
                prepdtLotNo: item.F41_PrePdtLotNo.trim(),
                preProductCode: item.F41_PreproductCode.trim()
            },
            success: function (response) {
                findC2ResponseInformation(response);
            },
            error: function (response) {
                alert(response.Message);
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();


});