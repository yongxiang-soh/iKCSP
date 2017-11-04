var isUiBlock = false;
$(document).ready(function () {

    $("#btnDetail").prop("disabled", true);
    $("#btnDeAllAssign").prop("disabled", true);
    $("#btnRetrieval").prop("disabled", true);
    $("#btnDeAssign").prop("disabled", true);


    $('#TempTableGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDeAssign').prop("disabled", false) : $('#btnDeAssign').prop("disabled", true);
    });

});
$("body").on("click", "#TempTableGrid tr", function () {
    getProductDetails();
});

function getProductDetails() {
    ResetData();
    var productCode = $('#txtProductCode').val();
    var palletNo = gridHelper.getSelectedItem("TempTableGrid").F51_PalletNo;
    $.ajax({
        url: formUrl.urlProductDetail,
        type: 'POST',
        data: { productCode: productCode, palletNo: palletNo },
        success: function (data) {
            if (typeof data != "undefined" && data != null) {
                for (var i = 0; i < data.length; i++) {
                    if (data.length <= 5) {
                        assignData(i + 1, data[i].F40_ProductCode, data[i].ProductName, data[i].F40_ProductLotNo, data[i].F40_AssignAmount.toFixed(2).replace(/./g, function (c, i, a) {
                            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
                        }), data[i].Flag);
                    }
                    //$("#btnDeAssign").prop("disabled", false);
                }
                
            }

            if (data.length <= 0) {
                setTimeout(function() {
                    var errorMesssage01 = { Success: false, Message: "Cannot find corresponding records from database!" }
                    showMessage(errorMesssage01, "", "");
                    return;
                });

            }
        }
    });
}
function assignData(i, productCode, productName, lotNo, quantity, cerFlag) {
    $("#ProductCode" + i).val(productCode);
    $("#ProductName" + i).val(productName);
    $("#LotNo" + i).val(lotNo);
    $("#Quantity" + i).val(quantity);
    $("#CerFlag" + i).val(cerFlag);
}


//event occurs when click button 1 Pallet
//Refer UC 19 for more information
function Pallet(index) {
    var productCode = $('#txtProductCode').val();
    var productLotNo = $('#txtProductLotNo').val();
    var requestRetrievalQuantity = $('#RequestedRetrievalQuantity').autoNumeric("get");
    var isPallet = true;

    if (index === 2) {
        isPallet = false;
    }

    if (!$('#indexForm').valid())
        return;

    $.ajax({
        url: formUrl.urlCheckRecord,
        type: 'POST',
        data: { productCode: productCode, productLotNo: productLotNo },
        success: function (data) {
            $.unblockUI();
            setTimeout(function () {
                if (!data.Success) {
                    $('#txtProductLotNo').val("");
                    setTimeout(function () {
                        showNormanMessage(data);
                    }, 1000);

                } else {

                    if (!confirm("Ready to assign?"))
                        return;

                    

                    $.ajax({
                        url: formUrl.urlGetTally,
                        type: 'POST',
                        data: {
                            productCode: productCode,
                            productLotNo: productLotNo,
                            isPallet: isPallet,
                            requestRetrievalQuantity: requestRetrievalQuantity
                        },
                        async: false,
                        success: function (data) {
                                
                            if (isPallet) {
                                $("#RequestedRetrievalQuantity").val("");
                            }
                            if (!isPallet) {
                                $.unblockUI();
                                setTimeout(function() {
                                    if (data > 0) {
                                        alert("Assigned quantity is not enough!");
                                    }
                                },1000);
                                    
                            }
                            var request = {
                                productCode: productCode,
                                productLotNo: productLotNo,
                                isPallet: isPallet,
                                requestRetrievalQuantity: requestRetrievalQuantity
                            };
                            gridHelper.searchObject("TempTableGrid", request);

                            //$('#Tally').val(data.tally.toFixed(2));
                        }
                    });
                }
            }, 500);
        }
    });

}

