$(document).ready(function () {
    $.validator.unobtrusive.parse('#storeMaterialForm');


});

function Storage() {   
    if ($("#storeMaterialForm").valid()) {
        hideErorr();
        if (!checkMaterialLotNo()) return;
        if ($("#GrandTotal").val() === "0") {
            //$("#errorList").text(message.MSG09).show();
            $("#GrandTotalError").text(message.MSG09).show();
            return;
        }
        if (!checkPackQtyValidity()) return;
        if (!checkFractionValidity()) return;


        var no1 = $("#MaterialLotNo01").val();
        var no2 = $("#MaterialLotNo02").val();
        var no3 = $("#MaterialLotNo03").val();
        var no4 = $("#MaterialLotNo04").val();
        var no5 = $("#MaterialLotNo05").val();
        var arrInput = [no1, no2, no3, no4, no5];
        // var sorted_arr = arrInput.sort();

        for (var i = 0; i < arrInput.length - 1; i++) {
            if (arrInput[i] != '') {
                for (var j = i + 1; j < arrInput.length; j++) {
                    if (arrInput[i] === arrInput[j]) {
                        $('#errorList0' + j).text('This lot-no is been used!').show();
                        return;
                    }
                }
            }
        }        
        
        if (!confirm("Are you sure you want to store?"))
            return;
        $("#storeMaterialForm").submit();
        blockScreenAccess('Waiting for messages from C1');
    }

}

function aa(number) {
    calculateTotal($("#PackUnit" + number).val(), $("#PackQuantity01").val(), $("#Fraction01").val(), $("#Total01"));
}

function PackQuantity(element) {
    calculateTotal($('#PackUnit0' + element).val(), $('#PackQuantity0' + element).val(), $('#Fraction0' + element).val(), $('#Total0' + element));
}

function PackUnit(element) {

    //$('#PackUnit0' + element).change(function () {
    //if ($(this).attr('readonly') === true) {            
    calculateTotal($('#PackUnit0' + element).val(), $('#PackQuantity0' + element).val(), $('#Fraction0' + element).val(), $('#Total0' + element));
    //}
    //});    
}



function checkPackQtyValidity() {
    var packQtyValid = false;
    if ($("#MaterialLotNo01").val() !== "" && $("#PackQuantity01").val() !== "0") {
        packQtyValid = true;
    } else if ($("#MaterialLotNo02").val() !== "" && $("#PackQuantity02").val() !== "0") {
        packQtyValid = true;
    } else if ($("#MaterialLotNo03").val() !== "" && $("#PackQuantity03").val() !== "0") {
        packQtyValid = true;
    } else if ($("#MaterialLotNo04").val() !== "" && $("#PackQuantity04").val() !== "0") {
        packQtyValid = true;
    } else if ($("#MaterialLotNo05").val() !== "" && $("#PackQuantity05").val() !== "0") {
        packQtyValid = true;
    }
    if (!packQtyValid) {
        $("#errorList").text(message.MSG12).show();
        $(".errorInput").addClass("has-error");
        return false;
    }
    return true;
}

function checkFractionValidity() {

    var valid = true;
    if ($("#txtUnit").val() !== "K") {
        if (parseInt($("#Fraction01").val()) >= parseInt($("#PackUnit01").val())) {
            valid = false;
        }
    }
    if (!valid) {
        $("#errorList").text(message.MSG14).show();
        return false;
    }
    return true;
}

function checkMaterialLotNo() {
    if ($("#MaterialLotNo01").val() == "" && $("#MaterialLotNo02").val() == ""
        && $("#MaterialLotNo03").val() == "" && $("#MaterialLotNo04").val() == ""
        && $("#MaterialLotNo05").val() == "") {
        $("#errorList").text(message.MSG08).show();
        $(".errorInput").addClass("has-error");
        return false;
    }
    return true;
}

function hideErorr() {
    $("#errorList").text("").hide();
    $("#GrandTotalError").text("").hide();
    for (var i = 1; i < 6; i++) {
        $('#errorList0' + i).text("").hide();
    }
}

//Calculate Total
function calculateTotal(packUnit, packQuantity, fraction, total) {
    if ($("#txtUnit").val().trim() !== "K") {
        total.val(parseFloat(packUnit) * parseFloat(packQuantity) + parseFloat(fraction));
    } else {
        total.val(parseFloat(packQuantity));
    }

    calculateGrandTotal();
}

//Calculate GrandTotal
function calculateGrandTotal() {


    $('#GrandTotal').val(parseFloat($('#Total01').val()) + parseFloat($('#Total02').val()) + parseFloat($('#Total03').val()) + parseFloat($('#Total04').val()) + parseFloat($('#Total05').val()));
    var tt = $('#GrandTotal').val();
}

//Delete data in record when click button clear
function clearRow(index) {
    $("#MaterialLotNo0" + String(index)).val("");
    //$("#PackUnit0" + String(index)).val(0);
    $("#PackQuantity0" + String(index)).val(0);
    $("#Fraction0" + String(index)).val(0);
    $("#Total0" + String(index)).val(0);
    calculateGrandTotal();
}

function Refresh() {
    location.reload();
}

function onSuccess(data, status, xhr) {
    
    
    //window.setTimeout(function() { location.reload() }, 3000);
    //if (typeof data.Error != "undefined" && data.Error.length) {
    //    $("#errorList").text(data.Error).show();
    //}
    //if (data.Success === "True") {
    //    $("#btnExit").prop("disabled", true);
    //}
    if (!data.Success) {
        showMessage(data, "", "");
        return;
    }
    
    blockScreenAccess('Waiting for messages from C1');

}

function changeMaterialCode() {
    
    var poNo = $("#txtPONoCode").val();
    var materialCode = $("#txtMaterialCode").val();
    var partialDelivery = $("#txtPartialDelivery").val();

    // Send request to server to do material unassignment.
    $.ajax({
        url: "/MaterialManagement/StorageOfMaterial/CheckMaterialCode",
        data: { materialCode: materialCode, poNo: poNo, partialDelivery: partialDelivery },
        type: "GET",
        success: function (data) {
            showNormanMessage(data);
        },
        error: function (data) {
            // Bad request status is responded.
            if (data.status === 400) {
                handleJQueryValidationMessages(data.responseJSON);
            }
        }
    });
}

$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {

        if (message.PictureNo != 'TCRM031F')
            return;

        // Unblock UI.
        $.unblockUI();

        $.ajax({
            url: '/MaterialManagement/StorageOfMaterial/PostStoreMaterial',
            type: 'post',
            success: function (items) {

                if (items == null || !(items instanceof Array))
                    return;
                
                for (var index = 0; index < items.length; index++) {
                    toastr["info"](initiateFirstCommunicationMessage(items[index]));
                }

                // Enable storage button.
                $("#btnStorage").prop("disabled", false);
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();
});
