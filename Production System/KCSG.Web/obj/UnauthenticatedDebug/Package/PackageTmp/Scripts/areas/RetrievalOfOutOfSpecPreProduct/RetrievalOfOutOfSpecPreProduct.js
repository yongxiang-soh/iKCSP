$().ready(function () {
    $("#PalletGrid").on("change", "input[type=\"radio\"]", function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        if (counterChecked > 0) {
            var status = $("input[name='ShelfStatus']:checked").val();

            if (status == "0") {
                $("#btnStorage").attr("disabled", false);
                $("#btnRetrieval").attr("disabled", true);

            } else {
                $("#btnRetrieval").attr("disabled", false);
                $("#btnStorage").attr("disabled", true);
            }
        }

    });
});

function onChangeShelfStatus(el) {
    
    var status = $(el).val();
    var request = {
        status: status
    };
    gridHelper.searchObject("PalletGrid", request);
    $("#btnStorage").prop("disabled", true);
    $('#btnStorage, #btnRetrieval').prop('disabled', true);
}

function Storage(shelfNo, shelfStatus, emptyPallet) {
    if (!confirm("Ready to store?")) {
        $.unblockUI();
        return;
    };
    //if (confirm(message.MSG18)) {
        $.ajax({
            url: formUrl.urlStorage,
            type: "POST",
            data: {
                shelfNo: shelfNo,
                shelfStatus: shelfStatus,
                emptyPallet: emptyPallet
            },
            success: function (data) {
                if (!data.Success) {
                    //var message = analyzeExceptionMessage(data);
                    showMessage(data, "PalletGrid", "");
                    return false;
                }
                if (data.Success) {
                    //$("#btnSearch").prop("disabled", true);
                    $("#btnStorage").prop("disabled", true);                   
                    //showMessage(data, "PalletGrid", "");
                } 
                //onChangeShelfStatus("input#txtMaterialCodeSearch");
                blockScreenAccess('Waiting for messages from C3...');
                //setTimeout(function () { blockScreenAccess('Waiting for messages from C3...'); }, 500);
            }
        });
    //}

}

function clickRetrieveStorage() {

    $.ajax({
        url: "/RetrievalOfOutOfSpecPreProduct/CheckStatus",
        type: 'POST',
        //data: { palletNo: palletNo },
        success: function (data) {
            
            //$.unblockUI();
            if (!data.Success) {
                setTimeout(function () { showMessage(data, "PalletGrid", ""); }, 500);
                //if (data.Message2 !== null && data.Message2 !== "" && data.Message2 !== undefined) {
                //    setTimeout(function () {
                //        var errorMessage2 = { Success: false, Message: data.Message2 }
                //        showMessage(errorMessage2, "", "");
                //    }, 4000);
                //}
                //return;

            } else {
                var status = $("input[name='ShelfStatus']:checked").val();
                var item = gridHelper.getSelectedItem("PalletGrid");
                var shelfNo = item.ShelfNo;
                var shelfStatus = item.F51_ShelfStatus;
                var emptyPallet = true;
                if (status == "0") {
                    Storage(shelfNo, shelfStatus, emptyPallet);
                } else {
                    emptyPallet = false;
                    Retrieval(shelfNo, shelfStatus, emptyPallet);
                }

                var parameter = {
                    status: findOutOfSpecProductStatus()
                };
            }
        }
    });                            
}


//function clickRetrieveStorage1() {
//    $.ajax({
//        url: formUrl.urlValidate,
//        type: "POST",
//        success: function (data) {

//            if (!data.Success) {

//                // Find the actual message should be displayed on the screen.
//                var message = analyzeExceptionMessage(data);
//                showMessage(message);
//                showMessage(message, "PalletGrid", "");
//                return false;
//            }

//            var status = $("input[name='ShelfStatus']:checked").val();
//            var item = gridHelper.getSelectedItem("PalletGrid");
//            var shelfNo = item.ShelfNo;
//            var shelfStatus = item.F51_ShelfStatus;
//            var emptyPallet = true;
//            if (status == "0") {
//                Storage(shelfNo, shelfStatus, emptyPallet);
//            } else {
//                emptyPallet = false;
//                Retrieval(shelfNo, shelfStatus, emptyPallet);
//            }

//            var parameter = {
//                status: findOutOfSpecProductStatus()
//            };


//            // Reload grid.
//            //gridHelper.searchObject("PalletGrid", parameter);
//        },

