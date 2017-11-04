
$(document).ready(function () {    
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control Normal
    $('#Grid').on('change', 'input[type="radio"]', function () {
        
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });

    $("#YearMonth").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $("#YearMonth").keyup(function (e) {
        if (e.keyCode === 27 && $("#YearMonth").val() != "") {
            $("#YearMonth").val("");
            search();
            return false;
        }
    });        
});

function addNewProdShip() {
    $.ajax({        
        //url: formUrlProdShip.urlEdit,        
        url: "/ProductShippingPlanning/Edit",
        type: 'GET',
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F44_ShpRqtAmt').focus();            
            //  $('#F39_KndEptBgnDate').val('');
            disableButton();
            //$("input[name='F44_ProductCode']").rules("add", { CheckExistCode: true });
            //$("input[name='F44_ProductLotNo']").rules("add", { CheckExistCode: true });
        }
    });
}

function edit() {
    var date = gridHelper.displayDate($('#Grid').find('input:checked').val());
    var data = gridHelper.getSelectedItem('Grid');
    
    $.ajax({
        //url: formUrlProdCer.urlEdit,
        url: "/ProductShippingPlanning/Edit",
        type: 'GET',
        data: {
            //date: date,
            prodShipCode: data.F44_ShipCommandNo,
            proCode: data.F44_ProductCode,
            userCode: data.F44_EndUserCode,
            productLotNo: data.F44_ProductLotNo,
            reqShippingQty: data.F44_ShpRqtAmt
        },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F44_ShpRqtAmt').focus();

        }
    });
}

function deleteItemProdShip() {
    
    var date = gridHelper.displayDate($('#Grid').find('input:checked').val());
    //var data = gridHelper.getSelectedItem(gridid).F44_ShipCommandNo;
    var data = gridHelper.getSelectedItem('Grid');
    if (confirm("Ready to delete?")) {
        $.ajax({
            //url: formUrlPdtPln.urlDelete,
            url: "/ProductShippingPlanning/Delete",
            type: 'POST',
            data: {
                prodCode: data.F44_ShipCommandNo
            },
            success: function (data) {
                $("#btnDelete").prop("disabled", true);
                $("#btnUpdate").prop("disabled", true);
                showMessage(data, 'Grid', "");
                
            }
        });
    };
}

function hideFormProdShip() {
    $("#formPlaceholder").html("");
    enableButton("Grid");
}

function search() {
    var request = {
        YearMonth: $("#YearMonth").val(),
        KndLine: $("#KndLine").val()
    };
    if (checkSearch()) {
        if (confirm("Unsaved data will be lost. Are you sure to search?")) {
            hideFormPdtPln();
            gridHelper.searchObject("PdtPlnGrid", request);
        }
    } else {
        gridHelper.searchObject("PdtPlnGrid", request);
    }
}

function SelectProductCode(el) {
    var productCode = $(el).val();
    $.ajax({
        url: formUrlPdtPln.urlGetProductName,
        type: 'GET',
        data: { preProductCode: productCode },
        success: function (data) {
            //$("#txtPreProductName").val(data.result.F03_PreProductName);
            $("#F03_PreProductName").val(data.result.F03_PreProductName);
        }
    });
}

function onSuccess(data, status, xhr) {
    
    //if (!data.Success) {
    //    if (data.Message === "MSG1") {
    //        $("#txtProductLotNo").val("");
    //        $("#errorProCode").text("Cannot find corresponding records from database!").show();
    //        $("#errorProLotNo").text("Cannot find corresponding records from database!").show();
    //        return;
    //    }        
    //}

    var request = {
        MaterialCode: $("#txtShippingNoSearch").val()
    };
    showMessage(data, "Grid", request);
    enableButton("Grid");
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    $("#formPlaceholder").html("");
    

}

//function onSuccess(data, status, xhr) {    
//    var request = {
//        MaterialCode: $("#txtShippingNoSearch").val()
//    };
//    showMessage(data, "Grid", request);
//    $("#formPlaceholder").html("");
//    $("#btnDelete").prop("disabled", true);
//    $("#btnUpdate").prop("disabled", true);
//    enableButton("Grid");

//}



function searchProductShip() {
    
    //var request = { ShippingNo: $("#txtShippingNoSearch").val().trim() };
    var shippingNo = $('#txtShippingNoSearch').val().trim();
    var model = {        
        ShippingNo: shippingNo
    }
    if ($('#formPlaceholder').html() != "") {
        if (confirm("Unsaved data will be lost. Are you sure to search?")) {
            $("#formPlaceholder").html("");
            gridHelper.searchObject("Grid", model);
        }
    } else {
        gridHelper.searchObject("Grid", model);
    }
}


/**
 * Find the selected product certification on the screen.
 * @returns {} 
 */
findSelectedProductCertification = function (name) {
    return $("input[type='radio'][name='" + name + "']:checked").val();
}

//$.validator.addMethod("CheckExistCode",
//    function(value, element, params) {
//        var proCode = $("#F44_ProductCode").val();
//        var proLotNo = $("#F44_ProductLotNo").val();
//        $.ajax({                    
//            url: "/ProductShippingPlanning/CheckExistCode",
//            type: 'POST',
//            data: {                
//                f44_ProductCode: proCode,
//                f44_ProductLotNo: proLotNo
//            },
//            success: function(data) {
//                $("#F44_ProductLotNo").val("");
//                $("#errorProCode").text("Cannot find corresponding records from database!").show();
//                $("#errorProLotNo").text("Cannot find corresponding records from database!").show();
//            }
//        });

//    });

//function OnChange() {

//    var status = $('input[name=StorageOfProductStatus]:checked').val();    
//    //var data = gridHelper.getSelectedItem('Grid');
//    var request = {
//        proCode: $("#F44_ProductCode").val(),
//        productLotNo: $("#txtProductLotNo").val()
//    }    
    
//    $.ajax({        
//        //url: formUrlProdCer.urlEdit,
//        url: "/ProductShippingPlanning/TranferInterFloor",
//        type: 'POST',        
//        data: {
//            //date: date,            
//            proCode: request.proCode,
//            productLotNo: request.productLotNo
//        },
        
//        success: function (data) {
//            disableButton();
//            $("#formPlaceholder").html(data);
//            $('#F44_ShipCommandNo').focus();
//            if (!data.success) {
//                
//                $('#txtProductLotNo').val("")
//                //showMessage(data, "", "")                
//            }

//        }
//    });
//}



