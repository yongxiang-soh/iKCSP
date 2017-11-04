$(document).ready(function () {

    //$("#btnExit").prop("disabled", true);
    //$('#btnPrint').prop("disabled", true);
    //$('#Grid').on('change', 'input[type="radio"]', function () {
    //    // If checkbox is not checked
    //    var counterChecked = 0;
    //    this.checked ? counterChecked++ : counterChecked--;
    //    counterChecked > 0 ? $('#btnSelected').prop("disabled", false) : $('#btnSelected').prop("disabled", true);
    //});   
});

$().ready(function () {
    //$("#txtSupplierCode").attr("readonly", false);
    $("#txtSupplierCode").focusout(function () {
        $("#txtSupplierCode").val($("#txtSupplierCode").val().trim());
    });

    $.validator.unobtrusive.parse('#searchForm');
});



function search() {
    //
    if ($("#searchForm").valid()) {
        var request = {
            supplierCode: $("#txtSupplierCode").val()
        };
        gridHelper.searchObject("Grid", request);
        //reloadGrid();
    }
}

function ClearSearch() {
    $("#txtSupplierCode").val("");
    $("#txtSupplierName").val("");    
    $("#GrandTotal").val("");    
    //reloadKneadingCommandGridSelected();
    //gridHelper.searchObject("Grid", null);
    var items = gridHelper.getListingData("Grid");

    items.splice(0);
    $("#Grid").jsGrid("option", "data", items);
    $("#Grid").find('.jsgrid-pager-container').hide();

}


function reloadGrid() {
    $('#selectedValue').val(null);
    var request = {
        selectedValue: null
    }

    gridHelper.searchObject("Grid", request);
    //$("#btnSelected").prop("disabled", true);
    //$("#btnOK").prop("disabled", true);
    //$("#btnDelete").prop("disabled", true);
    //$('#btnPrint').prop("disabled", true);
}

/**
 * This function is for exporting the input kneading command list.
 * @returns {} 
 */
exportInquiry = function (event) {
    var status = $('input[name=PrintOptions]:checked').val();

    // Prevent default behaviour of button.
    event.preventDefault();

    // Display confirmation dialog.
    if (!confirm("Ready to print?")) {
        return;
    };    
    $.ajax({
        url: formUrl.urlPrint,
        data: {
            status: status
        },
        type: "post",
        success: function (response) {

            var render = response.render;

            if (render != null) {

                $("#printArea")
                    .html(render)
                    .show()
                    .print()
                    .empty()
                    .hide();
            }
        }
    });
}

function granTotalLoaded() {
    $.ajax({
        url: formUrl.urlGrandTotal,
        cache: false,
        sync: false,
        data: {
            supplierCode: $('#txtSupplierCode').val()
        },
        type: 'POST',
        success: function (data) {
            $('#GrandTotal').val(data.data);
            $('#GrandTotal').autoNumeric('set', $('#GrandTotal').val());
        }
    });
}

function OnChangeTextBox() {
    $.ajax({
        url: formUrl.urlCheckPreProduct,
        //url: "/ProductCertification/Edit",
        type: 'GET',
        data: {
            supplierCode: $('#txtSupplierCode').val()
           
        },
        success: function (data) {
            $("#txtSupplierName").val(data);
        }
    });
}
