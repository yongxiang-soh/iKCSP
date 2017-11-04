var hubList =  {
    c1: "Communication-C1",
    c2: "Communication-C2",
    c3: "Communication-C3",
    c4: "Communication-C4",
    noteNotification: 'NoteNotification'
}

/**
 * Initiate a connection to hub.
 * @returns {} 
 */
initiateHubConnection = function (hubName) {

    // Find the hub connection from connections collection.
    var signalrHubConnection = $.connection[hubName];
    return signalrHubConnection;
}

/**
 * Listen to C1 hub connection and display 
 * @param {} signalrHubConnection 
 * @returns {} 
 */
listenHubC1 = function(signalrHubConnection) {
    if (signalrHubConnection == null)
        return;

    signalrHubConnection.client.receiveMessageFromC1 = function (message) {
        toastr["info"](message);
    }
}

/**
 * Listen to C2 hub connection and display 
 * @param {} signalrHubConnection 
 * @returns {} 
 */
listenHubC2 = function (signalrHubConnection) {
    if (signalrHubConnection == null)
        return;

    signalrHubConnection.client.receiveMessageFromC2 = function (message) {
        toastr["info"](message);
    }
}

/**
 * Listen to C3 hub connection and display 
 * @param {} signalrHubConnection 
 * @returns {} 
 */
listenHubC3 = function (signalrHubConnection) {
    if (signalrHubConnection == null)
        return;

    signalrHubConnection.client.receiveMessageFromC3 = function (message) {

        console.log(message);
        toastr["info"](message);
    }
}

/**
 * Listen to C4 hub connection and display 
 * @param {} signalrHubConnection 
 * @returns {} 
 */
listenHubC4 = function (signalrHubConnection) {
    if (signalrHubConnection == null)
        return;

    signalrHubConnection.client.receiveMessageFromC4 = function (message) {
        toastr["info"](message);
    }
}

/**
 * Broadcast connection from hub.
 * @returns {} 
 */
signalrStartHubConnection = function() {
    
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 5000); // Restart connection after 5 seconds.
    });

    $.connection.hub.start().done();
}

/**
 * Construct html of message which should be displayed on the screen.
 * @param {} message 
 * @returns {} 
 */
findDisplayMessageC1 = function (message) {

    var status = 'Success';
    if (message.F34_Status == 'C' || message.F34_Status == '6')
        status = 'Success';
    else if (message.F34_Status == 'D')
        status = 'Cancel';
    else if (message.F34_Status == '8')
        status = 'Two times storage';

    var innerHtml = '';
    innerHtml += '<ul>';
    innerHtml += '<li>Retrieval from ' + message.F34_From + 'to ' + message.F34_To + '</li>';
    innerHtml += '<li>Material Code: ' + message.MaterialCode + '</li>';
    innerHtml += '<li>Pallet No.: ' + message.F34_PalletNo + '</li>';
    innerHtml += '<li>Status: ' + status + '</li>';
    innerHtml += '</ul>';

    return innerHtml;
}

findC1ResponseInformation = function(response) {

    // Invalid response.
    if (response == null)
        return;

    // Find list of items responded back from service.
    var items = response.Items;

    // Items list is invalid
    if (items == null || items.length < 1)
        return;

    for (var i = 0; i < items.length; i++) {
        toastr["info"](findDisplayMessageC1(items[i]));
    }

}


findC1ResponseInformation = function (response) {

    // Invalid response.
    if (response == null)
        return;

    // Find list of items responded back from service.
    var items = response.Items;

    // Items list is invalid
    if (items == null || items.length < 1)
        return;

    for (var i = 0; i < items.length; i++) {
        toastr["info"](findDisplayMessageC1(items[i]));
    }

}

findC2ResponseInformation = function(response) {
    // Invalid response.
    if (response == null)
        return;

    // Response is not an array of message.
    if (!(response instanceof Array))
        return;

    // Messages list is empty.
    if (response.length < 1)
        return;
    for (var i = 0; i < response.length; i++)
        toastr["info"](initiateSecondCommunicationResponseMessage(response[i]));

    // Invalid response.
    //if (response == null)
    //    return;

    //// Find list of items responded back from service.
    //var messages = response.Messages;

    //// Items list is invalid
    //if (messages == null || messages.length < 1)
    //    return;

    //for (var i = 0; i < messages.length; i++) {
    //    var innerHtml = '<ul>';
    //    innerHtml += '<li>' + messages[i] + '</li>';
    //    innerHtml += '</ul>';
    //    toastr["info"](innerHtml);
    //}
}
findC3ResponseInformation = function(response) {    
    // Invalid response.
    // Invalid response.
    if (response == null)
        return;

    // Response is not an array of message.
    if (!(response instanceof Array))
        return;

    // Messages list is empty.
    if (response.length < 1)
        return;

    for (var i = 0; i < response.length; i++)
        toastr["info"](initiateThirdCommunicationResponseMessage(response[i]));
}

