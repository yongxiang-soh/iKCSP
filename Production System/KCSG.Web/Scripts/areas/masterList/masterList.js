$().ready(function (parameters) {
    $("#txtMasterCodeSearch").focus();
    $("#searchForm").submit(function (e) {
        e.preventDefault();
        search();
        return false;
    });
});

/**
 * Fired on search button is clicked.
 * Depend on the condition user selects, display the search result to client screen.
 * @returns {} 
 */
search = function () {
    $("#dvRes").show();

    var request = {
        MasterCode: $("#txtMasterCodeSearch").val().trim()
    };
    var value = $("#rdoMaterial:checked").val();
    
    if (value ==="0") {
        gridHelper.searchObject("MaterialGrid", request);
        $("#dvMaterial").show();
        $("#dvResName").text("Material");
    } else {
        $("#dvMaterial").hide();
    }
    if (value === "1") {
        gridHelper.searchObject("PreProductGrid", request);
        $("#dvPreProduct").show();
        $("#dvResName").text("Pre-Product ");
    } else {
        $("#dvPreProduct").hide();
    }
    if (value === "2") {
        gridHelper.searchObject("ProductGrid", request);
        $("#dvProduct").show();
        $("#dvResName").text("Product");
    } else {
        $("#dvProduct").hide();
    }
}

/**
 * This function is for printing search result on the table.
 * @returns {} 
 */
exportReport = function (event) {
    if (confirm("Are you sure to print all the item(s)?")) {
        $.ajax({
            url: "/ProductionPlanning/MasterList/Print",
            type: "post",
            data: $("#searchForm").serialize(),
            success: function (response) {

                // Template is detected.
                var render = response.render;
                if (render != null) {

                    $("#PrintArea")
                        .html(render)
                        .show()
                        .print()
                        .empty()
                        .hide();
                }
            }
        });
    }
   

}