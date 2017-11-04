$(document).ready(function() {
    $("#btnPostReception").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#MaterialPostReception').on('change', 'input[type="radio"]', function() {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnPostReception').prop("disabled", false) : $('#btnPostReception').prop("disabled", true);
    });
});

function postReception() {
    var data = gridHelper.getSelectedItem("MaterialPostReception");
    //if (!confirm("Are you sure you want to post-receive?"))
    //    return;

    // Backup data.
    var materialShelfStock = data;

    $.ajax({
        url: formUrl.urlPostReception,
        type: 'POST',
        data: {
            materialCode: data.F33_MaterialCode,
            materialName: data.F01_MaterialDsp,
            shelfNo: data.ShelfNo,
            palletNo: data.F33_PalletNo
        },
        success: function (data) {

            $.ajax({
                url: formUrl.urlFindMaterialReceptionInput,
                data: {
                    materialCode: materialShelfStock.F33_MaterialCode,
                    palletNo: materialShelfStock.F33_PalletNo
                },
                method: 'POST',
                success: function (x) {

                    if (x == null || !(x instanceof Array)) {
                        return;
                    }

                    for (var index = 1; index <= x.length; index++) {
                        var item = x[index - 1];
                        $('#LotNo' + index).val(item.F33_MaterialLotNo);
                        $('#Quantity' + index).val(item.F33_Amount);
                    }
                }
            });

            $("#formPlaceholder").html(data);
            //$('#F09_ProductCode').focus();
            $('#btnPostReception').prop("disabled", true);
        }
    });
}

function hideForm() {
    $("#formPlaceholder").html("");
    $('#btnPostReception').prop("disabled", false);
}

function onSuccess(data) {
    
    showMessage(data, "MaterialPostReception");
    $("#formPlaceholder").html("");
}