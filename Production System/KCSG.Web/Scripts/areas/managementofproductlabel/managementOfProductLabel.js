$(document).ready(function() {
    var externalLabel = parseInt($("#ExternalLabel option:selected").val());

    if (externalLabel === 2) {
        $('#LotNoIDY').prop('checked', true);
    }
    $('#Quantity').keyup(function() {
        var hasValue = this.value.length > 0;
        $("#errorlst").text("").hide();
        $("#Quantity").removeClass("input-validation-error");
    });
    $("#dvMfgDate").on('dp.change', function() { ChangeDate(); });
});
//$("body").bind("change", "#ShelfLife", function() {
//    ChangeDate();
//});

$("body").bind("change", "#CodeLabel", function() {
    var result = $('#CodeLabel option:selected').text();
    if (result === "No") {
        $("#SpecificCodeLabel").prop('readonly', true);
    } else {
        $("#SpecificCodeLabel").prop('readonly', false);
    }
});

function ChangeData() {
    var externalLabel = parseInt($("#ExternalLabel option:selected").val());
    var codeLabel = parseInt($("#CodeLabel option:selected").val());
    if (externalLabel === 3 || codeLabel === 1) {
        $('#LotNoIDY').prop('checked', true);
    } else {
        $('#LotNoIDY').prop('checked', false);
    }


    var internalLabelOption = $('#InternalLabel').val();
    var externalLabelOption = $('#ExternalLabel').val();
    var partNo = $('label[for="ScsPartNo"]');
    if (internalLabelOption == '10' || internalLabelOption == '11' || externalLabelOption == '7') {
        partNo.text('STS Part No.');
    } else if (internalLabelOption == '12' || internalLabelOption == '13' || externalLabelOption == '8') {
        partNo.text('Renasas Part No.');
    } else {
        partNo.text('SCS Part No.');
    }
}

function ChangeDate() {
    var shelfLife = $("#ShelfLife").val();
    var mfgDate = $("#MfgDate").val();
    if (mfgDate !== "") {
        var lstMfgDate = mfgDate.split('/');
        mfgDate = lstMfgDate[1] + '/' + lstMfgDate[0] + '/' + lstMfgDate[2];
        var shelfLifeNumber = 0;
        if (shelfLife !== "") {
            shelfLifeNumber = parseInt(shelfLife);
        }
        console.log(shelfLifeNumber);
        var result = new Date(mfgDate);
        result.setMonth(result.getMonth() + shelfLifeNumber);
        var a = moment(result).format("DD/MM/YYYY");
        $("#Expired").val(a);
    }
}

function Clear() {
    $("#formExternalLabel input[type=text]").val("");
    $("#formExternalLabel input[type=number]").val("");
    $("#formInternalLabel input[type=text]").val("");
    $("#formInternalLabel input[type=number]").val("");
    $(".clearData").val("");
    $('#LotNoIDY').prop('checked', false);
    $('#SmallFont').prop('checked', false);
    $('#KAP').prop('checked', false);
    $('#BarcodeOfExpireDate').prop('checked', false);
    $("#CmdNo").val("");
    $("#txtProductCode").val("");
}


function Print() {

    if (getExternalPrintOption() == '1' && getInternalPrintOption() == '1' && getATMPrintOption() == '0') {
        return;
    }

    //$("#addNewForm").submit();
    if ($("#CmdNo").valid() & $("#txtProductCode").valid() & $("#ShelfLife").valid() & $("#Pieces").valid()) {

        if (isStsPartNoSolid()) {
            $('#Pieces-error').addClass('hidden');
        } else {
            $('#Pieces-error').removeClass('hidden');
            return;
        };

        var quatity = parseFloat($("#Quantity").val());
        if (quatity < 0) {
            $("#errorlst").text("Pack quantity must be more than zero.").show();
            $("#Quantity").addClass("input-validation-error");
            return;
        }
        if (!confirm("Ready to print?"))
            return;
        $("#addNewForm").submit();

        var iCsNo1 = parseInt($('#CsNo1').val());
        var iCsNo2 = parseInt($('#CsNo2').val());
        var iPieces = parseInt($('#Pieces').val());

        if (iCsNo2 <= iCsNo1) {
            $('#CsNo1').val(iCsNo1 + iPieces);
        } else {
            $('#CsNo1').val(iCsNo2);
        }
        
    }

}

/* If Sts, Renesas or Chipac is selected, Sts must be filled in.*/
isStsPartNoSolid = function () {

    var externalLabelOption = $('#ExternalLabel').val();
    var internalLabelOption = $('#InternalLabel').val();
    var scsPartNo = $('#ScsPartNo').val();

    // External label is not valid.
    if (externalLabelOption == '6' || externalLabelOption == '7' || externalLabelOption == '8' || externalLabelOption == '9') {
        if (scsPartNo == null || scsPartNo.length < 1) {
            return false;
        }
    }

    // Internal label is not valid.
    if (internalLabelOption == '10' || internalLabelOption == '11' || internalLabelOption == '12' || internalLabelOption == '13') {
        if (scsPartNo == null || scsPartNo.length < 1) {
            return false;
        }
    }

    return true;
}

// Find label which will be printed by external.
getExternalPrintOption = function () {
    return parseInt($("#ExternalLabel").val());
}

// Find label which will be printed by internal.
getInternalPrintOption = function () {
    return parseInt($("#InternalLabel").val());
}

// Find label which will be printed by ATM.
getATMPrintOption = function () {
    return parseInt($("#CodeLabel").val());
}

