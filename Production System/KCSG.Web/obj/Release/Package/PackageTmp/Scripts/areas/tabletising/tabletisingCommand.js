var isChecked = false;

$(document).ready(function() {
    $("#btnSelect").prop("disabled", true);
    $("#btnContainer").prop("disabled", true);
    $("#btnStart").prop("disabled", true);
    $("#btnEnd").prop("disabled", true);
    $("#btnPrint").prop("disabled", true);
    $("#btnEquilibrium").prop("disabled", true);

    $('#TabletisingGrid').on('change', 'input[type="radio"]', function() {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnSelect').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        $("#btnEquilibrium").prop("disabled", false);
    });
    $('#TabletisingSelectGrid').on('change', 'input[type="radio"]', function() {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnContainer').prop("disabled", false) : $('#btnContainer').prop("disabled", true);
        counterChecked > 0 ? $('#btnStart').prop("disabled", false) : $('#btnStart').prop("disabled", true);
        //counterChecked > 0 ? $('#btnEnd').prop("disabled", false) : $('#btnEnd').prop("disabled", true);
        counterChecked > 0 ? $('#btnPrint').prop("disabled", false) : $('#btnPrint').prop("disabled", true);
        
        var status = gridHelper.getSelectedItem("TabletisingSelectGrid").Status;
        if (status === "Tableting" || status === "Change") {
            $('#btnEnd').prop("disabled", false);
        } else {
            $('#btnEnd').prop("disabled", true);
        }
    });
});

function search(cmdNo, lotno) {
    var resuest = {
        cmdNo: cmdNo,
        lotno: lotno
    }
    gridHelper.searchObject("TabletisingSelectGrid", resuest);
}

function Selected() {
    var cmdNo = gridHelper.getSelectedItem("TabletisingGrid").F41_KndCmdNo;
    var lotNo = gridHelper.getSelectedItem("TabletisingGrid").F41_PrePdtLotNo;
    $.ajax({
        url: "/TabletisingCommandSubSystem/TabletisingStartStop/CheckStatus",
        type: "POST",
        data: { cmdNo: cmdNo, lotNo: lotNo },
        success: function (data) {
            $.unblockUI();
            setTimeout(function() {
                if (data.Success) {
                    alert("This lot has not been completed.");
                }
                if (!confirm("Ready to continue?")) {
                    $.unblockUI();
                    return;
                }
                search(cmdNo, lotNo);
            },500);
        }

    });
}

function CheckNumberRecord() {
    var result = gridHelper.getListingData("TabletisingSelectGrid");
    if (result.length <= 0) {
        setTimeout(function() {
            var errorMessage01 = { Success: false, Message: "Cannot find corresponding records from database!" }
            showMessage(errorMessage01);
        });

    }
}

//function Check(cmdNo, lotNo) {
//    return $.ajax({
//        url: "/TabletisingCommandSubSystem/TabletisingStartStop/CheckStatus",
//        type: "POST",
//        data: { cmdNo: cmdNo, lotNo: lotNo },
//        success: function(data) {
//            if (data.Success) {
//                setTimeout(function() {
//                    alert("This lot has not been completed.");
//                }, 1000);
//            }
//        }

//    });
//}

function CheckValidationButtonEnd() {
    var cmdNo = gridHelper.getSelectedItem("TabletisingGrid").F41_KndCmdNo;
    var lotNo = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductLotNo;
    var preProductLotNo = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_PrePdtLotNo;
    var productCode = $('#TabletisingSelectGrid').find('input:checked').val();
    var productName = $('#TabletisingSelectGrid').find('input:checked').parent().next().next().html();
    //var productCode = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductCode;
    //var productName = gridHelper.getSelectedItem("TabletisingSelectGrid").ProductName;

    var packageValue = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_TbtCmdEndPackAmt;
    var fraction = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_TbtCmdEndFrtAmt;
    var packUnit = gridHelper.getSelectedItem("TabletisingSelectGrid").PackUnit;
    return $.ajax({
        url: "/TabletisingCommandSubSystem/TabletisingStartStop/ValidationForEndButton",
        type: "POST",
        data: { cmdNo: cmdNo },
        success: function(data) {
            if (data.Success) {
                
                if (confirm("There are left pre-product in warehouse yet. Ready to end the Tabletising Command?")) {
                    EndTabletising(cmdNo, lotNo, productCode, productName, packageValue, fraction, packUnit, preProductLotNo);
                } else {
                    $.unblockUI();
                }
            } else {
                if (confirm("Ready to end the Tabletising Command?")) {
                    EndTabletising(cmdNo, lotNo, productCode, productName, packageValue, fraction, packUnit, preProductLotNo);
                } else {
                    $.unblockUI();
                }
            }
        }

    });
}

