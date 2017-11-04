var signalrConnection = null;
var ib_instk = false;
var ib_outstk = false;
var palletNo = "";
var sheflno = "";
/**
 * Load product command details.
 * @returns {} 
 */
loadProductShippingCommandDetails = function (row, bay, level, palletNo, productLotNo) {
    var parameters = {
        shelfRow: row,
        shelfBay: bay,
        shelfLevel: level,
        palletNo: palletNo,
        productLotNo: productLotNo
    };

    gridHelper.searchObject("ProductShippingCommandDetailsGrid", parameters);
}

/**
 * Search product shipping commands
 * @returns {} 
 */
searchProductShippingCommand = function () {

    // Find the input shipping command.
    var parameters = {
        shippingNo: $("#ShippingNo").val()
    };

    // Find the shipping command no and display to grid.
    $("#btnGo").prop("disabled", true);

    gridHelper.searchObject("ProductShippingCommandGrid", parameters);
}

/**
 * Assign product shipping commands.
 * @returns {} 
 */
assignProductShippingCommand = function () {
    
    // No item has been selected.
    var item = findSelectedShippingCommand();
    if (item == null)
        return;
    if (confirm("Ready to assign?")) {
        // Disable all buttons of lower window.
        $("#btnPrint, #btnRestorage, #btnRetrieval, #btnUnassignProductShippingCommand").prop("disabled", true);
        $.ajax({
            url: "/ProductShippingCommand/AssignProductShippingCommands",
            type: "post",
            data: {
                shippingNo: item.F44_ShipCommandNo,
                productCode: item.F44_ProductCode,
                productLotNo: item.F44_ProductLotNo,
                shippingQuantity: parseFloat(item.F44_ShpRqtAmt),
                shippedAmount: parseFloat(item.F44_ShippedAmount)
            },
            success: function (response) {     
                if (response.Success != undefined && !Boolean(response.Success)) {
                    showMessage(response);
                    return;
                }
                // Find selected product command.
                var productCommandItem = findSelectedShippingCommand();
                if (productCommandItem == null)
                    return;
                ib_outstk = response.ib_outstk;
                ib_instk = response.ib_instk;
                sheflno = response.assignProductResult[0].ls_shelfno;
                //var lcassign = response.assignProductResult[0].lc_assign;
                //var shelfNo = response.assignProductResult[0].ls_shelfno;
                for (var i = 0; i < response.assignProductResult.length; i++) {
                    palletNo += response.assignProductResult[i].ls_palletno +"|";
                }   
                var parameters = {
                    productLotNo: productCommandItem.F44_ProductLotNo,
                    productCode: productCommandItem.F44_ProductCode,
                    requestAmount: (productCommandItem.F44_ShpRqtAmt - productCommandItem.F44_ShippedAmount),
                    palletNo :palletNo,
                    shelfNo: sheflno
                };
                
                gridHelper.searchObject("ProductShippingCommandDetailsGrid", parameters);
                gridHelper.ReloadGrid("ProductShippingCommandDetailsGrid");
                //// Load product shipping details.
                //loadProductShippingCommandDetails(item.row,
                //    item.bay,
                //    item.level,
                //    item.ls_palletno,
                //    productCommandItem.F44_ProductLotNo);
            }
        });
    }
}


/**
 * Unassign a specific shipping command.
 * @returns {} 
 */
unassignProductShippingCommand = function () {

    if (!confirm("Ready to de-assign?"))
        return;

    // Find the selected item on the grid.
    var selectedShippingCommand = findSelectedShippingCommand();
    if (selectedShippingCommand == null)
        return;

    $.ajax({
        url: "/ProductShippingCommand/UnassignShippingCommands",
        type: "post",
        data: {
            shippingNo: selectedShippingCommand.F44_ShipCommandNo
        },
        success: function (response) {
            //•	Reload the page, clear content of Data window 2 and set Total textbox to blank.
            //•	Enable button for Data window 1.
            //•	Disable all buttons of Data window 2.
            //window.location.reload();
            // Disable all button window 2
            // Trigger clear data of window 2
            $("#ProductShippingCommandDetailsGrid").jsGrid("option", "data", []);
            //	Enable “Assign” and “Go” buttons.
            $("#btnAssign, #btnGo").prop("disabled", false);
            $("[type='radio'][name*='ProductShippingCommandGrid']").attr('disabled', false);
            $("#btnPrint").prop("disabled", true);
            $("#btnRestorage").prop("disabled", true);
            $("#btnRetrieval").prop("disabled", true);
            $("#btnUnassignProductShippingCommand").prop("disabled", true);
            $("#Total").val("0.00");
        }
    });
}


/**
 * Callback of Product Shipping Commands when the data has been loaded.
 * @param {} data 
 * @returns {} 
 */
