
$(document)

    .ready(function () {
        $("#btnEmpty").prop("disabled", true);


        $("body").bind("change", "#txtPalletNo", function () {
            ReCaculator();
        });        
        $('#MaterialLotNo01')
            .keyup(function() {
                var hasValue = this.value.length > 0;
                $("#errorList").text("");
                $(".errorInput").removeClass("has-error");
            });

        $('body')
            .on("submit",
                "#addNewForm",
                function (event) {
                    
                    if ($("#MaterialLotNo01").val() === '' && $("#MaterialLotNo02").val() === ''
                        && $("#MaterialLotNo03").val() === '' && $("#MaterialLotNo04").val() === ''
                        && $("#MaterialLotNo05").val() === '') {
                        alert('Material lot-no cannot be empty!');
                        return false;
                    }
                    var no1 = $("#MaterialLotNo01").val();
                    var no2 = $("#MaterialLotNo02").val();
                    var no3 = $("#MaterialLotNo03").val();
                    var no4 = $("#MaterialLotNo04").val();
                    var no5 = $("#MaterialLotNo05").val();
                    var arrInput = [no1, no2, no3, no4, no5];
                    // var sorted_arr = arrInput.sort();

                    //for (var i = 0; i < arrInput.length; i++) {
                    //    $('#errorList0' + i).text("").hide;
                    //    if (arrInput[i] === '') {
                    //        $('#errorList0' + i).text('Please input data for this required field.').show();
                    //        return false;
                    //    }
                    //}

                    for (var i = 0; i < arrInput.length - 1; i++) {
                        if (arrInput[i] != '') {
                            for (var j = i + 1; j < arrInput.length; j++) {
                                $('#errorList0' + j).text("").hide;
                                if (arrInput[i] === arrInput[j]) {
                                    $('#errorList0' + j).text('This lot-no is been used!').show();
                                    return false;
                                }
                            }
                        }
                    }
                    
                    for (var i = 1; i <= arrInput.length; i++) {
                        if (arrInput[i - 1] !== '' && $('#PackQuantity0' + i).val() === '0') {
                            //if ($('#PackQuantity0' + i).val() === '0') {
                            $('#errorPackQuantity0' + i).text('At least one line must be valid, please input data!').show();
                                return false;
                            //}                            
                        }
                        if ((arrInput[i - 1] === '' && $('#PackQuantity0' + i).val() !== '0')) {
                            $('#errorList0' + i - 1).text('At least one line must be valid, please input data!').show();
                            return false;
                        }
                    }
                    
                    $("#errorList").text("").hide();
                    //if (!checkPackQtyValidity()) return false;
                    if (!checkFractionValidity()) return false;
                    if ($("#GrandTotal").val() === "0") {
                        //$("#GrandTotalError").text(message.MSG09).show();
                        $("#GrandTotalError").text('Pack quantity must be more than zero!').show();
                        return false;
                    }
                    if (!confirm("Are you sure you want to restore?")) return false;
          });

    });

/**
 * This function is for unassigning materials.
 * @returns {} 
 */
unassignMaterials = function () {
    
    if (!$("#addNewForm").valid())
        return;

    if ($('#txtPalletNo').val() == '') {
        $('#errorPalletNo').text('Please input data for this required field.').show();
        return;
    }
    // Display confirmation dialog.
    if (!confirm("Are your sure you want to de-assign?"))
        return;
    var palletNo = $("#txtPalletNo").val();
    var materialCode = $("#txtMaterialCode").val();

    // Send request to server to do material unassignment.
    $.ajax({
        url: "/MaterialManagement/RestorageOfMaterial/Unassign",
        data: { palletNo: palletNo, materialCode: materialCode },
        type: "post",
        success: function (data) {
            if (data.Success) {
                $("#btnRestorage").prop("disabled", true);
                $("#btnDeassign").prop("disabled", true);
                $("#btnEmpty").prop("disabled", false);
            }
            
            showNormanMessage(data);
        },
        error: function(data) {
            // Bad request status is responded.
            if (data.status === 400) {
                handleJQueryValidationMessages(data.responseJSON);
            }
        }
    });
}

/**
 * This function is for cleaning materials list
 * @returns {} 
 */
