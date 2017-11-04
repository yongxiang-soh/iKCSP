var isProductSelected = false;

$(document).ready(function () {

    $("#btnSelect").prop("disabled", true);
    $("#btnStorage").prop("disabled", true);
    $('input[type=text]').addClass('removePadding');
    // Handle click on checkbox to set state of "Select all" control
    $('#NormalGrid').on('change', 'input[type="radio"]', function () {
        CheckCountRow();
        // If checkbox is not checked
        //var counterChecked = 0;
        //this.checked ? counterChecked++ : counterChecked--;
        //counterChecked > 0 ? $('#btnSelect').prop("disabled", false) : $('#btnSelect').prop("disabled", true);
        //counterChecked > 0 ? $('#btnStorage').prop("disabled", false) : $('#btnStorage').prop("disabled", true);
    });

    // Handle click on checkbox to set state of "Select all" control
    $('#OutOfPlanGrid').on('change', 'input[type="radio"]', function () {
        CheckCountRow();
        // If checkbox is not checked
        //var counterChecked = 0;
        //this.checked ? counterChecked++ : counterChecked--;
        //counterChecked > 0 ? $('#btnSelect').prop("disabled", false) : $('#btnSelect').prop("disabled", true);
        //counterChecked > 0 ? $('#btnStorage').prop("disabled", false) : $('#btnStorage').prop("disabled", true);
    });

    $('#GridOutOfPlan').hide();
    $("button[type='button']").prop("disabled", true);
    $(".clear-data input[type='text']").prop("disabled", true);
    $("#PalletNo").focus();

    $('#Fraction1').keyup(function () {
        $('#errorFraction1').text("").hide();
        $("#Fraction1").removeClass("input-validation-error");
    })
    $('#Fraction2').keyup(function () {
        $('#errorFraction2').text("").hide();
        $("#Fraction2").removeClass("input-validation-error");
    })
    $('#Fraction3').keyup(function () {
        $('#errorFraction3').text("").hide();
        $("#Fraction3").removeClass("input-validation-error");
    })
    $('#Fraction4').keyup(function () {
        $('#errorFraction4').text("").hide();
        $("#Fraction4").removeClass("input-validation-error");
    })
    $('#Fraction5').keyup(function () {
        $('#errorFraction5').text("").hide();
        $("#Fraction5").removeClass("input-validation-error");
    })
    $('#PackQty1').keyup(function () {
        $('#errorPackQty1').text("").hide();
        $("#PackQty1").removeClass("input-validation-error");
    });
    $('#PackQty2').keyup(function () {
        $('#errorPackQty2').text("").hide();
        $("#PackQty2").removeClass("input-validation-error");
    });
    $('#PackQty3').keyup(function () {
        $('#errorPackQty3').text("").hide();
        $("#PackQty3").removeClass("input-validation-error");
    });
    $('#PackQty4').keyup(function () {
        $('#errorPackQty4').text("").hide();
        $("#PackQty4").removeClass("input-validation-error");
    });
    $('#PackQty5').keyup(function () {
        $('#errorPackQty5').text("").hide();
        $("#PackQty5").removeClass("input-validation-error");
    });
    
});

function ChangeGrid() {
    var code = $('input[name=StorageOfProductStatus]:checked').val();
    if (code === "1") {
        $('#GridNormal').hide();
        $('#GridOutOfPlan').show();
        gridHelper.ReloadGrid("OutOfPlanGrid");
        $("#PalletNo").focus();
    } else {
        $('#GridOutOfPlan').hide();
        $('#GridNormal').show();
        gridHelper.ReloadGrid("NormalGrid");
        $("#PalletNo").focus();
    }
    $('#lstValue').val("");
    $('#valueSelected').val("");
    for (var i = 1; i <= 5; i++) {
        clearRow(i);
    }
    $("#btnSelect").prop("disabled", true);
    $("#PalletNo").removeClass("input-validation-error");
    $("#PalletNo-error").text("").hide();
    $("#OutOfSpec").prop("checked", false);
    $("#PalletNo").val("");
}

