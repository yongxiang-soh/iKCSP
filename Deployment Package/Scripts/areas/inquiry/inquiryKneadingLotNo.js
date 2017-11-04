$(document).ready(function () {
    if ($("#txtPreProductCode").val() != "") {
        $("#txtPreProductCode").prop("disabled", true);
    }
    if ($("#txtPreProLotNo").val() != "") {
        $("#txtPreProLotNo").prop("disabled", true);
    }
    $("#txtPreProductCode").focusout(function () {
        $("#txtPreProductCode").val($("#txtPreProductCode").val().trim());
    });
});

$().ready(function () {
    $.validator.unobtrusive.parse('#searchForm');
});



function search() {    
    if ($("#searchForm").valid()) {
        var request = {
            preCode:$("#txtPreProductCode").val(),
            preLotNo: $("#txtPreProLotNo").val(),
      };      
      gridHelper.searchObject("Grid", request);
      $("#btnStart").prop("disabled", true);
    }     
}

function ClearSearch() {
    $("#txtPreProductCode").val("");
    $("#txtPreProductName").val("");
    $("#txtPreProLotNo").val("");
    $("#txtPreProductCode").prop("disabled", false);
    $("#txtPreProLotNo").prop("disabled", false);
    //reloadKneadingCommandGridSelected();
    //gridHelper.searchObject("Grid", null);
    var items = gridHelper.getListingData("Grid");

    items.splice(0);
    $("#Grid").jsGrid("option", "data", items);
    $("#Grid").find('.jsgrid-pager-container').hide();

}

startKneadingCommand = function () {

    //System shows the confirmation message MSG 8. If user selects “No”, cancel Start action; reload the current page

    // Find kneading items which are currently selected.
    var selectedKneadingCommand = gridHelper.getSelectedItem("Grid");
    window.location.href = 'InquiryByLotNo?preLotNo=' + selectedKneadingCommand.F42_PrePdtLotNo + '&preCode=' + $('#txtPreProductCode').val() + '&preName=' + $('#txtPreproductName').val()



}


clickKneadingTable = function () {
    // Get the current selected item in the grid.
    var selectedKneadingItem = gridHelper.getSelectedItem("Grid");
    if (selectedKneadingItem == null)
        return;

    $("#Grid").find("input[type='radio']").prop("checked", false);
    $(".jsgrid-selected").find('input[type="radio"]').prop("checked", true);
    $("#btnStart").prop("disabled", false);

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


function granTotalLoaded() {
    $.ajax({
        url: formUrl.urlGrandTotal,
        cache: false,
        sync: false,
        data: {
            commandCode: $("#txtKneadingNo").val()
        },
        type: 'POST',
        success: function (data) {
            $('#Total').val(data.data);
        }
    });

}

