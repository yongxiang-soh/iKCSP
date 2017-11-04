$(document).ready(function () {
    $("#btnDeleteMaterial").prop("disabled", true);
    $("#btnUpdateMaterial").prop("disabled", true);

    $('#PreproductMaterialGrid').on('change', 'input[type="radio"]', function () {
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        if (counterChecked > 0) {
            $('#btnDeleteMaterial').prop("disabled", false);
            $('#btnUpdateMaterial').prop("disabled", false);
        } else {
            $('#btnDeleteMaterial').prop("disabled", true);
            $('#btnUpdateMaterial').prop("disabled", true);
        }
    });
 });

function GetMaterialName() {
    if ($("#formAddPdtMkp #IsCreate").val() == "True") {
        $.ajax({
            url: formUrlPrePdtMkp.urlGetMaterialName,
            type: 'POST',
            data: {
                materialCode: $("#F02_MaterialCode").val()
            },
            success: function (response) {
                if (response !== "") {
                    $("#F01_MaterialName").val($.trim(response.MaterialName));
                    $("#F01_LiquidClass").val($.trim(response.MaterialLiquid));
            }
        }
    });
    }
}

function save() {
    if ($("#formAddPdtMkp").valid()) {
        $.ajax({
            url: formUrlPrePdtMat.urlEdit,
            type: 'POST',
            data: $("#formAddPdtMkp").serialize(),
            success: function(response) {
                hideFormPrePdtMkp();
                var request = {
                    preProductId: $("#F03_PreProductCode").val()
                };
                $("#btnUpdateMaterial").prop('disabled', true);
                $("#btnDeleteMaterial").prop('disabled', true);
                showMessage(response, "PreproductMaterialGrid", request);
                $("#materialListError").hide();
            }
        });
    }
}


function onPrePdtMkpSuccess(data, status, xhr) {
    hideFormPrePdtMkp();
}

function hideFormPrePdtMkp() {
    $("#editMaterialArea").html("");
    enableButton("PreproductMaterialGrid");
    $("#btnSavePreProduct").prop("disabled", false);
    $("#btnCancelPreProduct").prop("disabled", false);
}