function GetTally() {
    var results = gridHelper.getListingData("TempTableGrid");
    var tally = 0;
    for (var i = 0; i < results.length; i++) {
        tally += results[i].RemainingAmount;
    }
    if (tally > 0) {
        $("#btnDetail").prop("disabled", false);
        $("#btnDeAllAssign").prop("disabled", false);
        $("#btnRetrieval").prop("disabled", false);
        $("#btnPallet").prop("disabled", true);
        $("#btnAssign").prop("disabled", true);
        $("#btnRestorage").prop("disabled", true);
        $("#RequestedRetrievalQuantity").attr('readonly', true);
        $(".input-group-addon").removeAttr("data-target");
        $("#Tally").val(tally.toFixed(2).replace(/./g, function (c, i, a) {
            return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
        }));
    }
    
}

function Detail() {
    var items = gridHelper.getListingData("TempTableGrid");
    if (items.length == 0) {
        errorMessage = { Success: false, Message: "Cannot find corresponding records from database!" }
        showMessage(errorMessage);
        return;
    }

    $("#detail-product").show();
}
//event occurs when click button De-assign
//Refer UC 22 for more information  
function DeAssign() {
    if (!confirm("Ready to de-assign?"))
        return;
    var rowIndex = $("#TempTableGrid").find("td > input[type='radio']:checked").parent().parent().index();
    var shelfNo = gridHelper.getSelectedItem("TempTableGrid").ShelfNo;
    var palletNo = gridHelper.getSelectedItem("TempTableGrid").F51_PalletNo;
    var tally = gridHelper.getSelectedItem("TempTableGrid").RemainingAmount.toFixed(2);
    $.ajax({
        url: formUrl.urlDeAssign,
        type: 'POST',
        data: { shelfNo: shelfNo, palletNo: palletNo },
        success: function (data) {
            if (data.Success) {
                var newTally = ($("#Tally").autoNumeric("get") - tally).toFixed(2).replace(/./g, function (c, i, a) {
                    return i && c !== "." && ((a.length - i) % 3 === 0) ? ',' + c : c;
                });
                ResetData();
                deleteGridItem(rowIndex);
                $("#Tally").val(newTally);
                $("#btnDeAssign").prop("disabled", true);
            }

            $.unblockUI();
        }
    });
}


deleteGridItem = function (index) {
    var items = gridHelper.getListingData("TempTableGrid");
    itemRemove = items.splice(index, 1);
    $("#TempTableGrid").jsGrid("option", "data", items);
}
//event occurs when click button De-assign
//Refer UC 23 for more information 
function DeAllAssign() {
    if (!confirm("Ready to de-assign?"))
        return;
    var productCode = $('#txtProductCode').val();
    var productLotNo = $('#txtProductLotNo').val();
    var lstResultGrid = gridHelper.getListingData("TempTableGrid");
    var lstPalletNo = "";

    lstResultGrid.forEach(function (result, index) {
        var isChecked = false;
        var palletNo = result.F51_PalletNo;
        var arrPalletNo = lstPalletNo.split('-');
        if (arrPalletNo.length < 2) {
            lstPalletNo += palletNo + '-';
        } else {
            for (var i = 0; i < arrPalletNo.length - 1; i++) {
                if (palletNo === arrPalletNo[i]) {
                    isChecked = true;
                    break;
                }
            }
            if (!isChecked) {
                lstPalletNo += palletNo + '-';
            }
        }

    });
    $.ajax({
        url: formUrl.urlDeAllAssign,
        type: 'POST',
        data: { productCode: productCode, productLotNo: productLotNo, lstPalletNo: lstPalletNo },
        success: function (data) {
            $('#Tally').val("0");
            $("#btnDetail").prop("disabled", true);
            $("#btnDeAllAssign").prop("disabled", true);
            $("#btnRetrieval").prop("disabled", true);
            $("#btnPallet").prop("disabled", false);
            $("#btnAssign").prop("disabled", false);
            $("#btnRestorage").prop("disabled", false);
            $("#RequestedRetrievalQuantity").attr('readonly', false);

            $("#product_code_div .input-group-addon").attr("data-target", "#modalProductCodeSelect");
            $("#product_lotno_div .input-group-addon").attr("data-target", "#modalProductLotNoSelect");

            var items = gridHelper.getListingData("TempTableGrid");
            itemRemove = items.splice(0);
            $("#TempTableGrid").jsGrid("option", "data", items);

            $("#detail-product").hide();

            $.unblockUI();
        }
    });
}