emptyMaterials = function() {
    if (!$("#addNewForm").valid())
        return;

    if ($('#txtPalletNo').val() == '') {
        $('#errorPalletNo').text('Please input data for this required field.').show();
        return;
    }

    // Display confirmation dialog.
    if (!confirm("Are you sure you want to store empty pallet?"))
        return;
    var palletNo = $("#txtPalletNo").val();
    var materialCode = $("#txtMaterialCode").val();
    // Send request to server to do material unassignment.
    $.ajax({
        url: "/MaterialManagement/RestorageOfMaterial/Empty",
        data:{palletNo:palletNo,materialCode:materialCode},
        type: "post",
        success: function(data) {
            //if (!data.Success) {
            showMessage(data, "", "");
            blockScreenAccess("Waiting for message from C1");
            //}

        },
       
    });
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
    if ($("#txtUnit").val().trim() !== "K") {
        for (var i = 1; i <= 5; i++) {
            if ($('#MaterialLotNo0' + i).val() != '') {
                if (parseInt($("#Fraction0" + i).val()) >= parseInt($("#PackUnit0" + i).val())) {
                    $('#errorFraction0' + i).text('Fraction cannot be more than the packing unit!').show();
            valid = false;
                }
            }   
        }   
    }
    return valid;
}

//Calculate Total
function calculateTotal(packUnit, packQuantity, fraction, total) {
    //if ($("#txtUnit").val().trim() !== "0") {
    //    total.val(parseFloat(packUnit) * parseFloat(packQuantity) + parseFloat(fraction));
    //} else {
    //    total.val(parseFloat(packQuantity));
    //}

    for (var index = 1; index < 6; index++) {
        var szPackUnit = $('#PackUnit0' + index).val();
        if (!szPackUnit)
            continue;

        var flPackQuantity = 0;
        var szPackQuantity = $('#PackQuantity0' + index).val();
        if (szPackQuantity)
            flPackQuantity = parseFloat(szPackQuantity);

        var szFraction = $('#Fraction0' + index).val();
        var flFraction = 0;
        if (szFraction)
            flFraction = parseFloat(szFraction);

        var flPackUnit = parseFloat(szPackUnit);
        $('#Total0' + index).val(flPackQuantity * flPackUnit + flFraction);
    }

    calculateGrandTotal();
}

//Calculate GrandTotal
function calculateGrandTotal() {
    $('#GrandTotal').val(parseFloat($('#Total01').val()) + parseFloat($('#Total02').val()) + parseFloat($('#Total03').val()) + parseFloat($('#Total04').val()) + parseFloat($('#Total05').val()));
}

function PackQuantity(element) {

    for (var i = 1; i <= 5; i++) {
        if ($('#PackQuantity0' + i).val() != '0') {
            $('#errorPackQuantity0' + i).hide();
        }
    }
    calculateTotal($('#PackUnit0' + element).val(), $('#PackQuantity0' + element).val(), $('#Fraction0' + element).val(), $('#Total0' + element));
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

function checkMaterialNotNo() {
    
    }

function onSuccess(data) {
    if (data.Success === true) {
        blockScreenAccess("Waiting messages from C1");
    } else {
        showMessage(data);
    }
}




/*Call when pallet no is changed.*/
changePalletNo =  function () {

    setTimeout(function () {
        $.ajax({
            url: "/RestorageOfMaterial/FindMaterialDetails",
            method: "POST",
            data: {
                palletNo: $('#txtPalletNo').val(),
                materialCode: $('#txtMaterialCode').val()
            },
            success: function (x) {

                if (!x || !(x instanceof Array))
                    return;

                for (var i = 0; i < x.length; i++) {
                    $('#MaterialLotNo0' + (i + 1)).val(x[i].F33_MaterialLotNo);
                    $('#PackQuantity0' + (i + 1)).val(x[i].F33_Amount / x[i].F01_PackingUnit);
                    $('#PackUnit0' + (i + 1)).val(x[i].F01_PackingUnit);
                }

                // Re-calculate fields.
                ReCaculator();
            }
        });

    }, 200);
    
    
}

function ReCaculator() {

    

    for (var i = 1; i <= 5; i++) {
        PackQuantity(i);
    }
    calculateGrandTotal();
}


$(function () {

    // Initiate communication to c1 hub.
    var signalrCommunication = initiateHubConnection(hubList.c1);

    // Message is sent from 
    signalrCommunication.client.receiveMessageFromC1 = function (message) {
        if (message.PictureNo != 'TCRM051F')
            return;

        // Unblock message.
        $.unblockUI();

        $.ajax({
            url: '/MaterialManagement/RestorageOfMaterial/PostStoreMaterial',
            type: 'post',
            success: function (items) {

                if (items == null || !(items instanceof Array))
                    return;

                for (var index = 0; index < items.length; index++) {
                    toastr["info"](initiateFirstCommunicationMessage(items[index]));
                }

                // Enable storage button.
                $("input[type='button'],button").prop("disabled", false);
                $("#btnEmpty").prop('disabled', true);
            }
        });

    }

    // Start connection to hub.
    signalrStartHubConnection();
}); 


