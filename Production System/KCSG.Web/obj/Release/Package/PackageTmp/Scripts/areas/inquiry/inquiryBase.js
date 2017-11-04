
var InquiryBase = function () {
    
    return {
        init: function () {
            //$('body').on('input', '#updateSubscreentcf032f #PrcordNo', function () {
            //    var prcordNo = $('#PrcordNo').val(),
            //        prtdvrNo = $('#PrtdvrNo').val();
            //    $.ajax({
            //        url: '/Inquiry/InquiryBase/GetAcceptanceClassification',
            //        data: {
            //            prcordNo: prcordNo,
            //            prtdvrNo: prtdvrNo
            //        },
            //        type: "post",
            //        success: function (response) {
            //            console.log(response);
            //            $('#AcceptanceClassification').val(response);
            //            $.unblockUI();
            //        }
            //    });
            //});

            //#region updateSubscreentcf034f
            $('body').on('change', '#updateSubscreentcf034f #txtPreProductCode', function () {
                var precode = $('#txtPreProductCode').val(),
                    prelot = $('#PreProductLotNo').val();
                $.ajax({
                    url: '/Inquiry/InquiryBase/Getkndcmd',
                    data: {
                        precode: precode,
                        prelot: prelot
                    },
                    type: "post",
                    success: function (response) {
                        $('#KneadingCommandNo').val(response);
                        $.unblockUI();
                    }
                });
            });

            $('body').on('change', '#updateSubscreentcf034f #PreProductLotNo', function () {
                var precode = $('#txtPreProductCode').val(),
                    prelot = $('#PreProductLotNo').val();
                $.ajax({
                    url: '/Inquiry/InquiryBase/Getkndcmd',
                    data: {
                        precode: precode,
                        prelot: prelot
                    },
                    type: "post",
                    success: function (response) {
                        $('#KneadingCommandNo').val(response);
                        $.unblockUI();
                    }
                });
            });
            //#endregion

        },
        onUpdateLocationMaterialSuccess: function (data) {
            // 
            var modelId = "locationMaterialStatus";
            //$("#" + modelId + " .modal-body").html(data.Data);
            if (data.IsShowNewModel) {
                $("#" + modelId + " .modal-body").html(data.Data);
                $("#" + modelId).modal();
                $.unblockUI();
            } else {
                $("#" + modelId).modal('hide');
                showNormanMessageAndReloadUc789(data, null, null);
            }
            
        },
        onUpdateSubscreentcf032fSuccess: function (data) {
             
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
                showNormanMessage(data, null, null);
            } else {
                $('#lblMessageError').text(data.Message);
                $.blockUI({
                    baseZ: 2000,
                    message: $("#dvError"),
                    timeout: 3000,
                    css: {
                        width: '500px',
                        height: '120px',
                        border: 'none',
                        backgroundColor: 'none',
                        '-webkit-border-radius': '10px',
                        '-moz-border-radius': '10px',
                        opacity: 0.8,
                        color: '#fff'
                    },
                    onUnblock: function () {
                        var location = window.location.pathname;
                        if (location === "/Inquiry/InquiryByShelfStatus") {
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
                                success: function (data1) {
                                    $("#formPlaceholder").html(data1);
                                }
                            });
                        }
                    }

                });
            }
            
        },
        onUpdateSubscreentcf034fSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        },
        onUpdateSubscreentcf038fSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        },
        onUpdateSubscreentcf03AfSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        },
        onUpdateSubscreentcf036fSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        },
        onUpdateSubscreentcf037fSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        },
        onUpdateSubscreentcf03BfSuccess: function (data) {
            if (data.Success) {
                var modelId = "locationMaterialStatus";
                $("#" + modelId).modal('hide');
            }
            showNormanMessageAndReloadUc789(data, null, null);
            //showNormanMessage(data, null, null);
        }
    };
}();
InquiryBase.init();