function Retrieval() {
    if ($("#txtProductCode").valid() & $("#txtProductLotNo").valid()){
        
    var productCode = $('#txtProductCode').val();
    var productLotNo = $('#txtProductLotNo').val();
    var lstGridItem = gridHelper.getListingData("TempTableGrid");

    $.ajax({
        url: "/ForcedRetrievalOfProduct/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            //$.unblockUI();
            if (!data.Success) {
                setTimeout(function () { showMessage(data, "", ""); }, 500);
                //if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                //    setTimeout(function () {
                //        var errorMessage2 = { Success: false, Message: data.Message2 }
                //        showMessage(errorMessage2, "", "");
                //    }, 4000);
                //}
                //return;

            } else {
                $.ajax({
                    url: formUrl.urlCheckRecordForRetrievalButton,
                    type: 'POST',
                    data: { productCode: productCode, productLotNo: productLotNo },
                    success: function (data) {
                        if (!data.Success) {
                            showMessage(data, "", "");

                            $('#txtProductLotNo').val("");
                            return;
                        } else {
                            $.ajax({
                                url: formUrl.urlCheckStatus,
                                type: 'POST',
                                data: { id: null },
                                success: function (data) {
                                    if (!data.Success) {
                                        showMessage(data, "", "");
                                        return;
                                    }
                                    $.unblockUI();
                                    if (!confirm("Ready to retrieve?")) {
                                        return;
                                    }

                                    $.ajax({
                                        url: formUrl.urlRetrieval,
                                        type: 'POST',
                                        data: { lstGridItem: lstGridItem },
                                        success: function (data) {
                                            showMessage(data, "", "");
                                            if (data.Success) {
                                                $("#btnDetail").prop("disabled", true);
                                                $("#btnDeAllAssign").prop("disabled", true);
                                                $("#btnRetrieval").prop("disabled", true);
                                                $("#btnPallet").prop("disabled", false);
                                                $("#btnAssign").prop("disabled", false);
                                                $("#btnRestorage").prop("disabled", false);
                                            }

                                            blockScreenAccess('Waiting message from C3');
                                        }
                                    });
                                }
                            });
                        }
                    }
                });
            }
        }
    }); 
    }
}

function Restorage() {
    var isChecked = true;
    var palletNo = null;
    var productCode = $('#txtProductCode').val();
    var productLotNo = $('#txtProductLotNo').val();
    $.ajax({
        url: "/ForcedRetrievalOfProduct/Restorage",
        type: "post",
        data: {
            productCode: productCode,
            productLotNo: productLotNo
        },
        success: function (response) {
            palletNo = response.results;
            var url = "/ProductManagement/RestorageOfProduct/Index?palletNo=" + palletNo + "&isChecked=" + isChecked;
            window.location.href = url;
        }
    });
}
function ResetData() {
    for (var i = 1; i < 6; i++) {
        $("#ProductCode" + i).val("");
        $("#ProductName" + i).val("");
        $("#LotNo" + i).val("");
        $("#Quantity" + i).val("0.00");
        $("#CerFlag" + i).val("");
    }
}

function ResetForm() {
    $("#txtProductCode").val("");
    $("#txtProductName").val("");
    $("#txtProductLotNo").val("");
    $("#RequestedRetrievalQuantity").val("0.00");
    $("#Tally").val("0.00");
    $("#product_code_div .input-group-addon").attr("data-target", "#modalProductCodeSelect");
    $("#product_lotno_div .input-group-addon").attr("data-target", "#modalProductLotNoSelect");
    $("#RequestedRetrievalQuantity").attr('readonly', false);
}

/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR051F')
            return;

        var productCode = $("#txtProductCode").val();
        if (productCode == null || productCode.length < 1)
            return;

        $.unblockUI();

        // Respond from C3.
        $.ajax({
            url: '/ForcedRetrievalOfProduct/RespondReplyFromC3',
            type: 'post',
            data: {
                productCode: productCode
            },
            success: function (response) {

                // Invalid response.
                if (response == null)
                    return;

                // Response is not an array of message.
                if (!(response instanceof Array))
                    return;

                // Messages list is empty.
                if (response.length < 1)
                    return;

                
                for (var i = 0; i < response.length; i++) {
                    $("#btnRestorage, #btnAssign, #btnPallet").prop('disabled', false);
                    toastr["info"](initiateThirdCommunicationResponseMessage(response[i]));
                    ResetForm();
                }
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});
