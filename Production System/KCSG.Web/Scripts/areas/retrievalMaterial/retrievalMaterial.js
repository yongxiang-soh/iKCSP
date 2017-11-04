/**
 * This function is called when 1 Pallet button / Assign button is clicked.
 * @returns {} 
 */
assignPallet = function(buttonType) {
    
    // Prevent default behaviour of button.
    event.preventDefault();

    // Required paramters are blank.
    if (!$("#formRetrievalOfMaterial").valid())
        return;

    
    var value = $('input[name=CommWthMeasureSys]:checked').val();    
    if (value !== "0" &&
        value !== "1" &&
        value !== "2" &&
        value !== "3") {
        return;
    }
    
    $.ajax({
        url: "/RetrieveOfMaterial/RetrieveOrAssignPallet",
        type: "post",
        data: {
            materialCode: findMaterialCode(),
            requestedRetrievalQuantity: findRequestedRetrievalQuantity()
        },        
        success: function (response) {
            var a = response.result.IsSuccess;
            if (!response.result.IsSuccess) {
                var message = response.result.ErrorMessages[0];
                if (message === "MSG43-MSG47") {

                    var notificationMessage = "";
                    // 1 Pallet button is clicked.
                    if (buttonType == 1)
                        notificationMessage = "No in-stock shelf found in the location range!";
                    else
                        notificationMessage = "There is not pallet that has been assigned!";

                    alert(notificationMessage);
                }
                return false;
            }
            var result = response.result.Data;
            if (result == null)
                return;

            $("#txtTally").val(result.Tallet);

            // Initiate a message which will be displayed on screen.
            var notification = { Success: true, Message: "" };

            // 1 Pallet button is clicked.
            if (buttonType == 0) {
                //	If all actions above are success, the system will show information message MSG 44 in case button “1 Pallet” is pressed; 
                //notification.Message = "There is one pallet that has been assigned. The material amount is " + result.Tallet;
                notification.Message = "One pallet is assigned!";
            } else {
                //show message MSG 45 if there is one pallet which is assigned 
                if (result.Total == 1) {
                    notification.Message = "There is one pallet that has been assigned. The material amount is  " +
                        result.Tallet;
                } else {
                    notification.Message = "There is a " + result.Total + " pallet that has been assigned. The material amount is " + result.Tallet + "."
                }

            }

            // Display notification.
            alert(notification.Message);

            // 	Disable button “1 Pallet”, “Assign”.
            $("#btnOnePallet").prop("disabled", true);
            $("#btnAssign").prop("disabled", true);
            $(".input-group-addon").removeAttr("data-target");  
            //	Set all fields as read-only
            //$("#formRetrievalOfMaterial input[type!=='button']").prop("disabled", true);

            //	Enable button “De-assign”, “Retrieve” and “Detail”
            $("#btnRetrieval").prop("disabled", false).removeAttr("disabled");
            $("#btnUnassginMaterials").prop("disabled", false).removeAttr("disabled");
            $("#btnDetail").prop("disabled", false).removeAttr("disabled");
        }   
    });
}

/**
 * This function is for unassigning material by using specific conditions.
 * @returns {} 
 */
unassignMaterial = function() {

    // Validate the information first.
    if (!$("#formRetrievalOfMaterial").valid())
        return;

    $.ajax({
        url: "/RetrieveOfMaterial/UnassignPallets",
        type: "post",
        data: {
            materialCode: findMaterialCode(),
            requestedRetrievalQuantity : findRequestedRetrievalQuantity()
        },
        success: function (response) {
             
            // Set Tally to blank
            $("#txtTally").val("");

            //	Enable button “1 Pallet”, “Assign”.
            $("#btnOnePallet").prop("disabled", false);
            $("#btnAssign").prop("disabled", false);

            //	Enable all fields.
            $("#RequestedRetrievalQuantity").prop("disabled", false);
            
            //	Disable button “De-assign”, “Retrieve”, “Re-store” and “Detail
            $("#btnUnassginMaterials").prop("disabled", true);
            $("#btnRetrieval").prop("disabled", true);
            $("#btnRestorage").prop("disabled", true);
            $("#btnDetail").prop("disabled", true);


            //	Disable panelPalletsList
            $("#panelPalletsList").hide();

            //Enable field MaterialCode Search
            $('.input-group-addon[data-toggle="modal"]').attr('data-target', '#modalMaterialCodeSelect');
        }
    });
}

/**
 * Find pallets list by using specific conditions.
 * @returns {} 
 */
findPalletsList = function() {

    var parameters = {
        materialCode: findMaterialCode(),
        requestedRetrievalQuantity: findRequestedRetrievalQuantity()
    };

    gridHelper.searchObject("DetailPallet", parameters);

    $("#panelPalletsList").show();
}

/**
 * Find selected communication with measurement system.
 * @returns {} 
 */
findCommunicationWithMeasureSystem = function() {
    return $("input[name='CommWthMeasureSys']:checked").val();
}

/**
 * Find requested retrieval quantity.
 * @returns {} 
 */
findRequestedRetrievalQuantity = function() {
    return $("#RequestedRetrievalQuantity").val();
}

/**
 * Find selected material code.
 * @returns {} 
 */
findMaterialCode = function() {
    return $("#txtMaterialCode").val();
}

/**
 * This event is fired when one detail pallet grid has been initiated.
 * @returns {} 
 */
