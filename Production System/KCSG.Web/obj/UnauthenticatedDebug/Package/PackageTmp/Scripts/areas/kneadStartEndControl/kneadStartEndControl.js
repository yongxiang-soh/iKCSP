$(document).ready(function() {
    $("#btnDelete").prop("disabled", true);
    $('#KneadingStartEndControl').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        
    });
});


kneadingCommandsLoaded = function (grid) {

    //	If there is no data to display on the data window, system shows the message MSG 2
    if (grid == null || grid.data == null || grid.data.data.length < 1) {
        alert("Cannot find corresponding records from DB!");
        $("#btnStart").prop("disabled", true);
        $("#btnPause").prop("disabled", true);
        return;
    }
    
    $("#btnStart").prop("disabled", false);
    $("#btnPause").prop("disabled", false);
}

/**
 * This function is for interrupting kneading control commands list.
 * @returns {} 
 */
interruptKneadingCommands = function () {

    // System shows the confirmation message MSG 13. If user selects “No”, cancel Pause action; reload the current page
    if (!confirm("Ready to pause?")) {
        loadKneadingCommands();
        return;
    }

    // Find all selected items from kneading command.
    var selectedKneadingItems = gridHelper.getListingData("KneadingStartEndControl");
    var items = Object.assign(selectedKneadingItems);
    if (items == null || items.length < 1) {
        gridHelper.searchObject("KneadingStartEndControl");
        return;
    }

    for (var i = 0; i < items.length; i++) {
        var item = items[i];
        item.ProductionDate = analyzeDateTime(item.ProductionDate);
        item.UpdateDate2 = analyzeDateTime(item.UpdateDate2);
        item.UpdatedDate1 = analyzeDateTime(item.UpdatedDate1);
    }

    $.ajax({
        url: "/KneadingCommand/KneadingStartEnd/Interrupt",
        type: "post",
        contentType: 'application/json',
        data: JSON.stringify({
            kneadingCommands: items
        }),
        success: function (data) {
            //	Reload the current page by following UC 6: View Kneading Command.
            loadKneadingCommands();
        }
    });

}

/**
 * Stop kneading command process
 * @returns {} 
 */
stopKneadingCommand = function () {
    // Find the selected item.
    var selectedKneadingCommand = gridHelper.getSelectedItem("KneadingStartEndControl");

    // Copy selected kneading command to another variable to prevent it from being harmed.
    var item = Object.assign(selectedKneadingCommand);

    //	If the Kneading Status is different from “Not Kneaded” (or 0), then cancel the Stop action, reload the page
    if (item == null || item.KneadingStatus != 0) {
        loadKneadingCommands();
        return;
    }

    //	Then system shows the confirmation message MSG 11. 
    if (!confirm("Ready to delete?")) {
        //o	If user selects “No”, cancel Stop action; reload the current page. 
        loadKneadingCommands();
        return;
    }

    item.ProductionDate = analyzeDateTime(item.ProductionDate);
    item.UpdateDate2 = analyzeDateTime(item.UpdateDate2);
    item.UpdatedDate1 = analyzeDateTime(item.UpdatedDate1);

    $.ajax({
        url: "/KneadingCommand/KneadingStartEnd/Stop",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify({
            kneadingCommand: item
        }),
        success: function () {
            //	Reload the current page by following UC 6: View Kneading Command
            loadKneadingCommands();
            
        },
        error: function (data) {
            if (data.status === 400) {
                var processError = data.getResponseHeader("x-process-error");
                if (processError != null && processError.length > 0)
                    alert(processError);
            }
        }
    });
}

/**
 * Start kneading command process.
 * @returns {} 
 */
startKneadingCommand = function() {
  
    //System shows the confirmation message MSG 8. If user selects “No”, cancel Start action; reload the current page
    if (!confirm("Ready to go?"))
        return;

    // Find kneading items which are currently selected.
    var items = gridHelper.getListingData("KneadingStartEndControl");
    var commands = Object.assign(items);

    for (var itemIndex = 0; itemIndex < commands.length; itemIndex++) {
        var item = commands[itemIndex];
        commands[itemIndex].UpdatedDate1 = analyzeDateTime(item.UpdatedDate1);
        commands[itemIndex].UpdateDate2 = analyzeDateTime(item.UpdateDate2);
        commands[itemIndex].ProductionDate = analyzeDateTime(item.ProductionDate);
    }

    $.ajax({
        url: "/KneadingCommand/KneadingStartEnd/Start",
        type: "post",
        contentType: "application/json",
        data: JSON.stringify({
            item: {
                kneadingCommands: commands,
                kneadingLine: $('input[name="KneadingLine"][type="radio"]:checked').val()
            }
        }),
        success: function() {
            //	Reload the current page by following UC 6: View Kneading Command
            loadKneadingCommands();
        },
        error: function(data) {
            if (data.status === 400) {
                var processError = data.getResponseHeader("x-process-error");
                if (processError != null && processError.length > 0)
                    alert(processError);
            }
        }
    });
}

/**
 * Find value of radio button by using its name.
 * @returns {} 
 */
findSelectedKneadingLine = function (name) {
    var jCommand = "input[name='" + name + "'][checked='checked']";
    return $(jCommand).val();
}

/**
 * This function is for loading kneading commands list by using specific conditions.
 * @returns {} 
 */
loadKneadingCommands = function() {
    
    // Find the selected kneading command line.
    var kneadingCommandLine = findSelectedKneadingLine("KneadingLine");

    // Reload grid with specific filter conditions.
    var filter = {
        kneadingCommandLine: kneadingCommandLine
    };

    gridHelper.searchObject("KneadingStartEndControl", filter);
    $("#btnDelete").prop("disabled", true);
}

/**
 * This event is fired when kneading table is being clicked.
 * @returns {} 
 */
clickKneadingTable = function() {
    // Get the current selected item in the grid.
    var selectedKneadingItem = gridHelper.getSelectedItem("KneadingStartEndControl");
    if (selectedKneadingItem == null)
        return;

    $("#KneadingStartEndControl").find("input[type='radio']").prop("checked", false);
    $(".jsgrid-selected").find('input[type="radio"]').prop("checked", true);

}


$(function () {

    $('input[type="radio"]')
        .click(function (event) {

            // Find the name of being checked checkbox.
            var name = $(this).attr("name");

            // Uncheck all.
            $("input[name='" + name + "']")
                .prop("checked", false)
                .removeAttr("checked");

            // Check the current one.
            $(this)
                .prop("checked", true)
                .attr("checked", "checked");

            // Reload grid.
            loadKneadingCommands();
        });


});

/*
    This function is for analyze date time and convert to date instance
*/
analyzeDateTime = function (data) {

    // Build a regular expression for checking datetime returned from server whether it matches with /Date(<numeric>)/ or not.
    // As it does, convert it back to Date instance.
    var sqlDateRegex = /[/]Date[(]([0-9]|[-][0-9])+[)][/]/gi;
    var regexKeepNumber = /[^0-9.-]/gi;

    if (sqlDateRegex.test(data)) {

        // Replace the data.
        data = data.replace(regexKeepNumber, "");

        try {
            return new Date(1 * data);
        } catch (exception) {
            return null;
        }
    }

    return data;
}

$(function () {

    // Initate connection to signalr hub.
    var signalrConnection = initiateHubConnection(hubList.c4);
    listenHubC4(signalrConnection);
    signalrStartHubConnection();
});
