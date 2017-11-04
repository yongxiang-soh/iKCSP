$(document).ready(function () {
    
});

exportPreproductStock = function (event) {
    var status = $('input[name=PrintOptions]:checked').val();
    if (!confirm("Ready to print?")) {
        return;
    };
    // Prevent default behaviour of button.
    event.preventDefault();    
    $.ajax({
        url: formUrl.urlPrintPreProductStock,
        data: {
            status: status
        },
        type: "post",
        success: function (response) {

            var render = response.render;

            if (render != null) {

                $("#printMaterialNameArea")
                    .html(render)
                    .show()
                    .print()
                    .empty()
                    .hide();
            }
        }
    });
}