function Selected() {

    var productCode = "";
    var commandNo = "";
    var preProductLotNo = "";
    var outOfSpec = $("#OutOfSpec").is(":checked");
    var lstValue = "";
    var valueSelected = "";
    var outOfSpecFlag = "";

    var status = $('input[name=StorageOfProductStatus]:checked').val();
    if (status == 0) {
        outOfSpecFlag = gridHelper.getSelectedItem("NormalGrid").OutofSpecFlag;
    }

    if (status == 1) {
        outOfSpecFlag = gridHelper.getSelectedItem("OutOfPlanGrid").OutofSpecFlag;
    }

    var listValue = $('#lstValue').val().split('#');
    //lstValue = $('#lstValue').val();

    if (status == 0) {
        commandNo = gridHelper.getSelectedItem("NormalGrid").F56_KndCmdNo;
        productCode = gridHelper.getSelectedItem("NormalGrid").F56_ProductCode;
        preProductLotNo = gridHelper.getSelectedItem("NormalGrid").F56_PrePdtLotNo;

        var lstValueNormalGrid = commandNo + ',' + productCode + ',' + preProductLotNo + '#';
        if (listValue.length > 5) {
            var maximumErrorMessage = { Success: false, Message: "Maximum of view is 5 items." }
            showNormanMessage(maximumErrorMessage);
            return;
        }
            
        for (var i = 0; i < listValue.length - 1; i++) {
            if (listValue[i] === lstValueNormalGrid.split('#')[0]) {
                var errorMessageDuplicate = { Success: false, Message: "This product is duplicated by other details!" }
                showNormanMessage(errorMessageDuplicate);
                return;
            }
        }
       
        lstValue = $('#lstValue').val() + lstValueNormalGrid;
        $('#valueSelected').val(lstValueNormalGrid);
        valueSelected = lstValueNormalGrid;
       // $('#lstValue').val(lstValue);

        // GetSelected(lstValue, status);

    } else {
        productCode = gridHelper.getSelectedItem("OutOfPlanGrid").F58_ProductCode;
        preProductLotNo = gridHelper.getSelectedItem("OutOfPlanGrid").F58_PrePdtLotNo;

        var lstValueOutOfPlanGrid = productCode + ',' + preProductLotNo + '#';
        if (listValue.length > 5)
            return;
        for (var i = 0; i < listValue.length - 1; i++) {
            if (listValue[i] === lstValueOutOfPlanGrid.split('#')[0]) {
                var errorMessageDuplicate = { Success: false, Message: "This product is duplicated by other details!" }
                showNormanMessage(errorMessageDuplicate);
                return;
            }
        }
        lstValue = $('#lstValue').val() + lstValueOutOfPlanGrid;
        $('#valueSelected').val(lstValueOutOfPlanGrid);
        valueSelected = lstValueOutOfPlanGrid;
        //GetSelected(lstValue, status);
    }
    //

    if ((outOfSpec === true && outOfSpecFlag === "Normal") || (outOfSpec === false && outOfSpecFlag === "Out of Spec")) {
        var errorMessage = { Success: false, Message: "The selected product's attribute is different from the checkbox \"out-of-spec\"!" }
        showMessage(errorMessage, "", "");

        return;
    }

    if (outOfSpec === true) {
        if (!confirm('Do you make sure the products which will be stored on this pallet are "out-of-spec storage"?'))
            return;
        $("#OutOfSpec").prop("disabled", true);
    } else {
        if (!confirm('Do you make sure the products which will be stored on this pallet are "normal storage"?'))
            return;
        $("#OutOfSpec").prop("disabled", true);
    }
    $('#lstValue').val(lstValue);
    isProductSelected = true;
    CheckCountRow();
    GetSelected(valueSelected, status);
}

