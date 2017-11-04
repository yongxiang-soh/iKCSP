$(document).ready(function() {

    $('#btnDeAssign').prop("disabled", true);
    $('#btnReStorage').prop("disabled", true);
    $('input[type="text"]').prop('readonly', true);
    $('[id ^= "Remainder"]').prop('readonly', false);
    $('[id ^= "Fraction"]').prop('readonly', false);
});

function ReStorage() {
    if (!CheckInputData()) {
        $("#errorProductList").text("At least one line must be valid, please input data!").show();
        return;
    }
    $("#errorProductList").text("").hide();
    if (!CheckValidation())
        return;
    for (var i = 1; i <= 5; i++) {
        $("#error-remainder" + String(i)).text("").hide();
        $("#error-fraction" + String(i)).text("").hide();
        
    }
    $.ajax({
        url: formUrl.urlCheckConveyorCode,
        type: 'POST',
        data: { id: null },
        success: function (data) {
            $.unblockUI();
            setTimeout(function() {
                if (!data.Success) {
                    showMessage(data, "", "");
                    return;
                } else {
                    if (!confirm("Ready to store?")) {
                        $.unblockUI();
                        return;
                    }
                    $('#restorageProduct').submit();
                }
            }, 500);

        }
    });

}

function DeAssign() {
    var palletNo = $('#txtPalletNo').val();

    if (!confirm("Do you make sure to delete the product data about this pallet?"))
        return;

    $.ajax({
        url: formUrl.urlDeAssign,
        type: 'POST',
        data: { palletNo: palletNo },
        success: function(data) {
            $('input[type="text"]').val("");
            $('#btnReStorage').prop("disable", "true");
            $('#btnDeAssign').prop("disable", "true");
            showMessage(data, "restorageOfProduct", "");
            window.setTimeout('location.reload()', 3000);
        }
    });
}

function ShowData() {
    for (var i = 1; i <= 5; i++) {
        $("#error-remainder" + String(i)).text("").hide();
        $("#Remainder" + String(i)).removeClass("input-validation-error");
        $("#error-fraction" + String(i)).text("").hide();
        $("#Fraction" + String(i)).removeClass("input-validation-error");
    }

    var palletNo = $('#txtPalletNo').val();
    ResetData();
    $('#txtPalletNo').val(palletNo);
    $.ajax({
        url: formUrl.urlGetData,
        type: "POST",
        data: {
            palletNo: palletNo
        },
        success: function(response) {
            var count = 0;

            for (var i = 0; i < response.results.length; i++) {
                var result = response.results[i];
                count++;
                if (count > 5)
                    break;
                assignData(i + 1, result.F40_ProductCode, result.ProductName, result.F40_PrePdtLotNo, result.F40_ProductLotNo, result.PackUnit.toFixed(2), result.Remainder.toFixed(0).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'), result.Fraction.toFixed(2), result.Total.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,'), result.TabletingEndDate, result.F40_CertificationFlg, result.CertificationDate, result.AddDate);
            }
            if (count > 0) {
                $('#btnDeAssign').prop("disabled", false);
                $('#btnReStorage').prop("disabled", false);
            }
        }
    });
}

function assignData(i, productCode, productName, preProductLotNo, lotNo, packUnit, remainder, fraction, total, endDate, certificationFlag, certificationDate, addDate) {
    $("#ProductCode" + i).val(productCode);
    $("#ProductName" + i).val(productName);
    $("#PreProductLotNo" + i).val(preProductLotNo);
    $("#LotNo" + i).val(lotNo);
    $("#PackUnit" + i).val(packUnit);
    $("#Remainder" + i).val(remainder);
    $("#Fraction" + i).val(fraction);
    $("#Total" + i).val(total);
    $("#EndDate" + i).val(endDate);
    $("#CertificationFlag" + i).val(certificationFlag);
    $("#CertificationDate" + i).val(certificationDate);
    $("#AddDate" + i).val(addDate);
}

//Delete data in record when click button clear
function clearRow(index) {
    $("#ProductCode" + String(index)).val("");
    $("#ProductName" + String(index)).val("");
    $("#PreProductLotNo" + String(index)).val("");
    $("#LotNo" + String(index)).val("");
    $("#PackUnit" + String(index)).val("0.00");
    $("#Remainder" + String(index)).val("");
    $("#Fraction" + String(index)).val("0.00");
    $("#Total" + String(index)).val("0.00");
    $("#error-remainder" + String(index)).text("").hide();
    $("#error-fraction" + String(index)).text("").hide();
    $("#errorProductList").text("").hide();
    $("#Remainder" + String(index)).removeClass("input-validation-error");
    $("#Fraction" + String(index)).removeClass("input-validation-error");

}

function onSuccess(data) {
    $('input[type="text"]').val("");
    console.log('-- Restorage of product --');
    console.log(data);
    if (data.Success) {
        blockScreenAccess('Waiting for messages from C3...');
        return;
    }

    showMessage(data, "", "");
    //window.setTimeout('location.reload()', 3000);
}

function ResetData() {
    for (var i = 1; i <= 5; i ++) {
        clearRow(i);
    }
}

function CheckInputData() {
    var ischecked = false;
    for (var i = 1; i < 6; i++) {
        var productCode = $("#ProductCode" + i).val();
        if (productCode != null && productCode != "") {
            ischecked = true;
            break;
        }
    }
    return ischecked;
}


function CheckValidation() {

    var isChecked = true;
    for (var i = 1; i <= 5; i++) {
        if (($("#ProductCode" + String(i)).val() !== null && $("#ProductCode" + String(i)).val() !== "" && $("#PreProductLotNo" + String(i)).val() !== null && $("#PreProductLotNo" + String(i)).val() !== "")) {
            if (parseFloat($("#Remainder" + String(i)).val()) <= 0) {
                $("#error-remainder" + String(i)).text("Pack quantity must be more than zero!").show();
                $("#Remainder" + String(i)).addClass("input-validation-error");
                isChecked = false;
            }

            if (parseFloat($("#Fraction" + String(i)).val()) + 0.005 > parseFloat($("#PackUnit" + String(i)).val()) || parseFloat($("#Fraction" + String(i)).val()) < -0.005) {
                $("#error-fraction" + String(i)).text("At least one line must be valid, please input data!").show();
                $("#Fraction" + String(i)).addClass("input-validation-error");
                isChecked = false;
            }
        }
    }

    if (!isChecked)
        return false;
    return true;
}


/**
 * Callback which is called when document is ready.
 */
$(function() {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function(message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR071F')
            return;
        
        // Construct parameter.
        var parameter = {};

        var properties = ['ProductCode', 'PreProductLotNo', 'LotNo', 'PackUnit', 'Remainder', 'Fraction', 'Total', 'EndDate', 'CertificationFlag', 'CertificationDate', 'AddDate'];

        for (var propertyIndex = 0; propertyIndex < properties.length; propertyIndex++) {

            var propertyName = properties[propertyIndex];

            for (var index = 1; index < 6; index++) {

                var fullPropertyName = propertyName + index;
                parameter[fullPropertyName] = $('#' + fullPropertyName).val();
            }
        }

        $.ajax({
            url: '/RestorageOfProduct/Reply',
            type: 'post',
            data: parameter,
            success: function(data) {
                if (!(data instanceof Array))
                    return;

                for (var index = 0; index < data.length; index++) {
                    toastr["info"](initiateThirdCommunicationResponseMessage(data[index]));
                }
            },
            done: function (data) {
                $.unblockUI();
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});
