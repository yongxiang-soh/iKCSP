var stockTakingProductItem = {
    row: null,
    bay: null,
    level: null,
    palletNo: null,
};

$(function () {

    $("#ShelfNoFrom")
        .mask("99-99-99");

    $("#ShelfNoTo")
        .mask("99-99-99");

    resetShelfNoStart();
    resetShelfNoEnd();
});


/**
 * Search stock taking of product.
 * @returns {} 
 */
searchStockTakingOfProduct = function () {

    // 
    if ($("#indexForm").valid()) {
        // Find shelf start.
        var shelfStart = $("#ShelfNoFrom").val();
        var shelfEnd = $("#ShelfNoTo").val();

        // By default, treat the data is solid.
        var isDataCorrectFrom = true;
        var isDataCorrectTo = true;

        if (!isShelfNoValid(shelfStart)) {
            resetShelfNoStart();
            isDataCorrectFrom = false;
        }

        if (!isShelfNoValid(shelfEnd)) {
            resetShelfNoEnd();
            isDataCorrectTo = false;
        }

        //var notification = {
        //    Success: false
        //}

        if (!isDataCorrectFrom) {
            //notification["#errorlist"] = "Shelf no. error!";
            //showMessage(notification);
            $("#errorlist01").text("Shelf no. error!").show();
            return;
        }
        $('#errorlist01').hide();
        if (!isDataCorrectTo) {
            //notification["#errorlist"] = "Shelf no. error!";
            //showMessage(notification);
            $("#errorlist02").text("Shelf no. error!").show();
            return;
        }
        $('#errorlist02').hide();


        var startShelfNo = shelfStart.split("-");
        var endShelfNo = shelfEnd.split("-");
        if (!isShelfNoRangeValid(startShelfNo, endShelfNo)) {
            resetShelfNoStart();
            $("#errorlist01").text("From location is bigger than To location!").show();
            //notification["#errorlist"] = "\"From location\" is bigger than \"To location!\"";
            //showMessage(notification);
            return;
        }
        $('#errorlist').hide();

        var parameters = {
            shelfRowFrom: startShelfNo[0],
            shelfBayFrom: startShelfNo[1],
            shelfLevelFrom: startShelfNo[2],
            shelfRowTo: endShelfNo[0],
            shelfBayTo: endShelfNo[1],
            shelfLevelTo: endShelfNo[2]

        }

        gridHelper.searchObject("StockTakingOfProductGrid", parameters);
    }
}

function disabledSearch() {
    //var grid = $("#StockTakingOfProductGrid.ClientID");
    //if (grid.length > 0) {
    //	Disable “Go” button.
    if (gridHelper.getListingData("StockTakingOfProductGrid").length > 0) {
        $("#btnGo").prop("disabled", true);

        //	Disable 2 textbox
        $("#ShelfNoFrom").prop("disabled", true);
        $("#ShelfNoTo").prop("disabled", true);

        //	Enable 2 button
        $("#btnRetrieval, #btnClear").prop("disabled", false).removeAttr("disabled");

        var item = gridHelper.getSelectedItem("StockTakingOfProductGrid");
        if (item) {
            // get fisrt item for Stock-taking of Product Confirm
            var location = item.F51_ShelfRow + '-' + item.F51_ShelfBay + '-' + item.F51_ShelfLevel;
            var palletNo = item.F51_PalletNo;
            $('#locationfirst').val(location);
            $('#palletNofirst').val(palletNo);
        }


        //An da sua
    }

    //}               
}

/**
 * Search product details by using pallet no.
 * @returns {} 
 */
searchProductDetails = function () {

    // Find the selected item from grid.
    var selectedProduct = findSelectedProduct();

    // Product is invalid. Do nothing.
    if (selectedProduct == null)
        return;

    $.ajax({
        url: "/StockTakingOfProduct/FindProductDetails",
        type: "post",
        data: {
            palletNo: selectedProduct.F51_PalletNo
        },
        success: function (response) {
            var productDetails = response.productsDetail;

            if (productDetails == null || productDetails.length < 1) {
                $("#productDetailContainer").hide();
                //$("#btnRetreival, #btnClear").prop("disabled", true);                
            } else {

                var productCodeInnerHtml = "";
                var productNameInnerHtml = "";
                var productLotNoInnerHtml = "";
                var productAmountInnerHtml = "";

                //sss
                for (var i = 0; i < productDetails.length; i++) {
                    var productDetail = productDetails[i];

                    productCodeInnerHtml += "<input ";
                    productCodeInnerHtml += "class='form-control form-group' readonly='readonly'";
                    productCodeInnerHtml += "type='text' ";
                    productCodeInnerHtml += "value='" + productDetail.F40_ProductCode + "' ";
                    productCodeInnerHtml += "/>";

                    productNameInnerHtml += "<input ";
                    productNameInnerHtml += "class='form-control form-group' readonly='readonly'";
                    productNameInnerHtml += "type='text' ";
                    productNameInnerHtml += "value='" + (productDetail.F09_ProductDesp != null ? productDetail.F09_ProductDesp : '') + "' ";
                    productNameInnerHtml += "/>";

                    productLotNoInnerHtml += "<input ";
                    productLotNoInnerHtml += "class='form-control form-group' readonly='readonly'";
                    productLotNoInnerHtml += "type='text' ";
                    productLotNoInnerHtml += "value='" + productDetail.F40_ProductLotNo + "' ";
                    productLotNoInnerHtml += "/>";

                    productAmountInnerHtml += "<div class='input-group form-group'>";
                    productAmountInnerHtml += "<input ";
                    productAmountInnerHtml += "class='form-control' readonly='readonly' ";
                    productAmountInnerHtml += "type='number' style='text-align: right' ";
                    productAmountInnerHtml += "value='" + productDetail.F40_Amount + "' ";
                    productAmountInnerHtml += "/>";
                    productAmountInnerHtml += "<span class='input-group-addon'>.00</span>";
                    productAmountInnerHtml += "</div>";

                }

                $("#divProductCode").empty().append(productCodeInnerHtml);
                $("#divProductName").empty().append(productNameInnerHtml);
                $("#divLotNo").empty().append(productLotNoInnerHtml);
                $("#divQuantity").empty().append(productAmountInnerHtml);

                $("#btnRetrieval, #btnClear").prop("disabled", false).removeAttr("disabled");
                $("#productDetailContainer").show();

            }
        }
    });
}