onProductShippingDataLoaded = function (response) {

    var shippingCommands = response.data;
    if (shippingCommands == null || shippingCommands.length < 1) {
        var notification = {
            Message: "Cannot find corresponding records from database!",
            Success: false
        };

        showMessage(notification);
        return;
    }

    // Disable assign button.
    $("#btnAssign").prop("disabled", true);
    $("#ProductShippingCommandGrid")
       .find("tr")
       .removeAttr("onclick")
       .attr("onclick", "productShippingCommandItemClick()");

}

/**
 * Find the selected shipping command item from the grid by using radio button status.
 * @returns {} 
 */
findSelectedShippingCommand = function () {
    return gridHelper.findSelectedRadioItem("ProductShippingCommandGrid");
}

/**
 * Fired when document is rendered on browser.
 */
$(document).ready(function (e) {
    /**
     * Allow radio button is selected when grid row is clicked.
     */
    /**
     * Callback which is called when shipping command item is clicked.
     */
    $("#ProductShippingCommandGrid")
        .find("tr")
        .removeAttr("onclick")
        .attr("onclick", "productShippingCommandItemClick()");
});

/**
 * This function called when product shipping command item is clicked on.
 * @returns {} 
 */
productShippingCommandItemClick = function () {
    var shippingCommandItem = findSelectedShippingCommand();
    if (shippingCommandItem == null)
        return;
    if (gridHelper.getListingData("ProductShippingCommandDetailsGrid").length < 1) {
        $("#btnAssign").prop("disabled", false);
    }
   


    
}

/**
 * This function is called when product shipping command details are loaded.
 * @returns {} 
 */
loadProductShippingCommandDetails = function() {

    // Display the lower window as data is available.
    $("#productShippingDetailsContainer").show();
    if (gridHelper.getListingData("ProductShippingCommandDetailsGrid").length > 0) {
        //	Enable “Print”, “Retrieval”, “De-assign” buttons.
        $("#btnPrint").prop("disabled", false);

        //	Disable “Assign” and “Go” buttons.
        $("#btnAssign, #btnGo").prop("disabled", true);
        $("#productShippingDetailsContainer").find("tr").click(function() {
            $("#btnRestorage, #btnRetrieval, #btnUnassignProductShippingCommand").prop("disabled", false);
        });
        //	Disable Data window 1.
        $("[type='radio'][name*='ProductShippingCommandGrid']").attr('disabled', 'disabled');
        var sum = gridHelper.getListingData("ProductShippingCommandDetailsGrid").reduce(function(total, num) {
            return total += num.F40_Amount;
        }, 0);
        $("#Total").autoNumeric('set', sum);
    }
}

productShippingCommandDetailsRefreshed = function(event) {
    var data = event.grid.data;

    if (data == null) {
        //	Disable “Print”, “Retrieval”, “De-assign” buttons.
        $("#btnPrint, #btnRetrieval, #btnUnassignProductShippingCommand").prop("disabled", true);

        //	Enable “Assign” and “Go” buttons.
        $("#btnAssign, #btnGo").prop("disabled", false);
        $("[type='radio'][name*='ProductShippingCommandGrid']").attr('disabled', false);
        //	Clear content of Total textbox.
        $(selector).autoNumeric('set', 0);

        //	Enable Data window 1 and show the message MSG 1.
        $(".data-window-1").find("input, button").prop("disabled", false);

        // Display the lower window as data is available.
        $("#productShippingDetailsContainer").hide();

        var notification = {
            Message: "Cannot find corresponding records from database!",
            Success: false
        };

        showMessage(notification);
    }
}

Restorage = function () {


    window.location = "/ProductManagement/RestorageOfProduct";
};

/**
 * Export shipping command by using specific conditions.
 * @returns {} 
 */
exportShippingCommand = function () {
    if (!confirm("Ready to print?")) {
        return;
    }
    
    // Find selected shipping command from grid.
    //var shippingCommand = findSelectedShippingCommand();
    var shippingCommand = gridHelper.getSelectedItem("ProductShippingCommandGrid");
    if (shippingCommand == null)
        return;
    $.ajax({
        url: "/ProductShippingCommand/ExportProductShippingCommands",
        data: {
            productCode: shippingCommand.F44_ProductCode,
            productName: shippingCommand.F09_ProductDesp,
            productLotNo: shippingCommand.F44_ProductLotNo,
            shippingNo: shippingCommand.F44_ShipCommandNo,
            settings: gridHelper.getSearchCondition("ProductShippingCommandGrid"),
            lstPalletNo: palletNo,
            shelfNo: sheflno
        },
        type: "post",
        success: function (response) {

            var render = response.render;

            if (render != null) {

                $("#productShippingCommandPrintArea")
                    .html(render)
                    .show()
                    .print()
                    .empty()
                    .hide();
            }
        }
    });
}

//Retrieval=function() {
//    var shippingNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ShipCommandNo;
//    var productCode = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ProductCode;
//    var productLotNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ProductLotNo;
//    var palletNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F51_PalletNo;