function GetSelected(lstValue, status) {    
    var lstindexnull = "";
    for (var i = 1; i <= 5; i++) {
        if ($('#ProductCode' + i).val() == "") {
            lstindexnull = lstindexnull + i + ",";
        }
    }
    var listindexnull = lstindexnull.split(",");
    $.ajax({
        url: formUrl.urlSelected,
        type: 'POST',
        data: { lstValue: lstValue, status: status },
        success: function (response) {
            var count = 0;
            for (var i = 0; i < response.results.length; i++) {

                var result = response.results[i];
                count++;
                if (count > 5) {
                    break;
                }
                if (lstindexnull != "") {
                    assignData(listindexnull[0], result.ProductCode, result.ProductName, result.PreProductLotNo, result.LotNo, commaSeparateNumber(result.PackQty.toFixed(0)), result.Fraction.toFixed(2), result.CommandNo, result.PackUnit, result.TabletingEndDate);
                    var index = listindexnull[0];
                    $("#PackQty" + index).attr('readonly', false);
                    $("#Fraction" + index).attr('readonly', false);
                    listindexnull.splice(0, 1);
                } else {
                    assignData(i + 1, result.ProductCode, result.ProductName, result.PreProductLotNo, result.LotNo, commaSeparateNumber(result.PackQty.toFixed(0)), result.Fraction.toFixed(2), result.CommandNo, result.PackUnit, result.TabletingEndDate);
                var index = i + 1;
                $("#PackQty" + index).attr('readonly', false);
                $("#Fraction" + index).attr('readonly', false);
            }
                
            }
        }
    });
}

function Storage() {
    if (!$("#indexForm").valid())
        return;
    if (!CheckInputData())
        return;
    var palletNo = $('#PalletNo').val();

    var lstValue = $('#lstValue').val();
    if (lstValue == null || lstValue == "") {
        var errorMessage = { Success: false, Message: "At least one line must be valid, please input data!" }
        showMessage(errorMessage, "", "");
        return;
    }


    $.ajax({
        url: formUrl.urlCheckValidation,
        type: 'POST',
        data: { palletNo: palletNo },
        success: function (data) {
            $.unblockUI();
            if (!data.Success) {
                
                showMessage(data, "", "");
                if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                    setTimeout(function () {
                        var errorMessage2 = { Success: false, Message: data.Message2 }
                        showMessage(errorMessage2, "", "");
                    }, 4000);
                }
                return;

            } else {
                $.ajax({
                    url: formUrl.urlGetTotalAmount,
                    type: 'POST',
                    data: { palletNo: palletNo },
                    success: function (data) {
                        if (data.totalAmount > 0) {
                            setTimeout(function () {
                                if (!confirm("The corresponding pallet is out of the warehouse and the data is valid, Ok to store with it?"))
                                    return;
                                if (!confirm("Ready to store?"))
                                    return;
                                $("#OutOfSpec").prop("disabled", false);
                                $('#indexForm').submit();
                            }, 500);

                        } else {
                            setTimeout(function () {
                                if (!confirm("Ready to store?"))
                                    return;
                                $("#OutOfSpec").prop("disabled", false);
                                $('#indexForm').submit();
                            }, 500);
                        }
                           
                       
                    }
                });
            }

         
        }
    });

}


function assignData(i, productCode, productName, preProductLotNo, lotNo, packQty, fraction, commandNo, packUnit, tabletingEndDate) {
    $("#CommandNo" + i).val(commandNo);
    $("#ProductCode" + i).val(productCode);
    $("#ProductName" + i).val(productName);
    $("#PreProductLotNo" + i).val(preProductLotNo);
    $("#LotNo" + i).val(lotNo);
    //if ($("#PackQty" + i).val() == "" || $("#PackQty" + i).val() == packQty) {
     $("#PackQty" + i).val(packQty);
    //}
    if ($("#Fraction" + i))
     $("#Fraction" + i).val(fraction);
    $("#PackUnit" + i).val(packUnit);
    $("#TabletingEndDate" + i).val(tabletingEndDate);
    $("#clear" + i).prop("disabled", false);
}

