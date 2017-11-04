$(document).ready(function () {
    $("#btnDeleteProduct").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#ProductGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDeleteProduct').prop("disabled", false) : $('#btnDeleteProduct').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });

  
    $("#txtProductCodeSearch").focus();
    $("#txtProductCodeSearch").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $("#txtProductCodeSearch").keyup(function (e) {
        if (e.keyCode === 27 && $("#txtProductCodeSearch").val() != "") {
            $("#txtProductCodeSearch").val("");
            search();
            return false;
        }
    });
});

function addNew() {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: null },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#txtProductcode').focus();
        }
    });
}

function edit() {
    
    var id = $('#ProductGrid').find('input:checked').val();
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: id },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F09_ProductDesp').focus();
        }
    });
}

function hideForm() {
    
    
    if ($("#IsCreate").val()==="True") {
        var subMaterialCode = "";
        var productCode = $("#txtProductcode").val();
        $.ajax({
            url: formUrlSub.urlDelete,
            type: 'POST',
            data: { productCode: productCode, subMaterialCode: subMaterialCode },
            success: function(data) {
                $.unblockUI();
            }
        });
    }
    $("#formPlaceholder").html("");
   enableButton("ProductGrid");
}


function SaveProduct() {
    if ($("#SupMaterialGrid .jsgrid-nodata-row").length) {
        $("#addNewForm").valid();
        $("#materialListError").text("Please add at least one record.").show();

    } else {
        $("#addNewForm").submit();
    }
}
function search() {
    var request = {
        ProductCode: $("#txtProductCodeSearch").val().trim()
    };
    if (checkSearch()) {
        if (confirm("Unsaved data will be lost. Are you sure to search?")) {
            hideForm();
            gridHelper.searchObject("ProductGrid", request);
        }
    } else {
        gridHelper.searchObject("ProductGrid", request);
    }
}

function deleteItem() {
    var request = {
        ProductCode: $("#txtProductCodeSearch").val().trim()
    };
 var code = $('#ProductGrid').find('input:checked').val();
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: formUrl.urlDelete,
            type: 'POST',
            data: { lstCode: code },
            success: function (data) {
                showMessage(data, "ProductGrid", request);
                $("#btnUpdate").prop("disabled", true);
                $("#btnDeleteProduct").prop("disabled", true);
            }
        });
    };
}
function addNewSub() {
    if ($("#IsCreate").val() === "True" && $("#txtProductcode").val() === "") {
        alert("You must input the Product code before adding Sup. Material!");
        $("#materialListError").hide();
    } else {

        var productCode = $("#txtProductcode").val();
        var productName = $("#F09_ProductDesp").val();
        $.ajax({
            url: formUrlSub.urlEdit,
            type: 'GET',
            data: { productCode: productCode, productName: productName },
            success: function (data) {
                disableButton();
                $("#formAddSup").html(data);
                $('#subMaterialCode').focus();
            }
        });
    }
}

function ChangeSubCode(element) {
    $.ajax({
        url: formUrlSub.urlGetSubMaterialName,
        type: 'GET',
        data: { subMaterialCode: $(element).val() },
        success: function (data) {
            $("#SubMaterialName").val(data.F15_MaterialDsp);
            $("#unit").val(data.F15_Unit);

        }
    });
}
function UpdateSubMaterial() {

    var subMaterialCode = $('#SupMaterialGrid').find('input:checked').val();
    var productCode = $("#txtProductcode").val();
    var productName = $("#F09_ProductDesp").val();
    $.ajax({
        url: formUrlSub.urlEdit,
        type: 'GET',
        data: { productCode: productCode, productName: productName, subMaterialCode: subMaterialCode },
        success: function (data) {
            disableButton();
            $("#formAddSup").html(data);
            $('#subMaterialCode').focus();
        }
    });
}

function saveSub() {
    var request = { productCode: $("#txtProductcode").val() }
    if ($("#fAddSub").valid()) {
        $.ajax({
            url: formUrlSub.urlEdit,
            type: 'POST',
            data: $("#fAddSub").serialize(),
            success: function (data) {
                
                showMessage(data, "SupMaterialGrid", request);
                hideFormSub();
            }
        });
    }
}
function DeleteSub() {

    var subMaterialCode = $('#SupMaterialGrid').find('input:checked').val();
    var productCode = $("#txtProductcode").val();
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: formUrlSub.urlDelete,
            type: 'POST',
            data: { productCode: productCode,subMaterialCode:subMaterialCode },
            success: function (data) {
                var reuest = { productCode: productCode }
                showMessage(data, "SupMaterialGrid", reuest);
               
                hideFormSub();

            }
        });
    };
}

function hideFormSub() {
    $("#formAddSup").html("");
    enableButton("SupMaterialGrid");
    $('#btnUpdateSupMaterial').prop("disabled", "disabled");
    $('#btnDeleteSupMaterial').prop("disabled", "disabled");
}
function onSuccess(data, status, xhr) {
    $("#formPlaceholder").html("");
    enableButton("ProductGrid");
    var request = {
        ProductCode: $("#txtProductCodeSearch").val().trim()
    };
    $('#btnUpdate').prop("disabled", "disabled");
    $('#btnDeleteProduct').prop("disabled", true);
    showMessage(data, "ProductGrid", request);
}

function cancelSub() {
    $("#formAddSup").html("");
    enableButton("SupMaterialGrid");    
}

function ChangeProductCode() {
    $("#productCode").val($("#txtProductcode").val());
}

function ChangeProductName() {
    $("#productName").val($("#F09_ProductDesp").val());
}