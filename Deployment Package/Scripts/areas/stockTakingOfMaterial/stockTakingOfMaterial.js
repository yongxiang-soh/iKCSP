var selectedItem = null;

$(document)
    .ready(function() {
    $("#btnRetrieve").prop("disabled", true);

    $("#ShelfNoFrom").mask("99-99-99");
    $("#ShelfNoTo").mask("99-99-99");

    $("#errorList").text("").hide();

    $('#MaterialShelfGrid').on('change', 'input[type="radio"]', function () {
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        if (counterChecked > 0) {
                        $("#btnRetrieve").prop("disabled", false);
        } else {
                        $("#btnRetrieve").prop("disabled", true);
        }
    });

    $("body").on("click", "#MaterialShelfGrid tr ", function () {
        getMaterialStock();
    });
   
});

function loadDataSuccess() {
    //$("#MaterialShelfGrid").find("input").each(function (parameters) {
    //    $(this).c(function () {
    //        alert("b");
    //    });
    //});
}

function getMaterialStock(value) {
    var palletNo = gridHelper.getSelectedItem("MaterialShelfGrid").F33_PalletNo;
    $.ajax({
        url: formUrl.urlGetMaterialStock,
        type: "POST",
        data: { palletNo: palletNo },
        success: function (data) {
            for (var i = 0; i <= 5; i++) {
                assignData(i + 1, "", "");//clear data
            }
           
            if (typeof data != "undefined" && data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (data.length <= 5) {
                       
                        assignData(i + 1, data[i].F33_MaterialLotNo, data[i].F33_Amount);
                    }
                }
            }
        }
    });
}

function search() {
    if ($("#indexForm").valid()) {
        var shelfNoFrom = $("#ShelfNoFrom").val().split("-").join("");
        var shelfNoTo = $("#ShelfNoTo").val().split("-").join("");

        
        if (!validateShelfNo(shelfNoFrom, shelfNoTo)) return;

        $("#errorList").text("").hide();

        var request = {
            ShelfNoFrom: shelfNoFrom,
            ShelfNoTo: shelfNoTo
        };
        gridHelper.searchObject("MaterialShelfGrid", request);
    }
}

retrieve = function(){
    
    if (!confirm(validateMessage.MSG41))
        return;

    var item = gridHelper.findSelectedRadioItem("MaterialShelfGrid");
    if (item == null)
        return;

    selectedItem = item;

    // Data construction.
    var data = {
        firstRowShelfNo: item.F31_ShelfRow + "" + item.F31_ShelfBay + "" + item.F31_ShelfLevel,
        firstRowPalletNo: item.F33_PalletNo,
        firstRowMaterialCode: item.F33_MaterialCode,
        firstRowMaterialName: item.F01_MaterialDsp,
        currentRowPalletNo: item.F33_PalletNo
    };

    $.ajax({
        url: formUrl.urlRetrieve,
        type: "POST",
        data: data,
        success: function(data) {

            if (data.Success) {
                    //$("#btnSearch").prop("disabled", true);
                $("#btnRetrieve").prop("disabled", true);

                // Block the UI.
                blockScreenAccess('Waiting messages from C1');

                //search();
                return;
            } 

            showMessage(data, "", "");
        }
    });

}

function Clear() {
    var items = gridHelper.getListingData("MaterialShelfGrid");

    items.splice(0);
    $("#MaterialShelfGrid").jsGrid("option", "data", items);
    $("#MaterialShelfGrid").find('.jsgrid-pager-container').hide();

    //clear MaterialLotNo
    $("#MaterialLotNo01").val("");
    $("#MaterialLotNo02").val("");
    $("#MaterialLotNo03").val("");
    $("#MaterialLotNo04").val("");
    $("#MaterialLotNo05").val("");

    //clear Quantity
    $("#Amount01").val("");
    $("#Amount02").val("");
    $("#Amount03").val("");
    $("#Amount04").val("");
    $("#Amount05").val("");
}

//$("#gridArea").dblclick(function () {


function assignData(i, matLotNo, amount) {
    $("#MaterialLotNo0" + i).val(matLotNo);
    $("#Amount0" + i).val(amount);
}

