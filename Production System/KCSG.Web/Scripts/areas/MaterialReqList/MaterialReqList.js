$().ready(function() {
    $("#DateSearch").focus();
});
function search() {
   if ($("#searchForm").valid()) {
        $("#DateSearch").focus();
       var request = $("#searchForm").serialize();
        gridHelper.searchObject("MaterialGrid", request);
        $("#dvMaterial").show();
        $("#dvDefault").hide();
       
        $("#btnPrint").prop("disabled", false);
   }
}
Print = function (event) {
    if (confirm("Are you sure to print all the item(s)?")) {
        // Product/material/pre-product code.
        $.ajax({
            url: "/ProductionPlanning/MaterialRequirementList/Print",
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