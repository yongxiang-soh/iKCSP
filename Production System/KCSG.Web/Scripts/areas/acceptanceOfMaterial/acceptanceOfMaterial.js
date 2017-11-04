$(document).ready(function() {
    $("#btnAccept").prop("disabled", true);
    $("#btnReject").prop("disabled", true);

    $("#txtpNo").focus();
    // Handle click on checkbox to set state of "Select all" control
    $('#AcceptanceOfMaterial').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnAccept').prop("disabled", false) : $('#btnAccept').prop("disabled", true);
        counterChecked > 0 ? $('#btnReject').prop("disabled", false) : $('#btnReject').prop("disabled", true);
    });
});
// This function is for finding the selected item in Acceptance Of Material grid.
findSelectedItem = function () {
    return gridHelper.getSelectedItem("AcceptanceOfMaterial");
}



searchSubMaterial = function () {

    // Retrieve value from pNo and partial delivery and do search.

}

/**
     * Search for the selected item in the grid.
     * As the item is valid, mark it as accepted and save its state to database.
*/
acceptMaterial = function () {

    // Find the selected material in the Grid.
    var selectedMaterial = findSelectedItem();

    // As no item has been selected. Ignore this function.
    if (selectedMaterial == null)
        return;

    // Display confirmation dialog (MSG 019)
    if (!confirm("Are you sure you want to accept this material?"))
        return;

    // Call the Accept function in controller.
    $.ajax({
        url: "/MaterialManagement/AcceptanceOfMaterial/Accept",
        type: "post",
        data: {
            "pNo": selectedMaterial.F30_PrcOrdNo,
            "partialDelivery": selectedMaterial.F30_PrtDvrNo
        },
        success: function (data) {
            // As acceptance is successful, reload the grid.
            reloadMaterialsList();
            $("#btnAccept").prop("disabled", true);
            $("#btnReject").prop("disabled", true);

        },
        error: function (data) {
            
        }
    });
}

/**
 * Find the selected material in the grid, do the rejection and save its state into database.
 * @returns {} 
 */
rejectMaterial = function() {
    // Find the selected material in the Grid.
    var selectedMaterial = findSelectedItem();

    // As no item has been selected. Ignore this function.
    if (selectedMaterial == null)
        return;

    // Display confirmation dialog with MSG 20.
    if (!confirm("Are you sure you want to reject this material?"))
        return;

    // Call the Accept function in controller.
    $.ajax({
        url: "/MaterialManagement/AcceptanceOfMaterial/Reject",
        type: "post",
        data: {
            "pNo": selectedMaterial.F30_PrcOrdNo,
            "partialDelivery": selectedMaterial.F30_PrtDvrNo
        },
        success: function (data) {
            // As acceptance is successful, reload the grid.
            reloadMaterialsList();
            $("#btnAccept").prop("disabled", true);
            $("#btnReject").prop("disabled", true);
        },
        error: function (data) {

        }
    });
}

/**
 * This function is for reloading materials list with searching & pagination conditions.
 * @returns {} 
 */
reloadMaterialsList = function () {

    // Find the pNo from search input textbox.
    var pNo = $("#txtpNo").val();

    // Find the Partial Delivery from search input box.
    var partialDelivery = $("#txtPartialDelivery").val();

    // Reload grid with these 2 filtering conditions.
    var requestParameters = {
        pNo: pNo,
        partialDelivery: partialDelivery
    };

    gridHelper.searchObject("AcceptanceOfMaterial", requestParameters);
    
}

/**
 * This function is called when Search button, pNo textbox, Partial delivery textbox is enter pressed.
 * Base on search conditions, materials are displayed on grid.
 * Form should be prevented from being submitted.
 * @returns {} 
 */
searchMaterials = function(event) {
    
    // Prevent form from being submitted.
    event.preventDefault();

    // Reload the materials list.
    reloadMaterialsList();

    return false;
}

ClearData = function () {
    
    $("#txtpNo").val("");
    $("#txtPartialDelivery").val("");
    //$('#AcceptanceOfMaterial').jqGrid('GridUnload');
    //reloadMaterialsList();
}