OnDetailPalletGridInitiated = function() {

    /**
     * This event is fired when a row in detail pallet is clicked on.
     */
    $("#DetailPallet tr")
        .click(function(event) {

            // Prevent default behaviour.
            //event.preventDefault();

            // Find the selected item in the grid.
            var chosenItem = gridHelper.findSelectedRadioItem("DetailPallet");

            if (chosenItem == null)
                return;

            // Hide the containers.
            $("#col-material-no").hide();
            $("#col-quantity").hide();

            $.ajax({
                url: "/RetrieveOfMaterial/FindPalletDetails",
                type: "post",
                data: {
                    palletNo: chosenItem.F31_PalletNo
                },
                success: function(response) {
                    $("#materialLotNoContainer").html("");
                    $("#quantityContainer").html("");

                    var materialsInnerHtml = "";
                    var quantityInnerHtml = "";

                    for (var i = 0; i < response.results.length; i++) {
                        var pallet = response.results[i];
                        materialsInnerHtml += "<input class='form-control form-group' readonly='readonly' value='" +
                            pallet.F33_MaterialLotNo +
                            "'/>";
                        quantityInnerHtml += "<input class='form-control form-group' readonly='readonly' value='" +
                            pallet.F33_Amount +
                            "'/>";
                    }

                    $("#materialLotNoContainer").html(materialsInnerHtml);
                    $("#quantityContainer").html(quantityInnerHtml);

                    $("#col-material-no").show();
                    $("#col-quantity").show();

                    $('#btnUnassignSpecificPallet').removeClass("hidden");


                }
            })

        });

}

/**
 * This function is for unassigning a specific pallet by using specific information.
 * @param {} event 
 * @returns {} 
 */
unassignSpecificPallet = function(event) {

    if (!confirm("Are you sure you want to de-assign?"))
        return;

    // Find the selected item in the grid.
    var chosenItem = gridHelper.findSelectedRadioItem("DetailPallet");

    if (chosenItem == null)
        return;
    
    $.ajax({
        url: "/RetrieveOfMaterial/UnassignPallet",
        type: "post",
        data: {
            shelfRow: chosenItem.F31_ShelfRow,
            shelfBay: chosenItem.F31_ShelfBay,
            shelfLevel: chosenItem.F31_ShelfLevel
        },
        success: function () {
            // Update grid.
            findPalletsList();

            // Recalculate tally amount.
            calculateTally();
        }
    });
}

cancelAssignMaterialRetrieval = function(event) {
    $("#panelPalletsList").hide();
}

/**
 * This function is for re-updating tally.
 * @returns {} 
 */
calculateTally = function() {
    $.ajax({
        url: "/RetrieveOfMaterial/CalculateTally",
        type: "post",
        data: {
            materialCode: findMaterialCode()
        },
        success: function(response) {
            if (response.tally != null)
                $("#txtTally").val(response.tally);
        }
    });
}

retrievalPallets = function(event) {
    
    // Prevent button's default behaviour.
    event.preventDefault();

    if (!confirm("Are you sure you want to retrieve?"))
        return;


    $.ajax({
        url: "/RetrieveOfMaterial/RetrievePallets",
        type: "post",
        data: {
            materialCode: findMaterialCode(),
            requestedRetrievalQuantity: findRequestedRetrievalQuantity()
        },
        success: function (data) {

            if (!data.Success) {
                var message = analyzeExceptionMessage(data);
                showMessage(message);
                return;
            } else {
                //	After all “Actionable Items” is processed, the system will disable all buttons in this screen
                $("#btnUnassginMaterials").prop("disabled", true);
                $("#btnRetrieval").prop("disabled", true);
                $("#btnDetail").prop("disabled", true);

                blockScreenAccess("Waiting messages from C1...");
            }
            
        },
        error: function (response) {
            var message = response.message;

            if (message == "MSG15") {
                alert("Conveyor status error!");
            } else if (message == "MSG16") {
                alert("Warehouse status error!");
            }
        }

    });
}

function OnSuccess(data, status, xhr) {

    if (data.Success) {
        $("#formPlaceholder").html(data);
        blockScreenAccess("Waiting for messages from C1...");
        return;
    }

    showMessage(data);
}
function CheckPallet() {
    $.ajax({
        url: "/RetrieveOfMaterial/CheckAssignPallet",
        type: "get",
        data: {
            materialCode: findMaterialCode()
          },
        success: function(data) {
            if (data.Checked) {
              $("#btnDetail").prop("disabled", false);
              $("#btnUnassginMaterials").prop("disabled", false);
              $("#btnRetrieval").prop("disabled", false);
                $("#txtTally").val(data.Tally);
            }
        }
    });
}
$(function () {
    
   
    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM071F')
            return;

        var materialCode = $("#txtMaterialCode").val();
        if (materialCode == null || materialCode.length < 1)
            return;

        $.ajax({
            url: '/MaterialManagement/RetrieveOfMaterial/PostRetrieveMaterial',
            type: 'post',
            data: {
                materialCode: materialCode
            },
            success: function (response) {

                if (response == null || !(response instanceof Array))
                    return;

                for (var index = 0; index < response.length; index++)
                    toastr["info"](initiateFirstCommunicationMessage(response[index]));

                $('#btnRestorage').prop('disabled', false)
                    .removeClass('disabled')
                    .removeAttr('disabled');
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();
});

/**
 * Event which is fired when restorage button is clicked.
 * @returns {} 
 */
clickRestorage = function() {
    window.location.replace(urlRestorageOfMaterial);
}
