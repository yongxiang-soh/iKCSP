$(document).ready(function () {
    $("#errorList").text("").hide();

    $.validator.unobtrusive.parse('#indexC1Form');
});

function ReStorage() {
    if (!checkPackQtyValidity())
        return;
    $('#indexC1Form').submit();

}

function onSuccess(data, status, xhr) {
    if (typeof data.Success != "undefined") {
        if (!data.Success) {
            $("#errorList").text(data.Error).show();
        }
    }

    
}

function clearRow(index) {
    $('#MaterialLotNo0' + index).val("");
    //$('#PackUnit0' + index).autoNumeric("set",0);
    $('#PackQuantity0' + index).autoNumeric("set", 0);
    $('#Fraction0' + index).autoNumeric("set", 0);
    //$('#Total0' + index).autoNumeric("set", 0);
}

function Total(index) {
   var parkUnit =   $('#PackUnit0' + index).autoNumeric("get", 0);
   var parkQty=  $('#PackQuantity0' + index).autoNumeric("get", 0);
   var fraction =  $('#Fraction0' + index).autoNumeric("get", 0);
   var total = 
   $('#Total0' + index).autoNumeric("set", 0);
}

function checkPackQtyValidity() {
    var packQtyValid = false;

    for (var index = 0; index < 100; index++) {

        var materialLotNo = "Materials_" + index + "_MaterialLotNo";
        var packQuantity = "Materials_" + index + "_PackQuantity";

        var txtMaterialLotNo = $("#" + materialLotNo).val();
        if (txtMaterialLotNo == null || txtMaterialLotNo.length < 1)
            continue;

        var txtPackQuantity = $("#" + packQuantity).val();
        if (txtPackQuantity == null || txtPackQuantity.length < 1)
            continue;

        packQtyValid = true;
        break;
    }
    
    if (!packQtyValid) {
        $("#errorList1").text(message.MSG12).show();
        $(".errorInput").addClass("has-error");
        return false;
    }
    return true;
}

function PackUnit(element) {          
    calculateTotal($('#PackUnit0' + element).val(), $('#PackQuantity0' + element).val(), $('#Fraction0' + element).val(), $('#Total0' + element));    
}

function PackQuantity(element) {
    calculateTotal($('#PackUnit0' + element).val(), $('#PackQuantity0' + element).val(), $('#Fraction0' + element).val(), $('#Total0' + element));
}

//Calculate Total
function calculateTotal(packUnit, packQuantity, fraction, total) {
    //if ($("#txtUnit").val().trim() !== "K") {
    if ($("#txtUnit").val() !== "K") {
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

function hideFormProdShip() {        
    $('#modalTCRM082F').modal('toggle');
}

onSuccess = function(data) {
    
    if (!data.Success) {
        showMessage(data);
        return;
    }

    $.unblockUI();
    $("#modalTCRM082F").modal("hide");

    showMessage(data);
    blockScreenAccess('Waiting for message from C1...');
}