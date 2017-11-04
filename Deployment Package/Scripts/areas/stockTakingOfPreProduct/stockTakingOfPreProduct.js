$(document).ready(function () {
    $('#btnRetrieve').prop("disabled", true);
   $("#ShelfNoFrom").val('01-01-01');
    $("#ShelfNoTo").val('02-08-07');
    $("#ShelfNoFrom").mask("99-99-99");
    $("#ShelfNoTo").mask("99-99-99");

    $('#StockTakingPreProduct').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnRetrieve').prop("disabled", false) : $('#btnRetrieve').prop("disabled", true);
    });



});

$(function () {

    $("#ShelfNoFrom").mask("99-99-99");
    $("#ShelfNoTo").mask("99-99-99");
    $("#btnRetrieve").prop("disabled", true);
    //Confirm with Retrieval of PreProduct Container
    $("#ShelfNo").mask("99-99-99");

    $("#StockTakingPreProduct")
        .click(function () {

            // Check the radio button.
            $("#StockTakingPreProduct")
                .find('.jsgrid-row.jsgrid-selected')
            .find('input[type="radio"]').prop('checked', true);

            var item = gridHelper.getSelectedItem('StockTakingPreProduct');
            if (item != null) {
                $("#btnRetrieve").prop("disabled", false);
            }
        });

});

//int fromRow, int fromBay, int fromLevel, int toRow, int toBay, int toLevel
/**
 * This function is for searching stocking storage of pre-product on the server.
 * @returns {} 
 */
search = function () {
    
    if ($("#indexForm").valid()) {
        var shelfNoFrom = analyzeShelfNo($("#ShelfNoFrom").val());
        var shelfNoTo = analyzeShelfNo($("#ShelfNoTo").val());

        if (!validateShelfNo(shelfNoFrom, shelfNoTo)) return;

        $("#errorlist").text("").hide();

        var request = {
            fromRow: shelfNoFrom[0],
            fromBay: shelfNoFrom[1],
            fromLevel: shelfNoFrom[2],
            toRow: shelfNoTo[0],
            toBay: shelfNoTo[1],
            toLevel: shelfNoTo[2]
        }

        gridHelper.searchObject("StockTakingPreProduct", request);
    }
}

function clearvalue() {
    $("#ShelfNoFrom").val("01-01-01");
    $("#ShelfNoTo").val("02-08-07");
    $("#errorlist").hide();
    gridHelper.searchObject("StockTakingPreProduct", null);
    $('#btnRetrieve').prop('disabled', true);

}
/**
 * This function is for retrieving item information from the grid above.
 * @returns {} 
 */
retrieve = function () {

    if (!confirm("Ready to retrieve?"))
        return;

    // Find the selected item in the grid.
    //var item = $("#StockTakingPreProduct").find("jsgrid-row").find('input[type="radio"]:checked').first();

    //// Find the item index.
    //var itemIndex = item.parent().parent().index();
    //if (itemIndex < 0) {
    //    $("#btnRetrieve").prop("disabled", true);
    //    return;
    //}
    var shelfNo = gridHelper.getSelectedItem("StockTakingPreProduct").ShelfNo;
    var preProductCode = gridHelper.getSelectedItem("StockTakingPreProduct").F49_PreProductCode;
    var containerCode = gridHelper.getSelectedItem("StockTakingPreProduct").F49_ContainerCode;
    var containerNo = gridHelper.getSelectedItem("StockTakingPreProduct").F37_ContainerNo;
    var updateDate = moment(gridHelper.getSelectedItem("StockTakingPreProduct").F37_UpdateDate).format('YYYY-MM-DD HH:mm:ss.SSS');

    // Find the pre-product.
    //var itemPreProduct = gridHelper.getListingData("StockTakingPreProduct")[itemIndex];
    $.ajax({
        url: "/PreProductManagement/StockTakingPreProduct/Retrieve",
        contentType: "application/json",
        method: "post",
        data: JSON.stringify({
            preProductCode: preProductCode,
            lsRow: shelfNo.substring(0, 2),
            lsBay: shelfNo.substring(3, 5),
            lsLevel: shelfNo.substring(6, 8),
            containerCode: containerCode,
            containerNo: containerNo,
            updateDate: updateDate
        }),
        success: function(response) {
            $("#isconvcode").val(response.convcode);
            if (response.Success) {
                $("#btnRetrieve").prop("disabled", true);

                // Block the UI.
                blockScreenAccess('Waiting for messages from C2.');
            } else {
                showMessage(response, "", "");
            }
        }
    });

}
/**
 * This function is for analyzing shelf number and split it into 3 parts.
 * @returns {} 
 */
analyzeShelfNo = function (shelfNo) {

    // Parse strings to int.
    var parts = shelfNo.split("-")
        .map(function (x) {
            return parseInt(x);
        });

    return parts;
}

