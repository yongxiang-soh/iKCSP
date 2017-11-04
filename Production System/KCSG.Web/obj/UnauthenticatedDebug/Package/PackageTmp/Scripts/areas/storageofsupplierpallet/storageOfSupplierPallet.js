$().ready(function() {
    $('#MaterialGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnRetrieve').prop("disabled", false) : $('#btnDelete').prop("btnRetrieve", true);
        
    });
    $("#txtSupplierCode").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $('#IncrementOfPallet').keyup(function () {
        $("#lstError").text("").hide();
        $("#IncrementOfPallet").removeClass("input-validation-error");
    });
});


function ClearSearch() {
    $("#txtSupplierCode").val("");
    $("#txtMaxPallet").val("0");
    $("#txtSupplierName").val("");
    gridHelper.searchObject("MaterialGrid");
}

function edit(shelfRow, shelfBay, shelfLevel) {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: {
            shelfRow: shelfRow,
            shelfBay: shelfBay,
            shelfLevel: shelfLevel
        },
        success: function (data) {
            //showMessage(data, "MaterialGrid", "");
            if (data.Success) {
                blockScreenAccess("Waiting messages from C1...");
            }
         }
    });
}
function hideForm() {
    $("#formPlaceholder").html("");
}

function search() {
    
    if (!checkvalidation())
        return;


    var request = {
        SupplierCode: $("#txtSupplierCode").val().trim(),
        MaxPallet: $("#txtMaxPallet").val().trim()
    };
    gridHelper.searchObject("MaterialGrid", request);
}

function checkvalidation() {
    var checked = true;
    if ($('#txtSupplierCode').val() === "") {
        $("#errorPONo").text("Please input data for this required field.").show();
        //$(".errorInput").addClass("has-error");
        checked = false;
    }    
    if (checked === false)
        return false;
    return true;
}

function RetrieveItem() {
    debugger;
    var id = $('#MaterialGrid').find('input:checked').val();
    if (confirm("Are you sure you want to retrieve?")) {
        var res = id.split('~');
        var shelfRow = res[0];
        var shelfBay = res[1];
        var shelfLevel = res[2];

        edit(shelfRow, shelfBay, shelfLevel);
    };
}

$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM121F' && message.PictureNo!='TCRM122F')
            return;

        if (message.PictureNo == 'TCRM121F') {


            $.ajax({
                url: '/MaterialManagement/StorageOfSupplierPallet/StorageOfSupplierPalletMessageC1Reply',
                type: 'post',
                data: {
                    // materialCode: materialCode
            
                },
                success: function(response) {
                    debugger; 
                    if (response == null || !(response instanceof Array))
                        return;

                    for (var index = 0; index < response.length; index++) {
                        toastr["info"](initiateFirstCommunicationMessage(response[index]));
                        if (response[index].OldStatus == "6") {
                            showDetailStorageOfSupplierPalletScreen();
                        } else {
                            setTimeout(function () {
                                ClearSearch();
                            }, 3000);
                        }
                    }


                }
            });
        } else {
            $.ajax({
                url: '/MaterialManagement/StorageOfSupplierPallet/DeatailStorageOfSupplierPalletMessageC1Reply',
                type: 'post',
                data: {
                    // materialCode: materialCode

                },
                success: function (response) {

                    if (response == null || !(response instanceof Array))
                        return;

                    for (var index = 0; index < response.length; index++) {
                        toastr["info"](initiateFirstCommunicationMessage(response[index]));
                        setTimeout(function() {
                            ClearSearch();
                            ResetDetailOfSupplierPalletStorageForm();
                        }, 3000);
                    }
                }
            });
        }
    }

    // Start connection to hub.
    signalrStartHubConnection();
});

function showDetailStorageOfSupplierPalletScreen() {
    debugger;
    var supplierCode = $("#txtSupplierCode").val();
    var supplierName = $("#txtSupplierName").val();
    var maxPallet = $("#txtMaxPallet").val();
    var shelfNo = gridHelper.getSelectedItem("MaterialGrid").ShelfNo;
    var stackedPallet = gridHelper.getSelectedItem("MaterialGrid").StockedPallet;
    $("#txtDetailShelfNo").val(shelfNo);
    $("#txtDetailSupplierCode").val(supplierCode);
    $("#txtDetailSupplierName").val(supplierName);
    $("#MaxPalletDetail").val(maxPallet);
    $("#StackedPallet").val(stackedPallet);

    $("#detail-supplier-pallet").show();

}


function DetailStorage() {
    var shelfNo = $("#txtDetailShelfNo").val();
    var txtDetailSupplierCode = $("#txtDetailSupplierCode").val();
    var maxPalletDetail = $("#MaxPalletDetail").autoNumeric('get');
    var stackedPalletDetail = $("#StackedPallet").autoNumeric('get');
    var incrementOfPallet = $("#IncrementOfPallet").autoNumeric('get');
    if (parseInt(stackedPalletDetail) + parseInt(incrementOfPallet) > parseInt(maxPalletDetail)) {
        $("#lstError").text("The final amount of the pallet must not be greater than max number pallet allow!").show();
        $("#IncrementOfPallet").addClass("input-validation-error");
    } else {
        $.ajax({
            url: "/StorageOfSupplierPallet/DetailStorage",
            type: 'post',
            data: {
                shelfNo: shelfNo,
                supplierCode: txtDetailSupplierCode,
                stackedPallet: stackedPalletDetail,
                incrementOfPallet: incrementOfPallet
            },
            success: function (data) {
                if (data.Success) {
                    blockScreenAccess("Waiting messages from C1...");

                    
                }
            }
        });
    }
}

function ResetDetailOfSupplierPalletStorageForm() {
    $("#txtDetailShelfNo").val("");
    $("#txtDetailSupplierCode").val("");
    $("#txtDetailSupplierName").val("");
    $("#MaxPalletDetail").val("");
    $("#StackedPallet").val("");
    $("#IncrementOfPallet").val("");
    $("#detail-supplier-pallet").hide();
}



