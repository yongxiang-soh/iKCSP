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
    //$("#txtMaterialCode").attr("readonly", false);
    $("#txtMaterialCode").focusout(function () {
        $("#txtMaterialCode").val($("#txtMaterialCode").val().trim());
    });

    $.validator.unobtrusive.parse('#searchForm');
});



function search() {
    
    if ($("#searchForm").valid()) {
      var request = {
          materialCode: $("#txtMaterialCode").val()
      };      
      gridHelper.searchObject("Grid", request);
        //reloadGrid();      
      
    }     
}

function ClearSearch() {
    $("#txtMaterialCode").val("");
    $("#txtBailmentClass").val("");
    $("#txtMaterialName").val("");
    $("#GrandTotal").val("");
    //reloadKneadingCommandGridSelected();
    //gridHelper.searchObject("Grid", null);
    var items = gridHelper.getListingData("Grid");

    items.splice(0);
    $("#Grid").jsGrid("option", "data", items);
    $("#Grid").find('.jsgrid-pager-container').hide();

}

function Selected() {
   
    //$("#KneadingCommandGrid").find('input:checked').each(function () {
    //    var selectedDate = $(this).parent().next().html();
    //    var selectedId = $(this).parent().next().next().html();

    //    lstSect += selectedDate + "," + selectedId.trim() + "#";
    //});
    var selectId = gridHelper.getSelectedItem("KneadingCommandGrid").F39_PreProductCode.trim();
    var selectDate = $("#KneadingCommandGrid").find('input:checked').parent().next().html();
    var str = selectDate + "," + selectId;
    var lststart = $('#selectedValue').val().split("#");
    for (var i = 0; i < lststart.length; i++) {
        if (lststart[i] === str) {
            var errorMessage03 = { Success: false, Message: "This record has been selected" }
            showMessage(errorMessage03, "", "");
            return;
        }
    }
    if (lststart.length - 1 >= 99) {
        var errorMessage04 = { Success: false, Message: "Kneading order number must be less than 100!" }
        showMessage(errorMessage04, "", "");
        return;
    }

    $('#selectedValue').val($('#selectedValue').val() + str + "#");

    var resuest = {
        selectedValue: $('#selectedValue').val()
    }
    gridHelper.searchObject("KneadingCommandGridSelected", resuest);
    $("#btnOK").prop("disabled", false);
}

function DeleteItem() {
    var date = gridHelper.displayDate($('#KneadingCommandGridSelected').find('input:checked').val());
    var deleteCode = gridHelper.getSelectedItem("KneadingCommandGridSelected").F39_PreProductCode;

    var resuest = {
        selectedValue: $('#selectedValue').val()
    }
    if (!confirm("Ready to delete?"))
        return;
    $.ajax({
        url: "/KneadingCommand/InputOfKneadingCommand/SelectedKindingCommand",
        type: 'POST',
        data: { date: date, deleteCode: deleteCode },
        success: function (data) {
            showMessage(data, "KneadingCommandGridSelected", resuest);
            $("#btnDelete").prop("disabled", true);
            $('#btnPrint').prop("disabled", true);
        }
    });
}

function ConfirmKneading() {
    var selectedValue = $('#selectedValue').val();
    var within = $('#Within').val();
    var requestForKneadingCommandGrid = {
        Within: $("#Within").val(),
        KndLine: $("#KndLine").find('input:checked').val()
    };
    var requestForKneadingCommandGridSelect = {
        selectedValue: null
    }
    $.ajax({
        url: "/KneadingCommand/InputOfKneadingCommand/CreateOrUpdate",
        type: 'POST',
        data: { selectedValue: selectedValue, within: within },
        success: function (data) {
            gridHelper.searchObject("KneadingCommandGrid", requestForKneadingCommandGrid);

            reloadKneadingCommandGridSelected();
        }
    });

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
exportInquiryByMaterialName = function (event) {
    var status = $('input[name=PrintOptions]:checked').val();
    
    // Prevent default behaviour of button.
    event.preventDefault();

    // Display confirmation dialog.
    if (!confirm("Ready to print?")) {
        return;
    };
           
    $.ajax({
        url: formUrl.urlPrintMaterialName,
        data: {
            status: status
        },
        type: "post",
        success: function (response) {

            var render = response.render;

            if (render != null) {

                $("#printMaterialNameArea")
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
            materialCode: $("#txtMaterialCode").val()
        },
        type: 'POST',
        success: function (data) {
            $('#GrandTotal').val(data.data);
            $('#GrandTotal').autoNumeric('set', $('#GrandTotal').val());
        }
    });

}

function OnChangeTextBox() {    
    var model = {
        materialCode: $('#txtMaterialCode').val()
    }
    
    $.ajax({
        url: formUrl.urlCheckPreProduct,
        //url: "/ProductCertification/Edit",
        type: 'GET',
        data: {
            materialCode: model.materialCode
        },        
        success: function (data) {
            if (data.result != null) {
                $("#txtMaterialName").val(data.result.F01_MaterialDsp);
                if (data.result.F01_EntrustedClass === '0') {
                    $("#txtBailmentClass").val('Norm');
                } else {
                    $("#txtBailmentClass").val('Bail');
                }
                //$("#txtBailmentClass").val(data.result.F01_EntrustedClass);
            } else {
                $("#txtMaterialName").val("");
                $("#txtBailmentClass").val("");
            }
            
        }
    });
}