/**
 * Clear lower table data.
 * @returns {} 
 */
clearProductDetails = function (event) {

    //	Set default value for [Shelf No From] and [Shelf No To] textboxes and enable them
    resetShelfNoStart();
    resetShelfNoEnd();

    //	Enable “Go” button.
    $("#btnGo").prop("disabled", false);
    $("#ShelfNoFrom").prop("disabled", false);
    $("#ShelfNoTo").prop("disabled", false);

    //	Disable “Clear” and “Retrieval” 
    $("#btnRetrieval, #btnClear").prop("disabled", true);

    //	Remove all rows from Data Window, 
    $("#productDetailContainer").hide();

    //Clear grid    
    var items = gridHelper.getListingData("StockTakingOfProductGrid");

    items.splice(0);
    $("#StockTakingOfProductGrid").jsGrid("option", "data", items);
    $("#StockTakingOfProductGrid").find('.jsgrid-pager-container').hide();

    //var items = gridHelper.getListingData("StockTakingOfProductGrid");
    //itemRemove = items.splice(0);
    //$("#StockTakingOfProductGrid").jsGrid("option", "data", items);    
}


/**
 * Retrieve product details.
 * @returns {} 
 */
retrievalProductDetails = function () {
    // Find selected product.
    //var product = findSelectedProduct();
    var product = gridHelper.getSelectedItem("StockTakingOfProductGrid");

    if (product == null)
        return;



    $.ajax({
        url: "/StockTakingOfProduct/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            //
            //$.unblockUI();
            if (!data.Success) {
                setTimeout(function () { showMessage(data, "StockTakingOfProductGrid", ""); }, 500);
                //if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                //    setTimeout(function () {
                //        var errorMessage2 = { Success: false, Message: data.Message2 }
                //        showMessage(errorMessage2, "", "");
                //    }, 4000);
                //}
                //return;

            } else {
                if (!confirm("Ready to retrieve?"))
                    return;

                // Update stock taking of product item.
                //stockTakingProductItem = {
                //    palletNo: product.F51_PalletNo,
                //    row: product.F51_ShelfRow,
                //    bay: product.F51_ShelfBay,
                //    level: product.F51_ShelfLevel
                //};

                $.ajax({
                    url: "/StockTakingOfProduct/RetrieveProductDetails",
                    type: "post",
                    data: {
                        palletNo: product.F51_PalletNo,
                        row: product.F51_ShelfRow,
                        bay: product.F51_ShelfBay,
                        level: product.F51_ShelfLevel
                    },

                    success: function (data) {
                        // Disable buttons.            
                        if (!data.Success) {
                            //alert(data.Message[0]);                
                            showMessage(data, "StockTakingOfProductGrid", "");
                            return;
                        }
                        $("#btnClear, #btnRetrieval").prop('disabled', true);
                        //alert("Shelf No [" + row + bay + level + "] retrieving ...");

                        blockScreenAccess('Waiting messages from C3');
                    },

                    error: function (response) {
                        $("#btnClear, #btnRetrieval").prop('disabled', false);
                    }
                });
            }
        }
    });
}

/**
 * Fired when document is rendered on browser.
 */
$(document).bind('DOMNodeInserted', function (e) {
    /**
     * Callback which is called when shipping command item is clicked.
     */
    $("#StockTakingOfProductGrid")
        .find("tr")
        .removeAttr('onclick')
        .attr('onclick', 'searchProductDetails()');
});

/**
 * Check whether shelf no is valid or not base on the defined conditions.
 * @param {} shelfNo 
 * @returns {} 
 */