function validateShelfNo(shelfNoFrom, shelfNoTo) {
    if (!validateShelfIntValue(shelfNoFrom) || !validateShelfIntValue(shelfNoTo)) {
        //resetShelves();
        $('#errorlist').text(validateMessage.MSG49).show();
        return false;
    }

    var row1 = parseInt(shelfNoFrom[0]);
    var bay1 = parseInt(shelfNoFrom[1]);
    var level1 = parseInt(shelfNoFrom[2]);
    var row2 = parseInt(shelfNoTo[0]);
    var bay2 = parseInt(shelfNoTo[1]);
    var level2 = parseInt(shelfNoTo[2]);
    if (row1 > row2) {

        $("#errorlist").text("\"From location\" is bigger than \"To location\"!").show();
        return false;
    } else if (bay1 > bay2 && row1 === row2) {

        $("#errorlist").text("\"From location\" is bigger than \"To location\"!").show();
        return false;
    } else if (level1 > level2 && row1 === row2 && bay1 === bay2) {

        $("#errorlist").text("\"From location\" is bigger than \"To location\"!").show();
        return false;
    }
    return true;
}

function validateShelfIntValue(shelf) {
    var row = parseInt(shelf[0]);
    var bay = parseInt(shelf[1]);
    var level = parseInt(shelf[2]);
    if (row < 1 || row > 2 || bay < 1 || bay > 8 || level < 1 || level > 7) {
        return false;
    }
    return true;
}
function resetShelves() {
    $("#ShelfNoFrom").val("01-01-01").change();
    $("#ShelfNoTo").val("02-08-07").change();
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

        if (message.PictureNo == 'TCIP041F') {

            var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");
            if (item == null)
                return;
            var shelfNo = parseShelfNo(item.ShelfNo);
            var preProductCode = item.F49_PreProductCode;
            var containerCode = item.F49_ContainerCode;
            // Unblock UI.
            $.unblockUI();

            $.ajax({
                url: "/StockTakingPreProduct/ReceiveMessageFromC2ServerForStoring",
                type: "post",
                data: {
                    shelfNo: shelfNo, preProductCode: preProductCode, containerCode: containerCode
                },
                success: function (response) {
                   // Analyze and display response messages.
                    findC2ResponseInformation(response);
                    // Find the selected item on grid and update the confirm panel.
                    if (response[0].F50_Status == '6') {
                        $("#RetrievalShelfNo").val(item.ShelfNo);
                        $("#PreProductCode").val(item.F49_PreProductCode);
                        $("#ContainerType").val(item.F37_ContainerType);
                        // Display the lower table.
                        $("#TCIP043F").show();
                    } if (response[0].F50_Status == '9') {
                        gridHelper.searchObject("StockTakingPreProduct","");
                    }

                }
            });
        } else if (message.PictureNo == 'TCIP043F') {

            
            var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");

            if (item == null)
                return;
            // Unblock UI.
            $.unblockUI();
            $.ajax({
                url: "/StockTakingPreProduct/AnalyzeStockTakingForMoving",
                type: "post",
                data: {
                    containerCode: $("#isconvcode").val(),
                    lchStatus: $("#LchStatus").val(),
                    okClicked: $("#OkClicked").val(),
                    preProductCode :item.F49_PreProductCode
                },
                success: function (response) {

                    if (response == null || !(response instanceof Array))
                        return;

                    var ls_FrShelf = $("#RetrievalShelfNo").val().replace(/-/g, "");
                    // Analyze and display response messages.
                    findC2ResponseInformation(response);
                    
                    if ($("#OkClicked").val()) // OK button clicked
                    {
                        $("#ShelfNo").val(item.ShelfNo);
                        $("#PreProductCode,#txtPreProductCode").val(item.F49_PreProductCode);
                        $("#ContainerType,#txtContainerType").val(item.F37_ContainerType);
                        $("#txtPreProductName").val(item.F03_PreProductName);
                        $("#ContainerNo").val(item.F37_ContainerNo);
                        $("#LotNo").val(item.F49_PreProductLotNo);
                        $("#ContainerCode").val(item.F49_ContainerCode);

                        if (response[0].F50_From != null && ls_FrShelf == response[0].F50_From) {
                            if (response[0].F50_Status == '6') {
                                
                                $("#TCIP042F").show();
                                //$("#TCIP043F").hide();
                                $("#btnNg").prop('disabled', true);
                                $("#btnRetrieveOk").prop('disabled', true);
                                $('#TCIP043F input').prop('disabled', true);
                            }
                        }
                    } else {
                        if (ls_FrShelf == response[0].F50_To) {
                            if (response[0].F50_Status == '6') {
                                $("#TCIP042F").show();
                                //$("#TCIP043F").hide();
                                $("#btnNg").prop('disabled', true);
                                $("#btnRetrieveOk").prop('disabled', true);
                                $('#TCIP043F input').prop('disabled', true);
                            }
                        }
                    }
                    
                },
                error: function(response) {
                    alert(response.Message);
                }
            });
        } else if (message.PictureNo == 'TCIP042F') {

            var confirmItem = gridHelper.findSelectedRadioItem("StockTakingPreProduct");
            if (confirmItem == null)
                return;
            var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");

            if (item == null)
                return;
            // Unblock UI.
            $.unblockUI();

            $.ajax({
                url: "/StockTakingPreProduct/ReceivingMessageFromC2",
                type: "post",data: {
                    preProductCode: item.F49_PreProductCode
                },
                success: function(response) {

                    if (response && (response instanceof Array)) {

                        for (var index = 0; index < response.length; index++) {
                            response[index].PreProductCode = $('#txtPreProductCode').val();
                        }
                        // Analyze and display response messages.
                        findC2ResponseInformation(response);
                    }

                    $('#TCIP042F input').prop('disabled', true);
                    $('#btnRestorage').prop('disabled', true);
//                    $("#TCIP042F").hide();
                }
            });
        }
    }

    // Start connection to hub.
    signalrStartHubConnection();


});

