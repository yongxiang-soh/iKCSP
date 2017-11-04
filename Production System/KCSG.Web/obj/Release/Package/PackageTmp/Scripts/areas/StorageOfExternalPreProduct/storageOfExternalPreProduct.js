$(document).ready(function() {
    $('#Quantity').keyup(function() {
        var hasValue = this.value.length > 0;
        $("#quantity-error").text("").hide();
        $("#Quantity").removeClass("input-validation-error");
    });
});

function CheckQuantityTextBox() {
    if (parseFloat($("#Quantity").val()) <= 0) {
        $("#quantity-error").text("Pack quantity must be more than zero!").show();
        $("#Quantity").addClass("input-validation-error");
        $("#Quantity").val("");
        return false;
    }
    return true;
}

function Storage() {

    if (!$('#indexForm').valid())
        return;
    if (!CheckQuantityTextBox())
        return;

    var preProductCode = $('#txtPreProductCode').val();
    var lotNo = $('#LotNo').val();
    var palletNo = $('#PalletNo').val();
    $.ajax({
        url: "/ProductManagement/StorageOfExternalPreProduct/CheckKneadingClassAndStatus",
        type: 'POST',
        data: { preProductCode: preProductCode, lotNo: lotNo, palletNo: palletNo },
        success: function(data) {
            if (!data.Success) {
                if (data.Message === "MSG44") {
                    var message44 = { Success: false, Message: "Input lot-no is error!" }
                    showMessage(message44, "", "");
                    $("#LotNo").val("");
                }
                if (data.Message === "MSG45") {
                    var message45 = { Success: false, Message: "Input pallet-no is error!" }
                    showMessage(message45, "", "");
                    $("#PalletNo").val("");
                }
            } else {
                $.ajax({
                    url: "/ProductManagement/StorageOfExternalPreProduct/GetRemainingAmount",
                    type: 'POST',
                    data: { palletNo: palletNo },
                    success: function (data) {
                        $.unblockUI();
                        setTimeout(function() {
                            if (data < 0.005) {
                                if (confirm("There is some product in the pallet, do you want to continue?")) {
                                    $.ajax({
                                        url: "/ProductManagement/StorageOfExternalPreProduct/DeleteProductShelfStock",
                                        type: 'POST',
                                        data: { palletNo: palletNo },
                                        success: function(data) {
                                            $.unblockUI();
                                        }
                                    });
                                } else {
                                    return;
                                }

                            }
                            $.ajax({
                                url: "/ProductManagement/StorageOfExternalPreProduct/CheckOutSidePreProductStockStatus",
                                type: 'POST',
                                data: { palletNo: palletNo },
                                success: function(data) {
                                    if (!data.Success) {
                                        showMessage(data, "", "");
                                        $("#PalletNo").val("");
                                    } else {
                                        $.unblockUI();
                                        setTimeout(function() {
                                            $('#indexForm').submit();
                                           }, 500);
                                    }
                                }
                            });
                        }, 1000);
                    }
                });

            }
        }
    });

}

function LotEnd() {
    if (!$('#indexForm').valid())
        return;
    var lotNo = $('#LotNo').val();
    var preProductCode = $('#txtPreProductCode').val();
    if (lotNo != null && preProductCode != null) {
        $.ajax({
            url: formUrl.urlLotEnd,
            type: 'POST',
            data: { lotNo: lotNo, PreProductCode: preProductCode },
            success: function(data) {
                showMessage(data, "", "");
                $('#indexForm')[0].reset();
            }
        });
    }
}

function onSuccess(data, status, xhr) {
    
    if (!data.Success) {
        $.unblockUI();
        if (data.Message.length > 0) {
            if (data.Message[0] === "MSG8") {
                var message08 = { Success: false, Message: "Conveyor status error!" }
                showMessage(message08);
                return;
            }
            if (data.Message[0] === "MSG9") {
                var message09 = { Success: false, Message: "Warehouse status error!" }
                showMessage(message09);
                return;
            }
            if (data.Message[0] === "MSG19") {
                alert("There is no empty location available in the warehouse now!");
                return;
            }
        }

        return;
    }
    $("#Kndcmdno").val(data.KndNo);
    
    $('#btnLotEnd').prop("disabled", false);
    blockScreenAccess('Waiting messages from C3');
}

function ResetData() {
    //$("#txtPreProductCode").val("");
    //$("#txtPreproductName").val("");
    //$("#LotNo").val("");
    //$("#PalletNo").val("");
    //$("#Quantity").val("0.00");
}


/**
 * Callback which is called when document is ready.
 */
$(function() {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function(message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR091F')
            return;
        if ($("#LotNo").val() === "") {
            return;
        }

        $.unblockUI();

        // Respond from C3.
        $.ajax({
            url: '/StorageOfExternalPreProduct/RespondMessageC3',
            type: 'post',
            data: {
                lotNo: $("#LotNo").val(),
                preProductCode: $("#txtPreProductCode").val(),
                Kndcmdno : $("#Kndcmdno").val()
            },
            success: function(response) {
                ResetData();
                // Invalid response.
                if (response == null)
                    return;

                // Response is not an array of message.
                if (!(response instanceof Array))
                    return;

                // Messages list is empty.
                if (response.length < 1)
                    return;

                for (var i = 0; i < response.length; i++)
                    toastr["info"](initiateThirdCommunicationResponseMessage(response[i], 'External Pre-product', 'External Pre-product Code'));
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});
