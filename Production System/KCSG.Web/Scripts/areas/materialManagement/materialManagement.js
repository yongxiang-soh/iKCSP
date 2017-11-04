$(document).ready(function () {
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    $('#txtPrcOrdNoCodeSearch').focus();
    // Handle click on checkbox to set state of "Select all" control
    $('#MaterialReceptionGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });
    $("#txtMaterialReceptionCodeSearch").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $('#txtPrcOrdNoCodeSearch').keyup(function () {
        var hasValue = this.value.length > 0;
        $("#errorPONo").text("").hide();
        $("#txtPrcOrdNoCodeSearch").removeClass("input-validation-error");
    });
    $('#txtParDeliveryCodeSearch').keyup(function () {
        var hasValue = this.value.length > 0;
        $("#errorPD").text("").hide();
        $("#txtParDeliveryCodeSearch").removeClass("input-validation-error");
    });
   
});

function addNew() {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: null },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F30_PrcOrdNo').focus();
            disableButton();
        }
    });
}




function edit(prcOrdNo, prtDvrNo) {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: {
            prcOrdNo: prcOrdNo,
            prtDvrNo: prtDvrNo,
        },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F30_MaterialCode').focus();
        }
    });
}

function hideForm() {
    $("#formPlaceholder").html("");
    enableButton("MaterialReceptionGrid");
}

function checkvalidation() {
    var checked = true;
    if ($('#txtPrcOrdNoCodeSearch').val() === "") {
        $("#errorPONo").text(message.MSG01).show();
        $("#txtPrcOrdNoCodeSearch").addClass("input-validation-error");
        checked = false;
    }
    if ($('#txtParDeliveryCodeSearch').val() === "") {
        $("#errorPD").text(message.MSG01).show();
        $("#txtParDeliveryCodeSearch").addClass("input-validation-error");
        checked = false;
    }
    if (checked === false)
        return false;
    return true;
}

function search() {
    if (!checkvalidation())
        return;
    var request = {
        PrcOrdNo: $("#txtPrcOrdNoCodeSearch").val().trim(),
        PartialDelivery: $("#txtParDeliveryCodeSearch").val().trim()
    };
    gridHelper.searchObject("MaterialReceptionGrid", request);
}

function UpdateItem() {
    var prcOrdNo = gridHelper.getSelectedItem("MaterialReceptionGrid").F30_PrcOrdNo;
    var prtDvrNo = gridHelper.getSelectedItem("MaterialReceptionGrid").F30_PrtDvrNo;
    edit(prcOrdNo, prtDvrNo);
}

function deleteItem() {
    var prcOrdNo = gridHelper.getSelectedItem("MaterialReceptionGrid").F30_PrcOrdNo;
    var prtDvrNo = gridHelper.getSelectedItem("MaterialReceptionGrid").F30_PrtDvrNo;
    var materialCode = gridHelper.getSelectedItem("MaterialReceptionGrid").F30_MaterialCode;
    if (confirm("Are you sure you want to delete?")) {
        $.ajax({
            url: formUrl.urlDelete,
            type: 'POST',
            data: {
                prcOrdNo: prcOrdNo,
                prtDvrNo: prtDvrNo,
                materialCode: materialCode
            },
            success: function (data) {
                var request = {
                    PrcOrdNo: $("#txtPrcOrdNoCodeSearch").val().trim(),
                    PartialDelivery: $("#txtParDeliveryCodeSearch").val().trim()
                };
                showMessage(data, "MaterialReceptionGrid",request);
                $("#btnDelete").prop("disabled", true);
                $("#btnUpdate").prop("disabled", true);
            }
        });
    };
}

function onSuccess(data, status, xhr) {
    var request = {
        PrcOrdNo: $("#txtPrcOrdNoCodeSearch").val().trim(),
        PartialDelivery: $("#txtParDeliveryCodeSearch").val().trim()
    };
    showMessage(data, "MaterialReceptionGrid", request);
    enableButton("MaterialReceptionGrid");
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    if(data.Success){
        $("#formPlaceholder").html("");
    }
}


function Clear() {
    $("#btnClear").parent().parent().find('input[type=text]').each(function () {
        $(this).val("");
    });
    gridHelper.searchObject("MaterialReceptionGrid", null);
}