function ContainerSet() {
    var cmdNo = gridHelper.getSelectedItem("TabletisingGrid").F41_KndCmdNo;
    var lotNo = gridHelper.getSelectedItem("TabletisingGrid").F41_PrePdtLotNo;
    var lowerLotNo = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductLotNo;
    var productCode = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductCode;
    if (confirm("Ready to set container?")) {
        $.ajax({
            url: "/TabletisingCommandSubSystem/TabletisingStartStop/ContainerSet",
            type: 'POST',
            Async:false,
            data: { cmdNo: cmdNo, lotNo: lotNo },
            success: function(data) {
                if (!data.Success) {
                    setTimeout(function() {
                        alert(data.Message);
                    }, 1000);

                }
                search(cmdNo, lotNo);
               // gridHelper.searchObject("TabletisingSelectGrid");
                $("#btnPrint").prop("disabled", true);
                $("#btnContainer").prop("disabled", true);
                $("#btnStart").prop("disabled", true);
                $("#btnEnd").prop("disabled", true);
                $.unblockUI();
                isChecked = false;
                //var interval = setInterval(function() {

                //    if (isChecked) {
                //        clearInterval(interval);
                //        //console.log(isChecked);
                //    } else {
                //        TimeJob(cmdNo, productCode, lotNo, lowerLotNo);
                //    }
                //}, 10000);

            }
        });
       
    }

}

function TimeJob(cmdNo, productCode, lotNo, lowerLotNo) {

    $.ajax({
        url: "/TabletisingCommandSubSystem/TabletisingStartStop/TimeJob",
        type: "POST",
        data: { cmdNo: cmdNo, productCode: productCode, lotNo: lotNo, lowerLotNo: lowerLotNo },
        success: function(data) {
            if (data.Success) {
                $.unblockUI();
                if (data.Message[0] === "editable") {
                    $("#btnContainer").prop("disabled", false);
                } else {
                    $("#btnContainer").prop("disabled", true);
                }
                isChecked = true;
            }
            //$("#btnContainer").prop("disabled", data.Success != null ? data.Success : false);
            if (!data.Success) {
                isChecked = false;
            }
        }
    });

}

function StartTabletising() {
    var commandNo = gridHelper.getSelectedItem("TabletisingGrid").F41_KndCmdNo;
    var lotNo = gridHelper.getSelectedItem("TabletisingGrid").F41_PrePdtLotNo;
    var preProductCode = gridHelper.getSelectedItem("TabletisingGrid").F41_PreproductCode;
    var productCode = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductCode;
    
    if (confirm("Ready to start?")) {
        $.ajax({
            url: "/TabletisingCommandSubSystem/TabletisingStartStop/Start",
            type: "POST",
            data: { commandNo: commandNo, lotNo: lotNo, preProductCode: preProductCode, productCode: productCode },
            success: function(data) {
                search(commandNo, lotNo);

                $("#btnContainer").prop("disabled", true);
                $("#btnStart").prop("disabled", true);
                $("#btnEnd").prop("disabled", false);
            }
        });
    }

}

function EndTabletising(cmdNo, lotNo, productCode, productName, packageValue, fraction, packUnit, preProductLotNo) {

    $.ajax({
        url: "/TabletisingCommandSubSystem/TabletisingStartStop/End",
        type: "GET",
        data: { cmdNo: cmdNo, lotNo: lotNo, productCode: productCode, productName: productName, packageValue: packageValue, fraction: fraction, packUnit: packUnit, preProductLotNo: preProductLotNo },
        success: function(data) {
            $('#endForm').modal('toggle');
            $('#endForm .modal-body').html(data);
        }
    });
}

function Equilibrium() {
    location.reload();
}


function Print() {
    var commandNo = gridHelper.getSelectedItem("TabletisingGrid").F41_KndCmdNo;
    var productCode = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductCode;
    var lotNo = gridHelper.getSelectedItem("TabletisingSelectGrid").F56_ProductLotNo;
    var tableLine = gridHelper.getSelectedItem("TabletisingGrid").F41_TabletLine;
    var url = "/TabletisingCommandSubSystem/ManagementOfProductLabel/Index?commandNo=" + commandNo + "&productCode=" + productCode + "&lotNo=" + lotNo + "&tableLine=" + tableLine;
    window.location.href = url;

}