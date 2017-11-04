
$("#modalSelect").keypress(function (e) {
    if (e.keyCode === 13) {
        searchKeyWord();
        return false;
    }
});

function searchKeyWord() {
    var id =
        $("#txtMaterialCode").val();
    
    var request = {
        ModelType: $("#ModelType").val(),
        KeyWord: $("#txtKeyWordSearch").val().trim()
    };
    if (id !== null || id !== "")
        request= {
            ModelType: $("#ModelType").val(),
            KeyWord: $("#txtKeyWordSearch").val().trim(),
            id:id
        }
    gridHelper.searchObject("SupplierGrid", request);
}




function selectPONo() {
    var pONoCode = gridHelper.getSelectedItem("SupplierGrid").F30_PrcOrdNo;
    var partialDeliver = gridHelper.getSelectedItem("SupplierGrid").F30_PrtDvrNo;
    
    $('#modalPONOSelect').modal('toggle');

    $('#txtPONoCode').val(pONoCode);
    $('#txtPartialDelivery').val(partialDeliver);
    

    $("#modalPONOSelect .modal-body").html("");
}
$("#modalPONOSelect .close").click(function () {
    $("#modalPONOSelect .modal-body").html("");
});

function selectSupplier() {
    $('#modalSupplierCodeSelect').modal('toggle');
    var supplierCode = gridHelper.getSelectedItem("SupplierGrid").F04_SupplierCode;
    var supplierName = gridHelper.getSelectedItem("SupplierGrid").F04_SupplierName;
    var supplierMax = gridHelper.getSelectedItem("SupplierGrid").F04_MaxLoadAmount;
    $("#txtSupplierCode").val(supplierCode).change();
    $("#txtSupplierName").val(supplierName);
    $("#txtSupplierMax").val(supplierMax);
    $("#modalSupplierCodeSelect .modal-body").html("");
}

$("#modalSupplierCodeSelect .close").click(function () {
    $("#modalSupplierCodeSelect .modal-body").html("");
});

function selectContainer() {
    $('#modalContainerTypeSelect').modal('toggle');
    var containerType = gridHelper.getSelectedItem("SupplierGrid").F08_ContainerType;
    var containerName = gridHelper.getSelectedItem("SupplierGrid").F08_ContainerName;
    $('#txtContainerType').val(containerType);
    $('#txtContainerName').val(containerName);
    $("#modalContainerTypeSelect .modal-body").html("");
}

$("#modalContainerTypeSelect .close").click(function () {
    $("#modalContainerTypeSelect .modal-body").html("");
});


function selectPreProduct() {    
    $('#modalPreProductCodeSelect').modal('toggle');
    var preProductCode = gridHelper.getSelectedItem("SupplierGrid").F03_PreproductCode;
    var preProductName = gridHelper.getSelectedItem("SupplierGrid").F03_PreproductName;
    $('#txtPreProductCode').val(preProductCode.trim());
    $("#txtPreproductName").val(preProductName);
    $('#txtPreProductCode').focus();
    $('#txtPreProductCode').trigger("change");
    $("#modalPreProductCodeSelect .modal-body").html("");
}
$("#modalPreProductCodeSelect .close").click(function () {
    $("#modalPreProductCodeSelect .modal-body").html("");
});



function selectProduct() {
    $('#modalProductCodeSelect').modal('toggle');
    var preProductCode = gridHelper.getSelectedItem("SupplierGrid").F09_ProductCode;
    var preProductName = gridHelper.getSelectedItem("SupplierGrid").F09_ProductDesp;
    $('#txtProductCode').val(preProductCode);
    $("#txtProductName").val(preProductName);
    if ($("#txtEndUserNameuc12").length > 0) {
        LoadUser();
    }
    $('#txtProductCode').focus();
    $("#modalProductCodeSelect .modal-body").html("");
    $('#txtProductCode').trigger("change");
}

$("#modalProductCodeSelect .close").click(function () {
    $("#modalProductCodeSelect .modal-body").html("");
});

function selectSupplementMaterial() {
    $('#modalSupplementMaterialCodeSelect').modal('toggle');
    var supMaterialCode = gridHelper.getSelectedItem("SupplierGrid").F15_submaterialcode;
    var supMaterialName = gridHelper.getSelectedItem("SupplierGrid").F15_materialdsp;
    var supMaterialUnit = gridHelper.getSelectedItem("SupplierGrid").F01_Unit;
    $('#txtSubMaterialCode').val(supMaterialCode);
    $('#txtSubMaterialName').val(supMaterialName);
    $('#txtSubMaterialUnit').val(supMaterialUnit);
    $("#txtSubMaterialCode").focus();
    $("#modalSupplementMaterialCodeSelect .modal-body").html("");
}

$("#modalSupplementMaterialCodeSelect .close").click(function () {
    $("#modalSupplementMaterialCodeSelect .modal-body").html("");
});

