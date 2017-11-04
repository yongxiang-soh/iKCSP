/**
 * This function is called when materials list has been assigned successfully.
 * @returns {} 
 */
materialAssignedSuccesfully = function (data) {
    
    //	Disable button “Assign”.
    $("#btnAssign").attr("disabled", "disabled");

    //	Enable button “Retrieve” and “De-assign”.
    $("#btnRetrieve").removeAttr("disabled");
    $("#btnDeAssign").removeAttr("disabled");

    //	Set field [Material Code] as read-only.
    $("#txtMaterialCode").next().removeAttr("data-target");

    //	Set [Assigned] as number of processed records, starting from 0.
    $("#Assigned").val(data.Assigned);

    //	Set [Assigned Quantity] as total [f31_amount] of processed records
    $("#txtAssignedQuantity").val(data.AssignedQuantity);
}

/**
 * This function is for submitting material search information and do their assignment.
 * @returns {} 
 */
assignMaterials = function() {

    if ($("#materialSearchForm").valid()) {
        $.ajax({
            url: "/MaterialManagement/ForcedRetrievalOfRejectedMaterial/Assign",
            data: $("#materialSearchForm").serialize(),
            type: "POST",
            success: function (data) {
               
                materialAssignedSuccesfully(data);
            },
            error: function(data) {
                
                // Bad request status is responded from server. 
                if (data.status === 400) {
                    // Display validation message on screen as available.
                    handleJQueryValidationMessages(data.responseJSON);

                    // Display process error as available.
                    var processError = data.getResponseHeader("x-process-error");
                    if (processError != null && processError.length > 0)
                        alert(processError);

                    return;
                }
            }
        });
    }
}

/**
 * This function is for submitting material search information and do their rejection.
 * @returns {} 
 */
rejectMaterials = function() {
    if ($("#materialSearchForm").valid()) {
        $.ajax({
            url: "/MaterialManagement/ForcedRetrievalOfRejectedMaterial/Reject",
            data: $("#materialSearchForm").serialize(),
            type: "POST",
            success: function (data) {
                //	Enable button “Assign”.
                $("#btnAssign").removeAttr("disabled").removeAttr("readonly");

                //	Disable button “De-assign” and “Retrieve”.
                $("#btnDeAssign").attr("disabled", "disabled");

                //	Set field [Material Code] as editable
                $("#txtMaterialCode").next().attr("data-target","#modalMaterialCodeSelect");

                //	Clear value of all other fields.
                $("#materialSearchForm input").val("");
                $("#Assigned").val("0");
                $("#txtAssignedQuantity").val("0");

                $("#btnRetrieve").attr("disabled", "disabled");
            }
           
        });
    }
}

retrieveMaterials = function() {

    if ($("#materialSearchForm").valid()) {

        //Display confirmation dialog MSG 41.
        if (!confirm("Are you sure you want to retrieve?"))
            return;

        $.ajax({
            url: "/MaterialManagement/ForcedRetrievalOfRejectedMaterial/Retrieve",
            data: $("#materialSearchForm").serialize(),
            type: "POST",
            success: function (data) {
                if (data.Success) {
                    //	After all, the system will disable button “De-assign” and “Retrieval”.
                    //	Disable button “De-assign” and “Retrieve”.
                    $("#btnDeAssign").attr("disabled", "disabled");

                    //	Set field [Material Code] as editable
                    $("#btnRetrieve").removeAttr("disabled").removeAttr("readonly");

                    // Block the UI.
                    blockScreenAccess('Waiting messages from C1.');
                }
                else {
                    showMessage(data);
                }
            }
            
        });
    }
}


$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM151F')
            return;

        // Find material code.
        var materialCode = $("#txtMaterialCode").val();

        if (materialCode == null || materialCode.length < 1)
            return;

        materialCode = materialCode.trim();

        // Unblock the UI.
        $.unblockUI();

        $.ajax({
            url: '/MaterialManagement/ForcedRetrievalOfRejectedMaterial/PostProcessRejectedMaterial',
            type: 'post',
            data: {
                materialCode: materialCode
            },
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
