var PreProductCharging = function () {
    return {
        init: function () {
            debugger;
            if ($("#IsError").val() == "True") {
                var respon = { Success: false, Message: $("#ErrorCode").val() }
                showMessage(respon);
            }

            $('#ContainerCode2')
                .change(function (event) {
                    event.preventDefault();
                    
                    var containerCodeScanned = $('#ContainerCode2').val();
                    if (containerCodeScanned != null && containerCodeScanned.length > 12) {
                        containerCodeScanned = containerCodeScanned.substr(0, 12);
                        $("#ContainerCode2").val(containerCodeScanned);
                    } 
                });

            $('#ContainerCode1')
                .change(function (event) {
                    $('#KneadingCmdNo').val("");
                    $('#PreProductLotNo').val("");
                    $('#ProductLotNo').val("");
                    $('#PreProductCode').val("");
                    $('#ProductCode').val("");
                    $('#ProductName').val("");
                    $('#PreProductName').val("");
                    event.preventDefault();

                    var containerCode = $('#ContainerCode1').val();
                    if (containerCode != null && containerCode.length > 12) {
                        containerCode = containerCode.substr(0, 12);
                        $("#ContainerCode1").val(containerCode);
                    }

                    $.ajax({
                        url: "/TabletisingCommandSubSystem/PreProductCharging/GetPreProductChargingItemByContainerCode",
                        type: "POST",
                        data: { containerCode: containerCode },
                        success: function (data) {
                            if (data.Success) {
                                $('#KneadingCmdNo').val(data.Data.KneadingCmdNo);
                                $('#PreProductLotNo').val(data.Data.PreProductLotNo);
                                $('#ProductLotNo').val(data.Data.ProductLotNo);
                                $('#PreProductCode').val(data.Data.PreProductCode);
                                $('#ProductCode').val(data.Data.ProductCode);
                                $('#ProductName').val(data.Data.ProductName);
                                $('#PreProductName').val(data.Data.PreProductName);
                                //$('#ContainerCode1').val(data.Data.ContainerCode1);

                                $('#ContainerCode2').prop('disabled', false);
                                $('#btnAdd').prop('disabled', false);
                                $.unblockUI();
                            } else {
                                showNormanMessage(data, null, null);
                                $('input').prop('disabled', true);
                                $('#btnAdd').prop('disabled', true);
                            }
                        }

                    });
                });
        },
        onSuccess: function (data) {
            showNormanMessage(data, null, null);
        },
    };
}();
PreProductCharging.init();

function CheckLock() {
    var containerCode1 = $("#ContainerCode1").val().trim();
    var containerCode2 = $("#ContainerCode2").val().trim();

    if (containerCode1 == containerCode2) {
        $("#unlocked").attr('checked', 'checked');
    } else {
        $("#locked").attr("checked", 'checked');
    }

}