function selectEndUser() {
    $('#modalEndUserCodeSelect').modal('toggle');
    var endUserCode = gridHelper.getSelectedItem("SupplierGrid").F10_EndUserCode;
    var endUserName = gridHelper.getSelectedItem("SupplierGrid").F10_EndUserName;
    $('#txtEndUserCode').val(endUserCode);
    $('#txtEndUserName').val(endUserName);
    $('#txtEndUserCode').focus();

    $("#modalEndUserCodeSelect .modal-body").html("");

}

$("#modalEndUserCodeSelect .close").click(function () {
    $("#modalEndUserCodeSelect .modal-body").html("");
});


function selectMaterial() {    
    $('#modalMaterialCodeSelect').modal('toggle');
    var materialCode = gridHelper.getSelectedItem("SupplierGrid").F01_MaterialCode;
    var materialName = gridHelper.getSelectedItem("SupplierGrid").F01_MaterialDsp;
    var entrustedClass = gridHelper.getSelectedItem("SupplierGrid").F01_EntrustedClass;
    var packingunit = gridHelper.getSelectedItem("SupplierGrid").F01_PackingUnit;
    var unit = gridHelper.getSelectedItem("SupplierGrid").F01_Unit;
    var rtrPosCls = gridHelper.getSelectedItem("SupplierGrid").F01_RtrPosCls;

    var liquid = gridHelper.getSelectedItem("SupplierGrid").F01_LiquidClass;
    $('#txtLiquid').val(liquid);
    $('#txtMaterialCode').val(materialCode).change();
    $('#txtMaterialCode').focus();
    $('#txtMaterialName').val(materialName);    
    if (entrustedClass === '0') {
        $('#txtBailmentClass').val('Norm');
        } else {
        $('#txtBailmentClass').val('Bail');
    }
    
    $('.packunit').val(packingunit).change();
    $('#txtUnit').val(unit);
    if (rtrPosCls === '0' || rtrPosCls === '1' || rtrPosCls === '2' || rtrPosCls === '3') {
        $("input[name=CommWthMeasureSys][value=" + rtrPosCls + "]").prop('checked', true);
    } else {
        $("input[name=CommWthMeasureSys][value='0']").prop('checked', false);
        $("input[name=CommWthMeasureSys][value='1']").prop('checked', false);
        $("input[name=CommWthMeasureSys][value='2']").prop('checked', false);
        $("input[name=CommWthMeasureSys][value='3']").prop('checked', false);
        //$('#CommWthMeasureSys').prop('checked', false);
    }
    

    $("#modalMaterialCodeSelect .modal-body").html("");
}




$("#modalMaterialCodeSelect .close").click(function () {
    $("#modalMaterialCodeSelect .modal-body").html("");
});

function selectPalletNo() {
    $('#modalPalletNoSelect').modal('toggle');
   // var palletno = gridHelper.getSelectedItem("SupplierGrid").F32_PalletNo;
    var palletno = gridHelper.getSelectedItem("SupplierGrid").F40_PalletNo;
    

    $('#txtPalletNo').val(palletno).change();
    $("#modalPalletNoSelect .modal-body").html("");
}

$("#modalPalletNoSelect .close").click(function () {
    $("#modalPalletNoSelect .modal-body").html("");
});

function selectShelfNo() {
    $('#modalShelfNoSelect').modal('toggle');
    var shelfNo = gridHelper.getSelectedItem("SupplierGrid").ShelfNo;

    $('#txtShelfNo').val(shelfNo).change();;
    $("#modalShelfNoSelect .modal-body").html("");
}

$("#modalShelfNoSelect .close").click(function () {
    $("#modalShelfNoSelect .modal-body").html("");
});

function selectTabletising() {
    $('#modalTabletingLineSelect').modal('toggle');
    var tabletisingLine = gridHelper.getSelectedItem("SupplierGrid").F14_DeviceCode;
    var tabletisingName = gridHelper.getSelectedItem("SupplierGrid").F14_DeviceName;

    $('#txtTabletisingLine').val(tabletisingLine);
    $('#txtTabletisingName').val(tabletisingName);
    $("#modalTabletingLineSelect .modal-body").html("");
}
$("#modalTabletingLineSelect .close").click(function () {
    $("#modalTabletingLineSelect .modal-body").html("");
});


function selectOutOfPlanProduct() {
    $('#modalOutOfPlanProductCodeSelect').modal('toggle');
    var productCode = gridHelper.getSelectedItem("SupplierGrid").F09_ProductCode;
    var productName = gridHelper.getSelectedItem("SupplierGrid").F09_ProductDesp;
    var prePdtLotNo = gridHelper.getSelectedItem("SupplierGrid").F58_PrePdtLotNo;
    var productLotNo = gridHelper.getSelectedItem("SupplierGrid").F58_ProductLotNo;
    var packQuantity = gridHelper.getSelectedItem("SupplierGrid").F58_TbtCmdEndPackAmt;
    var fraction = gridHelper.getSelectedItem("SupplierGrid").F58_TbtCmdEndFrtAmt;
    var tableEnDate = gridHelper.getSelectedItem("SupplierGrid").F58_TbtEndDate;
    
    $('#txtProductCode').val(productCode);
    $('#txtProductName').val(productName);
    $('#PrePdtLotNo').val(prePdtLotNo);
    $('#ProductLotNo').val(productLotNo);
    $('#PackQuantity').val(packQuantity);
    $('#Fraction').val(fraction);
    $('#TableEnDate').val(tableEnDate);
    $("#modalOutOfPlanProductCodeSelect .modal-body").html("");
}

