$(document).ready(function () {
    
});

$().ready(function () {    
    $("#txtProductCode").attr("readonly", true);
    $("#txtProductCode").focusout(function () {
        $("#txtProductCode").val($("#txtProductCode").val().trim());
    });
    $.validator.unobtrusive.parse('#searchForm');

});



function search() {
    
    if ($("#searchForm").valid()) {
      var request = {
          productCode: $("#txtProductCode").val(),
      };      
      gridHelper.searchObject("Grid", request);
    }     
}

function ClearSearch() {
    $("#txtProductCode").val("");
    $("#txtProductName").val("");
    $("#txtEndUserCode").val("");
    $("#txtEndUserNameuc12").val("");
       
    var items = gridHelper.getListingData("Grid");

    items.splice(0);
    $("#Grid").jsGrid("option", "data", items);
    $("#Grid").find('.jsgrid-pager-container').hide();

}

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

function LoadUser() {
    $.ajax({
        url: formUrl.urluser,
        cache: false,
        sync: false,
        data: {
            proCode: $("#txtProductCode").val(),
            proName: $("#txtProductName").val()
        },
        type: 'POST',
        success: function (data) {
            $('#txtEndUserCode').val(data.data.split("|")[0]);
            $('#txtEndUserNameuc12').val(data.data.split("|")[1]);
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


function granTotalLoaded() {
    $.ajax({
        url: formUrl.urlGrandTotal,
        cache: false,
        sync: false,
        data: {
            materialCode: $("#txtProductCode").val()
        },
        type: 'POST',
        success: function (data) {
            $('#Total').val(data.data);
        }
    });

}