ok = function() {

    // Save the state of clicked button.
    $("#OkClicked").prop('checked', true);
    if (confirm("Ready to retrieve?")) {
        $("#btnNg").prop('disabled', true);
        // Find the selected item on grid.
        var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");

        if (item == null)
            return;

        // Parse shelf no.
        var infos = parseShelfNo(item.ShelfNo);
        if (infos == null)
            return;
        console.log(item);
        $.ajax({
            url: "/StockTakingPreProduct/RetrievePreProductContainerOk",
            type: "post",
            async: false,
            data: {
                row: infos[0],
                bay: infos[1],
                level: infos[2],
                containerCode: item.F49_ContainerCode,
                containerNo: item.F37_ContainerNo,
                containerType: item.F37_ContainerType
            },
            success: function (response) {
                if (response.Success) {
                    $("#btnNg").prop('disabled', false);
                }
            }
        });
        //blockScreenAccess('Waiting for messages from C2.');
        blockScreenAccess('Conveyor From [CV214] To '+$("#isconvcode").val()+' moving…');
    }
}

cancel = function() {
    // Save the state of clicked button.
    $("#OkClicked").prop('checked', false);
    if (confirm("Ready to retrieve?")) {
        $("#btnRetrieveOk").prop('disabled', true);
        
        // Find the selected item on grid.
        var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");

        if (item == null)
            return;

        // Parse shelf no.
        var infos = parseShelfNo(item.ShelfNo);
        if (infos == null)
            return;

        $.ajax({
            url: "/StockTakingPreProduct/RetrievePreProductContainerNg",
            type: "post",
            async: false,
            data: {
                row: infos[0],
                bay: infos[1],
                level: infos[2],
                containerCode: item.F49_ContainerCode,
                containerNo: item.F37_ContainerNo
            },
            success: function (response) {
                // Find item.
                
                var confirmItem = gridHelper.findSelectedRadioItem("StockTakingPreProduct");
                if (confirmItem == null)
                    return;

                $("#btnRetrieveOk").prop('disabled', false);
               
             }
        });
        //blockScreenAccess('Waiting for messages from C2.');
        var ls_message = "";
        var as_shelfno = $("#RetrievalShelfNo").val();
        var containerType = $("#ContainerType").val();
        if (containerType == "0") {
            ls_message = "Shelf No [" + as_shelfno + "] storing ...";
        } 
        if (containerType == "2") {
            ls_message = "Shelf No [" + as_shelfno + "] retrieving...";
        }
        if (containerType == "3") {
            ls_message = "Shelf No [" + as_shelfno + "] retrieving...";
        }
        blockScreenAccess(ls_message);
    }
 
}

function onSuccess() {
     blockScreenAccess('Waiting for messages from C2.');
}
printLabel = function() {

    // find the selected item on the grid.
    var item = gridHelper.findSelectedRadioItem("StockTakingPreProduct");
    if (item == null)
        return;

    $.ajax({
        url: url.print,
        type: 'post',
        data: {
            commandNo: item.F49_KneadingCommandNo,
            preProductCode: item.F49_PreProductCode,
            preProductName: item.F03_PreProductName,
            lotNo: item.F49_PreProductLotNo,
            quantity: $("#Quantity").autoNumeric("get"),
            storageSeqNo: item.F49_Amount,
            containerCode: item.F49_ContainerCode
        },
        success: function() {
            $.unblockUI();
        }
    });
}