isShelfNoValid = function (shelfNo) {
    /*
    	If length of [Shelf No From] are not 6 or 8, 
    OR if Row 1 < "01", 
    OR Row 1 > "02", 
    OR Bay 1 < "01", 
    OR Bay 1 > "09", 
    OR Level 1 < "01", 
    OR Level 1 > "08":
    o	Set [Shelf No From] to default value.
    o	System shows the message MSG 53 and stops the use case.
    */
    if (shelfNo == null || (shelfNo.length != 6 && shelfNo.length != 8)) {
        //$("#errorList").text(validateMessage.MSG49).show();
        resetShelfNoStart();
        return false;
    }

    var infos = shelfNo.split("-");
    if (infos.length != 3) {
        //$("#errorList").text(validateMessage.MSG49).show();
        resetShelfNoStart();
        return false;
    }

    if (infos[0] < '01' || infos[0] > '02' || infos[1] < '01' || infos[1] > '09' || infos[2] < '01' || infos[2] > '08') {
        //$("#errorList").text(validateMessage.MSG49).show();
        resetShelfNoStart();
        return false;
    }

    return true;
}

/**
 * Reset shelf no start to default value.
 * @returns {} 
 */
resetShelfNoStart = function () {
    $("#ShelfNoFrom").val("01-01-01");
}

/**
 * 
 * @returns {} 
 */
resetShelfNoEnd = function () {
    $("#ShelfNoTo").val("02-09-08");
}



/**
 * Whether start shelf no is smaller than end shelf no or not.
 * @param {} startShelfNo 
 * @param {} endShelfNo 
 * @returns {} 
 */
isShelfNoRangeValid = function (startShelfNo, endShelfNo) {
    var totalStartShelfNo = startShelfNo.reduce(function (a, b) { return parseInt(a) + parseInt(b) }, 0);
    var totalEndShelfNo = endShelfNo.reduce(function (a, b) { return parseInt(a) + parseInt(b) }, 0);

    return totalStartShelfNo <= totalEndShelfNo;
}

/**
 * Find the selected product from grid.
 * @returns {} 
 */
findSelectedProduct = function () {
    return gridHelper.findSelectedRadioItem("StockTakingOfProductGrid");
}

function validateShelfNo(shelfNoFrom, shelfNoTo) {
    if (!validateShelfIntValue(shelfNoFrom) || !validateShelfIntValue(shelfNoTo)) {
        resetShelves();
        return false;
    }
    var shelfNoFromIntVal = parseInt(shelfNoFrom);
    var shelfNoToIntVal = parseInt(shelfNoTo);
    if (shelfNoFromIntVal > shelfNoToIntVal) {
        $("#errorList").text(validateMessage.MSG50).show();
        return false;
    }
    return true;
}

function onSuccess(data, status, xhr) {
    var a = 1;
    if (data.Success) {
        blockScreenAccess('Waiting messages from C3...');
    }
}


/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {
        if (message.PictureNo != 'TCPR061F' && message.PictureNo != 'TCPR062F')
            return;



        var item = findStockTakingItem();
        if (item == null)
            return;

        if (message.WarehouseItem.F47_CommandNo == '6000') {

            if (item.row == null || item.bay == null || item.level == null)
                return;

            // Unblock UI.
            $.unblockUI();

            $.ajax({
                url: '/ProductManagement/StockTakingOfProduct/RespondRestore',
                type: 'post',
                data: item,
                success: function (data) {
                    if (!(data instanceof Array))
                        return;

                    for (var index = 0; index < data.length; index++) {
                        toastr["info"](initiateThirdCommunicationResponseMessage(data[index]));
                    }

                    var scope = angular.element($("#panelConfirmStockTaking")).scope();
                    scope.location = null;
                    scope.$applyAsync();

                    gridHelper.ReloadGrid("StockTakingOfProductGrid");
                }
            });
        } else {

            // Unblock UI.
            $.unblockUI();

            $.ajax({
                url: '/ProductManagement/StockTakingOfProduct/RespondRetrieve',
                type: 'post',
                success: function (data) {
                    if (!(data instanceof Array))
                        return;

                    var isDisplayed = false;

                    for (var index = 0; index < data.length; index++) {
                        toastr["info"](initiateThirdCommunicationResponseMessage(data[index], null, null, 'Retrieval'));
                        if (isDisplayed)
                            continue;

                        if (data[index].OldStatus == '6') {
                            var scope = angular.element($("#panelConfirmStockTaking")).scope();
                            scope.findStocksList(item);
                            scope.$applyAsync();
                            isDisplayed = true;
                        }

                    }

                }
            });
        }


    }

    // Start hub connection.
    signalrStartHubConnection();
});

/**
 * Find stock taking of product item.
 * @returns {} 
 */
findStockTakingItem = function () {

    var radioItem = gridHelper.getSelectedItem("StockTakingOfProductGrid");
    var item = {};

    item['row'] = radioItem.F51_ShelfRow;
    item['bay'] = radioItem.F51_ShelfBay;
    item['level'] = radioItem.F51_ShelfLevel;
    item['palletNo'] = radioItem.F51_PalletNo;
    //palletNofirst

    return item;

}