//    });
//}

function Retrieval(shelfNo, shelfStatus, emptyPallet) {
    if (confirm(message.MSG33)) {

        $.ajax({
            url: formUrl.urlRetrieval,
            type: "POST",
            data: { shelfNo: shelfNo, shelfStatus: shelfStatus, emptyPallet: emptyPallet },
            success: function (data) {
                if (!data.Success) {
                    //var message = analyzeExceptionMessage(data);
                    showMessage(data, "PalletGrid", "");
                    return false;
                }
                if (data.Success) {                    
                    $("#btnRetrieval").prop("disabled", true);
                    //showMessage(data, "PalletGrid", "");
                }                
                blockScreenAccess('Waiting for messages from C3...');
                //setTimeout(function () { blockScreenAccess('Waiting for messages from C3...'); }, 500);

                //if (data.Success) {
                //    // Block the ui.
                //    blockScreenAccess('Waiting messages from C3...');
                //    return;
                //} else {
                //    showMessage(data);
                //}
              
            }
        });
    } else {
        $.unblockUI();
    }
}

// Find selected status.
findOutOfSpecProductStatus = function () {
    return $("input[name='ShelfStatus']:checked").val();
}


/**
 * Callback which is called when document is ready.
 */
$(function () {

    // Start connection to C3.
    var signalrConnection = initiateHubConnection(hubList.c3);

    signalrConnection.client.receiveMessageFromC3 = function (message) {

        // Message is not for this screen.
        if (message.PictureNo != 'TCPR111F')
            return;

        // Find the selected item on the grid.
        var item = gridHelper.findSelectedRadioItem("PalletGrid");

        if (item == null)
            return;

        var parameters = {
            shelfStatus: item.F51_ShelfStatus,
            shelfNo: item.ShelfNo
        };
        // Respond from C3.
        $.ajax({
            url: '/RetrievalOfOutOfSpecPreProduct/RespondMessageC3',
            type: 'post',
            data: parameters,
            success: function (items) {

                if (items == null || !(items instanceof Array))
                    return;

                for (var index = 0; index < items.length; index++) {

                    var item = items[index];
                    toastr["info"](initiateThirdCommunicationResponseToastr(item));

                    
                    if (item.F47_Status == 'E' && $("input[name='ShelfStatus']:checked").val() == '1') {
                        setTimeout(function () {
                            var message = {
                                Success: true,
                                Message: "There is no empty location available in the warehouse now!"
                            };

                            showMessage(message);
                        },
                        500);
                    }
                }
                gridHelper.ReloadGrid("PalletGrid");
            }
        });
    }

    // Start hub connection.
    signalrStartHubConnection();
});


/**
 * From data returned back from service.
 * @returns {} 
 */
initiateThirdCommunicationResponseToastr = function (pdtWarehouseCommand) {

    var thirdNotificationMessage = "Two Times";
    switch (pdtWarehouseCommand.F47_CommandNo) {
        case '1000':
            thirdNotificationMessage = "Storage";
            break;
        case '2000':
            thirdNotificationMessage = "Retrieval";
            break;
        case '3000':
            thirdNotificationMessage = "Move";
            break;
        case '7000':
            thirdNotificationMessage = "Stock taking off";
            break;
        case '6000':
            thirdNotificationMessage = "Stock taking in";
            break;
        case '1001':
            thirdNotificationMessage = "Two Times";
            break;
    }


    var htmlMessage = '';
    htmlMessage += '<ul>';
    htmlMessage += '<li> Warehouse: Product</li>';
    htmlMessage += '<li>' + thirdNotificationMessage + ' from ' + pdtWarehouseCommand.F47_From + ' to ' + pdtWarehouseCommand.F47_To + '</li>';
    htmlMessage += '<li> Out-of-sign Pre-product </li>';

    if (pdtWarehouseCommand.OldStatus == '6')
        htmlMessage += '<li>Status: success</li>';
    else if (pdtWarehouseCommand.OldStatus == '7')
        htmlMessage += '<li>Status: cancel</li>';
    else if (pdtWarehouseCommand.OldStatus == '9')
        htmlMessage += '<li>Status: two times storage</li>';
    else if (pdtWarehouseCommand.OldStatus == '8')
        htmlMessage += '<li>Status: two times storage</li>';

    htmlMessage += '</ul>';

    return htmlMessage;
}