//    var shelfNo = gridHelper.getSelectedItem("ProductShippingCommandDetailsGrid").ShelfNo;
//    var lstShelftNo = shelfNo.split('-');
//    var row = lstShelftNo[0];
//    var bay = lstShelftNo[1];
//    var level = lstShelftNo[2];
//    var message = "Ready to retrieve?";
//    if (ib_instk == true && ib_outstk == true) {
//        message = "Shipping product will be shipped from retrieved product and in-stock product!";
//    } else {
//        if (ib_instk ==  false) {
//            message = "Shipping product will be shipped from retrieved product!";
//        }
//    }
//    if (confirm(message)) {


//        $.ajax({
//            url: "/ProductShippingCommand/RetrieveProduct",
//            data: {
//                shippingNo: shippingNo,
//                productCode: productCode,
//                productLotNo: productLotNo,
//                row: row,
//                bay: bay,
//                level: level,
//                palletNo: palletNo
//            },
//            type: "post",
//            success: function(data) {

//                if (!data.Success) {
//                    showMessage(data);
//                    // showMessage(message);
//                    return;
//                }
//                //showMessage(data);
//                $.unblockUI();

//                // Wait for messages from C3.
//                blockScreenAccess('Waiting messages from C3');
//            }
//        });
//    }
//}

Retrieval = function () {    
    var shippingNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ShipCommandNo;
    var productCode = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ProductCode;
    var productLotNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ProductLotNo;
    //var palletNo = gridHelper.getSelectedItem("ProductShippingCommandGrid").F51_PalletNo;
    var palletNo = gridHelper.getSelectedItem("ProductShippingCommandDetailsGrid").F51_PalletNo;

    var shelfNo = gridHelper.getSelectedItem("ProductShippingCommandDetailsGrid").ShelfNo;
    var lstShelftNo = shelfNo.split('-');
    var row = lstShelftNo[0];
    var bay = lstShelftNo[1];
    var level = lstShelftNo[2];
    var message = "Ready to retrieve?";
    if (ib_instk == true && ib_outstk == true) {
        message = "Shipping product will be shipped from retrieved product and in-stock product!";
    } else {
        if (ib_instk == false) {
            message = "Shipping product will be shipped from retrieved product!";
        }
    }
    $.ajax({
        url: "/ProductShippingCommand/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            $.unblockUI();
            if (!data.Success) {                
                //showMessage(data, "", "");
                setTimeout(function () { showMessage(data, "ProductShippingCommandGrid", ""); }, 500);
                //if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                //    setTimeout(function () {
                //        var errorMessage2 = { Success: false, Message: data.Message2 }
                //        showMessage(errorMessage2, "", "");
                //    }, 4000);
                //}
                return;

            } else {

                if (confirm(message)) {
                    $.ajax({
                        url: "/ProductShippingCommand/RetrieveProduct",
                        data: {
                            shippingNo: shippingNo,
                            productCode: productCode,
                            productLotNo: productLotNo,
                            row: row,
                            bay: bay,
                            level: level,
                            palletNo: palletNo
                        },
                        type: "post",
                        success: function (data) {

                            if (!data.Success) {
                                showMessage(data);
                                // showMessage(message);
                                return;
                            }
                            $("#btnRetrieval").prop("disabled", true);
                            $("#btnUnassignProductShippingCommand").prop("disabled", true);
                            //showMessage(data);
                            $.unblockUI();

                            // Wait for messages from C3.
                            blockScreenAccess('Waiting messages from C3');
                        }
                    });
                }
            }


        }
    }); 

    //if (confirm(message)) {
    //    $.ajax({
    //        url: "/ProductShippingCommand/RetrieveProduct",
    //        data: {
    //            shippingNo: shippingNo,
    //            productCode: productCode,
    //            productLotNo: productLotNo,
    //            row: row,
    //            bay: bay,
    //            level: level,
    //            palletNo: palletNo
    //        },
    //        type: "post",
    //        success: function (data) {

    //            if (!data.Success) {
    //                showMessage(data);
    //                // showMessage(message);
    //                return;
    //            }
    //            //showMessage(data);
    //            $.unblockUI();

    //            // Wait for messages from C3.
    //            blockScreenAccess('Waiting messages from C3');
    //        }
    //    });
    //}
       
}


/**
 * Callback which is called when document is ready.
 */
//$(document).ready(function () {
$(function () {    
    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function(message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR041F')
            return;
        var prd = "";
        prd = gridHelper.getSelectedItem("ProductShippingCommandGrid").F44_ProductCode;
        // Unblock UI.
        $.unblockUI();

        // Respond from C3.
        $.ajax({
            url: '/ProductShippingCommand/RespondMessageC3',
            data: { productCode: prd },
            type: 'post',
            success: function (items) {

                if (items == null)
                    return;

                if (!(items instanceof Array))
                    return;

                for (var index = 0; index < items.length; index++)
                    toastr["info"](initiateThirdCommunicationResponseMessage(items[index]));
            }
        });
    }

    signalrStartHubConnection();
});