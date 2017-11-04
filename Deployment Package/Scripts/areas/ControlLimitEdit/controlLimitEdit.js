
$(document).ready(function() {
    $("#btnUpdate").prop("disabled", true);
    $('#Grid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });
});
$("body").on("click", "#Grid tr ", function () {
    LoadData();
});


//Show data in grid when selected value in dropdownlist
function LoadDataToGrid() {
    if ($("#Location").valid() & $("#From").valid() & $("#To").valid()) {
        Search();
    }
}
function Search() {
    ResetData();
    var location = $("#Location option:selected").text();
    var request = {
        location: location
    }
    gridHelper.searchObject("Grid", request);
}


//Load data for grid to all field  in Temperature and Humidity
function LoadData() {
    var itemSelected = gridHelper.getSelectedItem("Grid");
    //load data in Temperature
    $("#TempUCL").val(gridHelper.formatNumber(itemSelected.F80_T_Ucl));
    $("#TempLCL").val(gridHelper.formatNumber(itemSelected.F80_T_Lcl));
    $("#TempMean").val(gridHelper.formatNumber(itemSelected.F80_T_Mean));
    $("#TempSigma").val(gridHelper.formatNumber(itemSelected.F80_T_Sigma));
    $("#TempCp").val(gridHelper.formatNumber(itemSelected.F80_T_Cp));
    $("#TempCpk").val(gridHelper.formatNumber(itemSelected.F80_T_Cpk));
    $("#TempRange").val(gridHelper.formatNumber(itemSelected.F80_T_Range));

    //load data in Humidity
    $("#HumUCL").val(gridHelper.formatNumber(itemSelected.F80_H_Ucl));
    $("#HumLCL").val(gridHelper.formatNumber(itemSelected.F80_H_Lcl));
    $("#HumMean").val(gridHelper.formatNumber(itemSelected.F80_H_Mean));
    $("#HumSigma").val(gridHelper.formatNumber(itemSelected.F80_H_Sigma));
    $("#HumCp").val(gridHelper.formatNumber(itemSelected.F80_H_Cp));
    $("#HumCpk").val(gridHelper.formatNumber(itemSelected.F80_H_Cpk));
    $("#HumRange").val(gridHelper.formatNumber(itemSelected.F80_H_Range));
}

function ResetData() {
    $("#TempUCL").val("");
    $("#TempLCL").val("");
    $("#TempMean").val("");
    $("#TempSigma").val("");
    $("#TempCp").val("");
    $("#TempCpk").val("");
    $("#TempRange").val("");

    $("#HumUCL").val("");
    $("#HumLCL").val("");
    $("#HumMean").val("");
    $("#HumSigma").val("");
    $("#HumCp").val("");
    $("#HumCpk").val("");
    $("#HumRange").val("");
}

function onSuccess(data) {
    if (data.Success) {
        showMessage(data);
        setTimeout(function() {
            Search();
        }, 3000);

    }
}