$("#modalOutOfPlanProductCodeSelect .close").click(function () {
    $("#modalOutOfPlanProductCodeSelect .modal-body").html("");
});

function selectProductLotNo() {
    $('#modalProductLotNoSelect').modal('toggle');
    var productLotNo = gridHelper.getSelectedItem("SupplierGrid").F40_ProductLotNo;

    $('#txtProductLotNo').val(productLotNo);
    $("#modalProductLotNoSelect .modal-body").html("");
}

$("#modalProductLotNoSelect .close").click(function () {
    $("#modalProductLotNoSelect .modal-body").html("");
});

function selectPalletNoWithParameter() {    
    $('#modalPalletNoWithParameter').modal('toggle');
    var palletNo = gridHelper.getSelectedItem("SupplierGrid").F33_PalletNo;
    var materialCode = gridHelper.getSelectedItem("SupplierGrid").F33_MaterialCode;
    var materialName = gridHelper.getSelectedItem("SupplierGrid").MaterialName;
    var packingunit = gridHelper.getSelectedItem("SupplierGrid").F01_PackingUnit;
    var unit = gridHelper.getSelectedItem("SupplierGrid").F01_Unit;

    $('#txtPalletNo').val(palletNo).change();
    $('#txtMaterialCode').val(materialCode);
    $('#txtMaterialName').val(materialName);
    $('.packunit').val(packingunit).change();
    $('#txtUnit').val(unit);
    $("#modalPalletNoWithParameter .modal-body").html("");
}

$("#modalPalletNoWithParameter .close").click(function () {
    $("#modalPalletNoWithParameter .modal-body").html("");
});

function selectPONoWithParameter() {
    
    $('#modalPONOSelectWithParameter').modal('toggle');
    var pONoCode = gridHelper.getSelectedItem("SupplierGrid").F30_PrcOrdNo;
    var partialDeliver = gridHelper.getSelectedItem("SupplierGrid").F30_PrtDvrNo;
    var materialCode = gridHelper.getSelectedItem("SupplierGrid").F33_MaterialCode;
    var materialName = gridHelper.getSelectedItem("SupplierGrid").MaterialName;
    var packingunit = gridHelper.getSelectedItem("SupplierGrid").F01_PackingUnit;


    $('#txtPONoCode').val(pONoCode);
    $('#txtPartialDelivery').val(partialDeliver);
    $('#txtMaterialCode').val(materialCode);
    $('#txtMaterialName').val(materialName);
    $('.packunit').val(packingunit).change();
    $("#modalPONOSelectWithParameter .modal-body").html("");
}

$("#modalPONOSelectWithParameter .close").click(function () {
    $("#modalPONOSelectWithParameter .modal-body").html("");
});

function selectProductLotNoParameter() {

    $('#modalProductLotNoParameter').modal('toggle');
    var productLotNo = gridHelper.getSelectedItem("SupplierGrid").F40_ProductLotNo;

    $('#txtProductLotNo').val(productLotNo);
    $("#modalProductLotNoParameter .modal-body").html("");
}

$("#modalProductLotNoParameter .close").click(function () {
    $("#modalProductLotNoParameter .modal-body").html("");
});

// Select a label whose information should be displayed for printing.
function selectLabelPrint() {
    $('#modalLabelPrintManagement').modal('toggle');

    var item = gridHelper.getSelectedItem("SupplierGrid");
    if (item == null) {
        return;
    }

    console.log(item);
    $('#txtProductCode').val(item.F09_ProductCode);
    $('#txtProductName').val(item.F09_ProductDesp);
    $('#ExternalModelName,#InternalModelName,#txtProductName').val(item.F09_Label);
    $('#Size1').val(item.F09_TabletSize.trim());
    $('#Size2').val(item.F09_TabletSize2.trim());
    $('#InternalSize1').val(item.F09_TabletSize.trim());
    $('#InternalSize2').val(item.F09_TabletSize2.trim());
    $('#CmdNo').val(item.F42_KneadingCommand);
    $('#ExternalLabelType,#InternalLabelType').val(item.F09_TabletType);
    //$('#ExternalLabelType').val(item.F09_TabletType);
    //$('#InternalLabelType').val(item.F09_TabletType);
    $('#ShelfLife').val(item.F09_ValidPeriod.trim());
    ChangeDate();
    $("#modalLabelPrintManagement .modal-body").html("");
}

$("#modalLabelPrintManagement .close").click(function () {
    $("#modalLabelPrintManagement .modal-body").html("");
});