/**
 * Construct html of message which should be displayed on the screen.
 * @param {} message 
 * @returns {} 
 */
initiateFirstCommunicationMessage = function (item) {


    var titleRetrieve = "Two Times";
    switch (item.F34_CommandNo) {
        case '1000':
            titleRetrieve = "Storage";
            break;
        case '2000':
            titleRetrieve = "Retrieval";
            break;
        case '3000':
            titleRetrieve = "Move";
            break;
        case '7000':
            titleRetrieve = "Stock taking of";
            break;
        case '6000':
            titleRetrieve = "Stock taking in";
            break;
        case '1001':
            titleRetrieve = "Two Times";
            break;
    }

    var titleStatus = 'Success';
    if (item.OldStatus == '6')
        titleStatus = 'Success';
    else if (item.OldStatus == '7')
        titleStatus = 'Cancel';
    else if (item.OldStatus == '8')
        titleStatus = 'Two times storage';
    else if (item.OldStatus == '9')
        titleStatus = 'Empty Retrieval';

    var innerHtml = '';
    innerHtml += '<ul>';
    innerHtml += '<li>' + titleRetrieve + ' from ' + item.F34_From + ' to ' + item.F34_To + '</li>';
    if (item.MaterialCode != null && item.MaterialCode != ""){
        innerHtml += '<li>Material Code: ' + item.MaterialCode + '</li>';
    }
    if (item.F34_PalletNo != "" && item.F34_PalletNo != null) {
        innerHtml += '<li>Pallet No.: ' + item.F34_PalletNo + '</li>';
    }
    innerHtml += '<li>Status: ' + titleStatus + '</li>';
    innerHtml += '</ul>';

    return innerHtml;
}

initiateSecondCommunicationResponseMessage = function (response) {
    // Invalid response.
    if (response == null)
        return;

    // Items list is invalid
    if (response == null || response.length < 1)
        return;

    var titleWarehouse = "Warehouse: Pre-product";
    var lsMessage = "";
    switch (response.F50_CommandNo) {
        case '1000':
            lsMessage = "Storage";
            break;
        case '2000':
            lsMessage = "Retrieval";
            break;
        case '4000':
            lsMessage = "Move";
            break;
        case '7000':
            lsMessage = "Stock talking off";
            break;
        case '6000':
            lsMessage = "Stock taking in";
            break;
        case '1001':
            lsMessage = "Two Times";
            break;
    }

    var titleStatus = 'Success';
    if (response.F50_Status == '6')
        titleStatus = 'Success.';
    else if (response.F50_Status == '7')
        titleStatus = 'Cancel.';
    else if (response.F50_Status == '8')
        titleStatus = 'Two times storage.';
    else if (response.F50_Status == '9')
        titleStatus = 'Empty Retrieval.';

    var innerHtml = '';
    innerHtml += '<ul>';
    innerHtml += '<li>' + titleWarehouse +'</li>';
    innerHtml += '<li>' + lsMessage + ' from ' + response.F50_From + ' to ' + response.F50_To + '</li>';
    innerHtml += '<li>Pre-product Code: ' + response.PreProductCode + '</li>';
    innerHtml += '<li>Container Code: ' + response.F50_ContainerCode + '</li>';
    innerHtml += '<li>Status: ' + titleStatus + '</li>';
        innerHtml += '</ul>';

    return innerHtml;
}

/**
 * From data returned back from service.
 * @returns {} 
 */
initiateThirdCommunicationResponseMessage = function (pdtWarehouseCommand, warehouseTitle, productCodeTitle, thirdControllerTitleRetrieve) {

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
  
    if (thirdControllerTitleRetrieve == null || typeof (thirdControllerTitleRetrieve) != 'string')
        thirdControllerTitleRetrieve = thirdNotificationMessage;

    var htmlMessage = '';
    htmlMessage += '<ul>';
    htmlMessage += '<li> Warehouse: ' + (warehouseTitle == null ? 'Product' : warehouseTitle) + '</li>';
    htmlMessage += '<li>' + thirdControllerTitleRetrieve + ' from ' + pdtWarehouseCommand.F47_From + ' to ' + pdtWarehouseCommand.F47_To + '</li>';
    htmlMessage += '<li>' + (productCodeTitle == null ? 'Product Code' : productCodeTitle) + ': ' + pdtWarehouseCommand.ProductCode + '</li>';
    htmlMessage += '<li>Pallet No: ' + pdtWarehouseCommand.F47_PalletNo + '</li>';

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