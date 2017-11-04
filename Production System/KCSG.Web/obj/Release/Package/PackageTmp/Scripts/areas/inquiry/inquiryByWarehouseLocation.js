var InquiryByWarehouseLocation = function () {
    return {
        init: function () {
        },
        onSuccess: function (data) {
            //
            //console.log(data);
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId + " .modal-body").html(data.Data);
                $("#" + modelId).modal();
                $.unblockUI();
            } else {
                showNormanMessage(data, null, null);
            }
            //$.unblockUI();
        },
        onChange: function () {
            var searchCondition = $('input[name=SearchCondition]:checked').val();
            var executingClassification = $('input[name=ExecutingClassification]:checked').val();
            location.href = '/Inquiry/InquiryByWarehouseLocation/Index?searchCondition=' + searchCondition + '&executingClassification=' + executingClassification;
        },
        onPrint:function() {
            var searchCondition = $('input[name=SearchCondition]:checked').val();
            var printOption = $('input[name=PrintOption]:checked').val();
            var x = confirm("Ready to print?");
            if (!x) {
                return false;
            }

            if (searchCondition == "0") {
                $.ajax({
                    url: '/Inquiry/InquiryByMaterialName/ExportMaterialName',
                    data: {
                        status: printOption
                    },
                    type: "post",
                    success: function (response) {
                        var render = response.render;
                        if (render != null) {
                            $("#PrintArea")
                                .html(render)
                                .show()
                                .print()
                                .empty()
                                .hide();
                        }
                        $.unblockUI();
                    }
                });
            }

            if (searchCondition == "1") {
                $.ajax({
                    url: '/Inquiry/InquiryByPreProductName/ExportPreProductName',
                    data: {
                        status: printOption
                    },
                    type: "post",
                    success: function (response) {
                        
                        var render = response.render;

                        if (render != null) {

                            $("#PrintArea")
                                .html(render)
                                .show()
                                .print()
                                .empty()
                                .hide();
                        }
                        $.unblockUI();
                    }
                });
            }

            if (searchCondition == "2") {
                $.ajax({
                    url: '/Inquiry/InquiryByProductName/ExportExtProductName',
                    data: {
                        status: printOption
                    },
                    type: "post",
                    success: function (response) {

                        var render = response.render;

                        if (render != null) {

                            $("#PrintArea")
                                .html(render)
                                .show()
                                .print()
                                .empty()
                                .hide();
                        }
                        $.unblockUI();
                    }
                });
            }
        } 
    };
}();
InquiryByWarehouseLocation.init();