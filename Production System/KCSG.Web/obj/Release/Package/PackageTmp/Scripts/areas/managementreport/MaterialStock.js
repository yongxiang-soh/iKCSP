$(document).ready(function () {
    
});

exportInquiryByMaterialName = function (event) {
    var status = $('input[name=PrintOptions]:checked').val();
    if (!confirm("Ready to print?")) {
        return;
    };
    // Prevent default behaviour of button.
    event.preventDefault();    
    $.ajax({
        url: formUrl.urlPrintMaterialName,
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


function OnChangeTextBox() {    
    var model = {
        materialCode: $('#txtMaterialCode').val()
    }    
    $.ajax({
        url: formUrl.urlCheckPreProduct,
        //url: "/ProductCertification/Edit",
        type: 'GET',
        data: {
            materialCode: model.materialCode
        },
        success: function (data) {
            $("#txtMaterialName").val(data.result.F01_MaterialDsp);
            if (data.result.F01_EntrustedClass === '0') {
                $("#txtBailmentClass").val('Norm');
            } else {
                $("#txtBailmentClass").val('Bail');
            }
            //$("#txtBailmentClass").val(data.result.F01_EntrustedClass);
        }
    });
}