function validateShelfNo(shelfNoFrom, shelfNoTo) {
    if (!validateShelfIntValue(shelfNoFrom) || !validateShelfIntValue(shelfNoTo)) {
        resetShelves();
        return false;
    }
    var shelfNoFromIntVal = parseInt(shelfNoFrom);
    var shelfNoToIntVal = parseInt(shelfNoTo);
    if (shelfNoFromIntVal > shelfNoToIntVal) {
        $("#errorList").text(validateMessage.MSG50).show();
        return false;
    }
    return true;
}

function validateShelfIntValue(shelf) {
    
    var row = parseInt(shelf.substring(0, 2));
    var bay = parseInt(shelf.substring(2, 4));
    var level = parseInt(shelf.substring(4, 6));
    if (row < 1 || row > 2 || bay < 1 || bay > 13 || level < 1 || level > 8) {
        $("#errorList").text(validateMessage.MSG49).show();
        return false;
    }
    return true;
}

function resetShelves() {
    $("#ShelfNoFrom").val("01-01-01").change();
    $("#ShelfNoTo").val("02-13-08").change();
}

function onSuccess(data, status, xhr) {
    showMessage(data, "", "");
}

$(function() {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function(message) {

        if (message.PictureNo == "TCRM081F") {
            // Unblock UI.
            $.unblockUI();
            selectedItem = gridHelper.getListingData("MaterialShelfGrid")[0];
            if (selectedItem == null)
                return;

            $.ajax({
                url: "/MaterialManagement/StockTakingOfMaterial/PostRetrieveMaterial",
                type: "post",
                data: {
                    materialCode: selectedItem.F33_MaterialCode.trim()
                },
                success: function(data) {

                    // Invalid response.
                    if (data == null || !(data instanceof Array))
                        return;

                    // Refresh the screen.
                    //refresh();

                    var isOn = false;

                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        toastr["info"](initiateFirstCommunicationMessage(item));

                        if (!isOn) {
                            if (item.F34_Status == 'C') {

                                isOn = true;

                                // Display TCRM082F
                                displayTCRM082F();
                            }
                        }
                    }
                }
            });
        } else if (message.PictureNo == "TCRM082F") {
            $.unblockUI();
            selectedItem = gridHelper.getSelectedItem("MaterialShelfGrid");
            if (selectedItem == null)
                return;

            $.ajax({
                url: "/MaterialManagement/StockTakingOfMaterial/CompleteStoraging",
                type: "post",
                data: {
                    materialCode: selectedItem.F33_MaterialCode.trim()
                },
                success: function (data) {

                    // Invalid response.
                    if (data == null || !(data instanceof Array))
                        return;

                    // Refresh the screen.
                    //refresh();

                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        toastr["info"](initiateFirstCommunicationMessage(item));
                    }
                }
            });
        }
    };
    
    // Start connection to hub.
    signalrStartHubConnection();
});

displayTCRM082F = function() {
    
    selectedItem = gridHelper.getSelectedItem("MaterialShelfGrid");

    // Find the selected material.
    if (selectedItem == null)
        return;

    $.ajax({
        url: "/MaterialManagement/StockTakingOfMaterial/IndexC1",
        method: "post",
        data: {
            //shelfNo: material.ShelfNo,
            shelfNo: selectedItem.ShelfNo,
            palletNo: selectedItem.F33_PalletNo,
            materialCode: selectedItem.F33_MaterialCode,
            materialDsp: selectedItem.F01_MaterialDsp,
            palletNo: selectedItem.F33_PalletNo
            //palletNo: material.F33_PalletNo,
            //materialCode: material.F33_MaterialCode,
            //materialDsp: material.F01_MaterialDsp
        },
        success: function(response) {
            $("#modalTCRM082F").find(".modal-content").first().empty().html(response);
            $("#modalTCRM082F").modal("show");
        }
    });
};




refresh = function() {

    var items = ["MaterialLotNo0", "Amount0"];

    for (var itemIndex = 0; itemIndex < items.length; itemIndex++) {
        for (var i = 0; i < 5; i++) {
            $('#' + items[itemIndex] + i).val("");
        }
    }

 
    // Reload grid.
    var gridItems = gridHelper.getListingData("MaterialShelfGrid");
    gridItems = [];
    $("#MaterialShelfGrid").jsGrid("option", "data", gridItems);
}