function clearRow(index) {
    $("#CommandNo" + String(index)).val("");
    $("#ProductCode" + String(index)).val("");
    $("#ProductName" + String(index)).val("");
    $("#PreProductLotNo" + String(index)).val("");
    $("#LotNo" + String(index)).val("");
    $("#PackQty" + String(index)).val("");
    $("#PackQty" + String(index)).removeClass("input-validation-error");
    $("#Fraction" + String(index)).val("");
    $("#Fraction" + String(index)).removeClass("input-validation-error");
    $("#CommandNo" + String(index)).val("");
    $("#PackUnit" + String(index)).val("");
    $("#TabletingEndDate" + String(index)).val("");
    $("#clear" + String(index)).prop("disabled", true);
    $("#errorPackQty" + String(index)).text("").hide();
    $("#errorFraction" + String(index)).text("").hide();
    $("#PackQty" + String(index)).attr('readonly', true);
    $("#Fraction" + String(index)).attr('readonly', true);
    var lstValue = "";
    var status = $('input[name=StorageOfProductStatus]:checked').val();
    if (status == 0) {
        for (var i = 1; i <= 5; i++) {
            if ($("#CommandNo" + i).val() != "" && $("#ProductCode" + i).val() != "" && $("#PreProductLotNo" + i).val() != "") {
                lstValue += $("#CommandNo" + i).val() + ',' + $("#ProductCode" + i).val() + ',' + $("#PreProductLotNo" + i).val() + '#';
            }
        }
    }

    if (status == 1) {
        for (var i = 1; i <= 5; i++) {
            if ($("#ProductCode" + i).val() != "" && $("#PreProductLotNo" + i).val() != "") {
                lstValue += $("#ProductCode" + i).val() + ',' + $("#PreProductLotNo" + i).val() + '#';
            }
        }
    }
    
    //$('.clear-data input[type="text"]').val('');
    //var status = $('input[name=StorageOfProductStatus]:checked').val();
    $("#lstValue").val(lstValue);
    CheckCountRow();
    if (lstValue == null || lstValue == "") {
        $("#btnStorage").prop("disabled", true);
        $("#OutOfSpec").prop("disabled", false);
        return;
    }

}

function CheckCountRow() {
    var listValue = $('#lstValue').val().split('#');
    if (listValue.length > 5) {
        $("#btnSelect").prop("disabled", true);
    } else {
        //if ($('input[name="OutOfPlanGrid-F58_ProductCode"]').is(":checked")) {
            
        //}
        $("#btnSelect").prop("disabled", false);
    }

    if (listValue.length > 1) {
        $("#btnStorage").prop("disabled", false);
    } else {
        $("#btnStorage").prop("disabled", true);
    }
}

function CheckInputData() {
    var isChecked = true;
    for (var i = 1; i <= 5; i++) {
        if ($('#ProductCode' + i).val() !== "" && $('#PreProductLotNo' + i).val() !== "") {
            //if ($("#PalletNo").val() == "" || $("#PalletNo").val() == null) {
            //    $("#errorPalletNo").text("")
            //}
            if (parseFloat($('#PackQty' + i).val()) <= 0) {
                $("#errorPackQty" + i).text("Pack quantity must be more than zero!").show();
                $("#PackQty" + i).addClass("input-validation-error");
                isChecked = false;
            }
            if (parseFloat($('#Fraction' + i).val()) + 0.005 > parseFloat($('#PackUnit' + i).val())) {
                $("#errorFraction" + i).text("Fraction cannot be more than the packing unit!").show();
                $("#Fraction" + i).addClass("input-validation-error");
                isChecked = false;
            }
            if (parseFloat($('#Fraction' + i).val()) <= -0.01) {
                $("#errorFraction" + i).text("Value is out of range!").show();
                $("#Fraction" + i).addClass("input-validation-error");
                isChecked = false;
            }
        }
    }
    return isChecked;
}

