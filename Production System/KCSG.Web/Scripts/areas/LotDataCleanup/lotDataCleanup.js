function Test() {
    if ($("#NewCutOfDate").valid() & $("#NewCutOfTime").valid()) {
        var stringNewCutOffDate = $("#NewCutOfDate").val();
        var stringNewCutOffTime = $("#NewCutOfTime").val();
        $.ajax({
            url: "/EnvironmentManagement/LotDataCleanup/Testing",
            type: "post",
            data: {
                stringNewCutOffDate: stringNewCutOffDate,
                stringNewCutOffTime: stringNewCutOffTime
            },
            success: function (data) {
                $("#Lot2").val(data);
            }
        });
    }
}

function Go() {
    if ($("#NewCutOfDate").valid() & $("#NewCutOfTime").valid()) {
        var stringNewCutOffDate = $("#NewCutOfDate").val();
        var stringNewCutOffTime = $("#NewCutOfTime").val();
        $.ajax({
            url: "/EnvironmentManagement/LotDataCleanup/Delete",
            type: "post",
            data: {
                stringNewCutOffDate: stringNewCutOffDate,
                stringNewCutOffTime: stringNewCutOffTime
            },
            success: function(data) {
                //$.unblockUI();
                showMessage(data);
            }
        });
    }
}