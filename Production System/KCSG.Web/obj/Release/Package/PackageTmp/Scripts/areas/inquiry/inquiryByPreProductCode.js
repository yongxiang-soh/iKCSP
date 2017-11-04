$(document).ready(function () {
    //$("#txtPreProductCode").attr("readonly", false);
});

$().ready(function () {
    
    //$("#txtPreProductCode").attr("readonly", false);
    $("#txtPreProductCode").focusout(function () {
        $("#txtPreProductCode").val($("#txtPreProductCode").val().trim());
    });
    $("#txtLotNo").focusout(function () {
        $("#txtLotNo").val($("#txtLotNo").val().trim());
    });
    $.validator.unobtrusive.parse('#searchForm');
});



function search() {    
    if ($("#searchForm").valid()) {
      var request = {
          materialCode: $("#txtPreProductCode").val(),
          lotNo: $("#txtLotNo").val()
      };      
      gridHelper.searchObject("Grid", request);
    }     
}

function ClearSearch() {
    $("#txtPreProductCode").val("");
    $("#txtLotNo").val("");
    $("#txtPreproductName").val("");
    $("#Total").val("");
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
            materialCode: $("#txtPreProductCode").val(),
            lotNo: $("#txtLotNo").val()
        },
        type: 'POST',
        success: function (data) {
            $('#Total').val(data.data);
        }
    });

}