function ResetData() {
    for (var index = 1; index < 6; index++) {
        $("#CommandNo" + String(index)).val("");
        $("#ProductCode" + String(index)).val("");
        $("#ProductName" + String(index)).val("");
        $("#PreProductLotNo" + String(index)).val("");
        $("#LotNo" + String(index)).val("");
        $("#PackQty" + String(index)).val("");
        $("#Fraction" + String(index)).val("");
        $("#CommandNo" + String(index)).val("");
        $("#PackUnit" + String(index)).val("");
        $("#TabletingEndDate" + String(index)).val("");
        $("#PackQty" + String(index)).attr('readonly', true);
        $("#Fraction" + String(index)).attr('readonly', true);

    }
}

function onSuccess(data, status, xhr) {

if (data.Success) {
    blockScreenAccess('Waiting message from C3');
    return;
}
    showMessage(data);

    //setTimeout(function () {
    //    if (data.Message.length > 0) {
    //        alert(data.Message);
    //    }
    //}, 500);

    //showMessage(data, "", "");

}

/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        if (message.PictureNo != 'TCPR011F')
            return;

        $.unblockUI();

        // Data hasn't been selected.
        if (!isProductSelected)
            return;

        var storageOfProductStatus = $('input[name="StorageOfProductStatus"]:checked').val();
        // No item has been selected.
        switch (storageOfProductStatus) {
            case '0':
                if (gridHelper.findSelectedRadioItem('NormalGrid') == null)
                    return;
                break;
            case '1':
                if (gridHelper.findSelectedRadioItem('OutOfPlanGrid') == null)
                    return;
                break;
            default:
                return;
        }
        $.ajax({
            url: '/ProductManagement/StorageOfProduct/ProcessThirdCommunicationData',
            type: 'post',
            data: {
                item: findStorageProductItems()
            },
            success: function(response) {
                findC3ResponseInformation(response);
                    gridHelper.searchObject("OutOfPlanGrid", null);
                    gridHelper.searchObject("NormalGrid", null);
                    ResetData();
                    $('#lstValue').val("");
                    $('#valueSelected').val("");
                    $("#PalletNo").val("");
                    $("#OutOfSpec").prop("disabled", false);
                    $("#OutOfSpec").prop("checked", false);
                }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});

/**
 * Find storage of product items which is currently being selected.
 * @returns {} 
 */
findStorageProductItems = function () {
    var item = {};

    for (var i = 1; i < 6; i++) {
        var itemProductCode = 'ProductCode' + i;
        var itemProductName = 'ProductName' + i;
        var itemPreProductLotNo = 'PreProductLotNo' + i;
        var lotNoItem = 'LotNo' + i;
        var itemPackQuantity = 'PackQty' + i;
        var itemFraction = 'Fraction' + i;
        var itemCommandno = 'CommandNo' + i;

        item[itemProductCode] = $('#' + itemProductCode).val();
        item[itemProductName] = $('#' + itemProductName).val();
        item[itemPreProductLotNo] = $('#' + itemPreProductLotNo).val();
        item[lotNoItem] = $('#' + lotNoItem).val();
        item[itemPackQuantity] = $('#' + itemPackQuantity).val();
        item[itemFraction] = $('#' + itemFraction).val();
        item[itemCommandno] = $('#' + itemCommandno).val();
    }

    item['palletNo'] = $('#PalletNo').val().trim();
    item['outOfSpec'] = $('input[type="hidden"][name="OutOfSpec"]').val();
    item['storageOfProductStatus'] = $('input[name="StorageOfProductStatus"]:checked').val();

    return item;
}

function commaSeparateNumber(val) {
    while (/(\d+)(\d{3})/.test(val.toString())) {
        val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
    }
    return val;
}
