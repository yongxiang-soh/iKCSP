$(document).ready(function () {
    $.validator.unobtrusive.parse('#searchFrom');
    $("#btnDelete").prop("disabled", true); $("#btnUpdate").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#PreProductPlanGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });
    $("#YearMonth").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
});

function addNew() {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: {
            date: null,
            code: null
        },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F01_MaterialCode').focus();
           
        }
    });
}

function edit() {
    var date = gridHelper.displayDate($('#PreProductPlanGrid').find('input:checked').val());
    var data = gridHelper.getSelectedItem("PreProductPlanGrid");
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: {
            date: date,
            code: data.F94_PrepdtCode
            },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F01_MaterialDsp').focus();
            
        }
    });
}
function hideForm() {
    $("#formPlaceholder").html("");
    enableButton("PreProductPlanGrid");
}

function search() {
    if ($("#searchFrom").valid()) {
        var request = {
            YearMonth: $("#YearMonth").val()
        };
        if (checkSearch()) {
            if (confirm("Unsaved data will be lost. Are you sure to search?")) {
                hideForm();
                gridHelper.searchObject("PreProductPlanGrid", request);
            }
        } else {
            gridHelper.searchObject("PreProductPlanGrid", request);
        }
    }
    $('#btnDelete').prop("disabled", true);
    $('#btnUpdate').prop("disabled", true);
    $("#btnPrint").prop("disabled", false);
}

function deleteItem() {
    var date = gridHelper.displayDate($('#PreProductPlanGrid').find('input:checked').val());
    var code = gridHelper.getSelectedItem("PreProductPlanGrid").F94_PrepdtCode;
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: formUrl.urlDelete,
            type: 'POST',
            data: {
                date: date,
                code: code
            },
            success: function (data) {
                showMessage(data, "PreProductPlanGrid","");
                //gridHelper.searchObject("MaterialGrid");
                $('#btnDelete').prop("disabled", true);
                $('#btnUpdate').prop("disabled", true);
            }
        });
    };
}   
function SelectProductCode(el) {
    var productCode = $(el).val();
    $.ajax({
        url: formUrl.urlGetProductName,
        type: 'GET',
        data: { preProductCode: productCode },
        success: function (data) {
           $("#PreProductName").val(data.result.F03_PreProductName);
        }
    });
}

function onSuccess(data, status, xhr) {
    showMessage(data, "PreProductPlanGrid", "");
    $("#formPlaceholder").html("");
    enableButton("PreProductPlanGrid");
    $('#btnDelete').prop("disabled", true);
    $('#btnUpdate').prop("disabled", true);
}

Print = function() {
    // Product/material/pre-product code.
    var request = {
        YearMonth: $("#YearMonth").val()
    };
    if (confirm("Are you sure to print all the item(s)?")) {
        $.ajax({
            url: "/ProductionPlanning/PreProductPlan/Print",
            type: "post",
            data: request,
            success: function (response) {

                // Template is detected.
                var render = response.render;
                if (render != null) {

                    $("#PrintArea")
                        .html(render)
                        .show()
                        .print()
                        .empty()
                        .hide();
                }
            }
        });
    }
   
}
