var InquiryByShelfStatus = function () {
    return {
        init: function () {
            var type = $('#Type').val();
            $.ajax({
                url: "/Inquiry/InquiryByShelfStatus/ShowButonCommand",
                type: "POST",
                data: {
                    searchCondition: 0,
                    executingClassification: 0,
                    type: type
                },
                success: function (data) {
                    $("#formPlaceholder").html(data);
                }
            });
        },
        onChange: function () {
            var searchCondition = $('input[name=SearchCondition]:checked').val();
            var executingClassification = $('input[name=ExecutingClassification]:checked').val();
            var type = $('#Type').val();
            $.ajax({
                url: "/Inquiry/InquiryByShelfStatus/ShowButonCommand",
                type: "POST",
                data: {
                    searchCondition: searchCondition,
                    executingClassification: executingClassification,
                    type: type
                },
                success: function (data) {
                    $("#formPlaceholder").html(data);
                }
            });
        },
        onFunction:function(e) {
            var row = e.getAttribute('data-row'),
            bay = e.getAttribute('data-bay'),
            level = e.getAttribute('data-level'),
            executingClassification = $('input[name=ExecutingClassification]:checked').val(),
            type = $('#Type').val();
            $.ajax({
                url: "/Inquiry/InquiryByShelfStatus/PickShelf",
                type: "POST",
                data: {
                    executingClassification: executingClassification,
                    type: type,
                    row: leftPad(row,2),
                    bay: leftPad(bay,2),
                    level: leftPad(level,2)
                },
                success: function (data) {
                    if (data.Success) {
                        $.unblockUI();
                        var modelId = "locationMaterialStatus";
                        $("#" + modelId + " .modal-body").html(data.Data);
                        $("#" + modelId).modal();
                    } else {
                        showNormanMessage(data, null, null);
                    }
                    
                }
            });
        }

    };
}();
InquiryByShelfStatus.init();