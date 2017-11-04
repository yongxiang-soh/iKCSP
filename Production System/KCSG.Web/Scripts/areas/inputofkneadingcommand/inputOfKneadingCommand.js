$(document).ready(function () {
    $("#btnSelected").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
    $("#btnOK").prop("disabled", true);
    $('#btnPrint').prop("disabled", true)
    $('#KneadingCommandGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnSelected').prop("disabled", false) : $('#btnSelected').prop("disabled", true);
    });
    $('#KneadingCommandGridSelected').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $('#btnPrint').prop("disabled", false) : $('#btnPrint').prop("disabled", true);
    });
});

$().ready(function () {
    $.validator.unobtrusive.parse('#searchForm');
});



function search() {
    if ($("#searchForm").valid()) {
      var request = {
            Within: $("#Within").val(),
            KndLine: $("#KndLine").find('input:checked').val()
        };
      gridHelper.searchObject("KneadingCommandGrid", request);
      reloadKneadingCommandGridSelected();

    }
   
  
}

function ClearSearch() {
    $("#Within").val("");
    reloadKneadingCommandGridSelected();
    gridHelper.searchObject("KneadingCommandGrid", null);

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
    //$('#btnSelected').prop('disabled', true);
    //$("#KneadingCommandGrid").find('input').prop('disabled', true);
}

function DeleteItem() {
    //var date = gridHelper.displayDate($('#KneadingCommandGridSelected').find('input:checked').val());
    //var deleteCode = gridHelper.getSelectedItem("KneadingCommandGridSelected").F39_PreProductCode;

    //var resuest = {
    //    selectedValue: $('#selectedValue').val()
    //}
    if (!confirm("Ready to delete?"))
        return;

    //$('#KneadingCommandGridSelected').jsGrid("option", "data", []);
    gridHelper.searchObject("KneadingCommandGridSelected", null);
    $('#btnSelected').prop('disabled', false);
    gridHelper.ReloadGrid('KneadingCommandGrid');
    $('#btnPrint, #btnDelete, #btnOK').prop('disabled', true);
    $('#selectedValue').val(null);
    $("#KneadingCommandGrid").find('input').prop('disabled', false);

    //$.ajax({
    //    url: "/KneadingCommand/InputOfKneadingCommand/SelectedKindingCommand",
    //    type: 'POST',
    //    data: { date: date, deleteCode: deleteCode },
    //    success: function (data) {
    //        showMessage(data, "KneadingCommandGridSelected", resuest);
    //        $("#btnDelete").prop("disabled", true);
    //        $('#btnPrint').prop("disabled", true);
    //    }
    //});
}

function ConfirmKneading() {
    var selectedValue = $('#selectedValue').val();
    var within = $('#Within').val();
    var item = gridHelper.getSelectedItem("KneadingCommandGridSelected");

    if (item == null)
        return;

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
        data: { selectedValue: selectedValue, within: within, lotQuantity: item.F39_PrePdtLotAmt },
        success: function (data) {
            gridHelper.searchObject("KneadingCommandGrid", requestForKneadingCommandGrid);

            reloadKneadingCommandGridSelected();
        }
    });

}
function reloadKneadingCommandGridSelected() {
    $('#selectedValue').val(null);
    var request = {
        selectedValue: null
    }

    gridHelper.searchObject("KneadingCommandGridSelected", request);
    $("#btnSelected").prop("disabled", true);
    $("#btnOK").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
    $('#btnPrint').prop("disabled", true);
}

//function onSuccess(data, status, xhr) {
//    showMessage(data, "MaterialGrid", "");
//    $("#formPlaceholder").html("");
//    $("#btnDelete").prop("disabled", true);
//    $("#btnUpdate").prop("disabled", true);
//}

/**
 * This function is for exporting the input kneading command list.
 * @returns {} 
 */
exportInputKneadingCommand = function (event) {

    // Prevent default behaviour of button.
    event.preventDefault();

    // Display confirmation dialog.
    if (!confirm("Ready to print?")) {
        return;
    };

    // Find the selected item on grid.
    var kneadingCommand = gridHelper.findSelectedRadioItem("KneadingCommandGridSelected");

    // Invalid selected item.
    if (kneadingCommand == null)
        return;

    $.ajax({
        url: formUrl.urlPrintKneadingCommands,
        data: {
            preProductCode: kneadingCommand.F39_PreProductCode,
            commandNo: kneadingCommand.CmdNo
        },
        type: "post",
        success: function (response) {

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
