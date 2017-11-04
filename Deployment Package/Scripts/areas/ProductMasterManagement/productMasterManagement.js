$(document).ready(function() {
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
    $('#btnAdd').prop("disabled", true);

    //$('#Grid').on('change', 'input[type="radio"]', function() {
    //    // If checkbox is not checked
    //    var counterChecked = 0;
    //    this.checked ? counterChecked++ : counterChecked--;
    //    counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    //    counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
    //    counterChecked > 0 ? $('#btnAdd').prop("disabled", true) : $('#btnAdd').prop("disabled", false);

    //    $("#ProductName").attr('readonly', true);
    //    $("#NewProductName").prop('disabled', true);
    //});
   
    $("#dvTempGrid").find(".jsgrid-grid-header").scroll(function () {
        var leftpos = $(this).scrollLeft();
        $(".jsgrid-grid-body").animate({ scrollLeft: leftpos }, 0);
        return;
    });
  
});


function OnloadSuccess() {
      $(".jsgrid-grid-body").scroll(function () {
        var leftpos = $(this).scrollLeft();
        $("#dvTempGrid").find(".jsgrid-grid-header").animate({ scrollLeft: leftpos }, 0);
        return;
      });
      $("body").on("click", "#Grid tr ", function () {
          ResetData();
          Assign();
      });
}

function ChangeProductName() {
    var newProductName = $("#NewProductName option:selected").text();
    $("#ProductName").val("");
    $("#ProductName").val(newProductName);
    $("#USLMean").val("0.00");
    $("#UCLMean").val("0.00");
    $("#LSLMean").val("0.00");
    $("#LCLMean").val("0.00");
    $("#USLRange").val("0.00");
    $("#LSLRange").val("0.00");
    $("#NoOFLot").val("0");
    $("#btnAdd").prop("disabled", false);
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);

}

function Go() {

    if ($("#Location").valid()) {

        ResetData();
        var location = $('#Location option:selected').text();
        var request = {
            location: location
        }
        gridHelper.searchObject("Grid", request);
        $("#btnAdd").prop("disabled", false);
      
    }
}

function Assign() {
    var item = gridHelper.getSelectedItem("Grid");
    if (item != null) {
        var code = $('#Grid').find('input:checked').val();
        $("#F85_Code").val(code);
        $("#ProductName").val(item.ProductName);
        $("#USLMean").val(gridHelper.formatNumber(item.F85_M_Usl));
        $("#UCLMean").val(gridHelper.formatNumber(item.F85_M_Ucl));
        $("#LSLMean").val(gridHelper.formatNumber(item.F85_M_Lsl));
        $("#LCLMean").val(gridHelper.formatNumber(item.F85_M_Lcl));

        $("#USLRange").val(gridHelper.formatNumber(item.F85_R_Usl));
        $("#LSLRange").val(gridHelper.formatNumber(item.F85_R_Lsl));

        $("#NoOFLot").val(item.F85_No_Lot);
        $("#NewProductName").prop('disabled', true);

        $("#btnAdd").prop("disabled", true);
        $("#btnUpdate").prop("disabled", false);
        $("#btnDelete").prop("disabled", false);
        //$("#NewProductName").val(code);
    }

}

function Delete() {
    ClearErrorMessage();
    var code = $('#Grid').find('input:checked').val();
    if (!confirm("Ready to delete?"))
        return;
    $.ajax({
        url: "/EnvironmentManagement/ProductMasterManagement/Delete",
        type: "post",
        data: { code: code },
        success: function(data) {
            if (data.Success) {
                showMessage(data);
                setTimeout(function() {
                    ResetData();
                    Go();
                }, 3000);
                   $("#btnUpdate").prop("disabled", true);
                    $("#btnDelete").prop("disabled", true);
            }
        }
    });
}

function Add() {
    $("#isCreate").val("True");
    if ($("#productMasterManagementForm").valid()) {
        if (confirm("Ready to add?")) {
            $("#productMasterManagementForm").submit();

        }
    }
}

function Update() {
    $("#isCreate").val("False");
    if ($("#productMasterManagementForm").valid()) {
        if (confirm("Ready to update?")) {
            $("#productMasterManagementForm").submit();
        }
    }
}


function ResetData() {
    $("#F85_Code").val("");
    $("#ProductName").val("");
    $("#USLMean").val("");
    $("#UCLMean").val("");
    $("#LSLMean").val("");
    $("#LCLMean").val("");
    $("#USLRange").val("");
    $("#LSLRange").val("");
    $("#NoOFLot").val("");
    $("#ProductName").removeClass("input-validation-error");
    $("#ProductName-error").remove();
    $("#NewProductName-error").remove();

    //$("#btnAdd").prop("disabled", false);
    //$("#btnUpdate").prop("disabled", true);
    //$("#btnDelete").prop("disabled", true);

    //$("#ProductName").attr('readonly', false);
    $("#NewProductName").prop('disabled', false);
}

function onSuccess(data) {
    var isCreate = $("#isCreate").val();
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
    if (isCreate == 'True') {
        showMessage(data);
        if (data.Success) {
            ResetData();
            $("#NewProductName option:selected").remove();
        }
       
        

    } else {
        if (data.Success) {
            showMessage(data);
            setTimeout(function() {
                ResetData();
                Go();
            }, 3000);
        }
    }
}

function DisableAllButton() {
    $("#btnAdd").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
}

function ClearErrorMessage() {
    $("input[type='text']").removeClass('input-validation-error');
    $("#USLMean-error").text("").hide();
    $("#UCLMean-error").text("").hide();
    $("#LSLMean-error").text("").hide();
    $("#LCLMean-error").text("").hide();
    $("#USLRange-error").text("").hide();
    $("#LSLRange-error").text("").hide();
    $("#NoOFLot-error").text("").hide();
    $("#ProductName-error").text("").hide();
    $("#NewProductName-error").text("").hide();
}
