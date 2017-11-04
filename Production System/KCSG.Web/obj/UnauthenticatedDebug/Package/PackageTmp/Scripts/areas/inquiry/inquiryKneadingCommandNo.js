$(document).ready(function () {
    if($("#txtKneadingNo").val()!= "") {
        $("#txtKneadingNo").prop("disabled", true);
    }
    $("#btnStart").prop("disabled", true);
    $('#txtKneadingNo').keyup(function () {
        getCodeNamebycmNo();
                $.unblockUI();
    });

    var isdisable = $('#txtKneadingNo').prop('disabled');
    if (isdisable) {
        //$('#txtKneadingNo').change();
        getCodeNamebycmNo();
        $.unblockUI();
    }


});

$().ready(function () {
    $.validator.unobtrusive.parse('#searchForm');
    $("#txtKneadingNo").focusout(function () {
        $("#txtKneadingNo").val($("#txtKneadingNo").val().trim());
    });
});



function search() {
    if ($("#searchForm").valid()) {
        var request = {
          commandCode: $("#txtKneadingNo").val(),
      };      
      gridHelper.searchObject("Grid", request);
      $("#btnStart").prop("disabled", true);
    }     
}

function getCodeNamebycmNo() {
    var isDisabled = $('#txtKneadingNo').prop('disabled');
    $.ajax({
        url: urlForm.urlgetCodeNamebycmNo,
        cache: false,
        sync: false,
        data: {
            cmdNo: $("#txtKneadingNo").val()
        },
        type: 'POST',
        success: function (data) {
            if (data.Success) {
                $('#txtPreProductCode').val(data.F42_PreProductCode);
                $('#txtPreproductName').val(data.F03_PreProductName);
                $('#btnSearch').prop("disabled", false);
                //
                $("#btnStart").prop("disabled", true);
                var items = gridHelper.getListingData("Grid");
                items.splice(0);
                $("#Grid").jsGrid("option", "data", items);
                $("#Grid").find('.jsgrid-pager-container').hide();
            } else {
                $('#txtPreProductCode').val("");
                $('#txtPreproductName').val("");
                if (isDisabled) {
                    $('#btnSearch').prop("disabled", true);
                }
                //
                $("#btnStart").prop("disabled", true);
                var items = gridHelper.getListingData("Grid");
                items.splice(0);
                $("#Grid").jsGrid("option", "data", items);
                $("#Grid").find('.jsgrid-pager-container').hide();
            }
        }
    });

}

function ClearSearch() {
    $("#txtKneadingNo").val("");
    $("#txtPreProductCode").val("");
    $("#txtPreproductName").val("");
    $("#txtKneadingNo").prop("disabled", false);
    $('#btnSearch').prop("disabled", false);
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
    window.location.href = 'InquiryKneadingLotNo?preLotNo=' + selectedKneadingCommand.F42_PrePdtLotNo.trim() + '&preCode=' + $('#txtPreProductCode').val() + '&preName=' + $('#txtPreproductName').val()



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

