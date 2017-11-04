$(document).ready(function() {
    $('#EndDate').keyup(function () {
        $("#errorEndDate").text("").hide();
        $("#EndDate").removeClass("input-validation-error");
    });

    $("#dvEndDate").on('dp.change', function() {
        $("#errorEndDate").text("").hide();
        $("#EndDate").removeClass("input-validation-error");
    });
    $(".jsgrid-grid-body").scroll(function () {
        var leftpos = $(this).scrollLeft();
        $("#dvTempGrid").find(".jsgrid-grid-header").animate({ scrollLeft: leftpos }, 0);
        return ;
    });
    $("#dvTempGrid").find(".jsgrid-grid-header").scroll(function () {
        var leftpos = $(this).scrollLeft();
        $(".jsgrid-grid-body").animate({ scrollLeft: leftpos }, 0);
        return;
    });
});

function Go() {
    
    var endDate = moment($("#EndDate").val(),"DD/MM/YYYY");
    var currentDate = moment();

    if (endDate > currentDate) {
        $("#errorEndDate").text("The End Date is invalid.").show();
        $("#EndDate").addClass("input-validation-error");
        return;
    }

    var startDate = moment($("#StartDate").val(), "DD/MM/YYYY");
    var duration = moment.duration(endDate.diff(startDate));
    if (duration.asDays() >= 90) {
        $("#errorEndDate").text("The different of dates is more than 90 days!").show();
        $("#EndDate").addClass("input-validation-error");
        return;
    }

    if (endDate < startDate) {
        $("#errorEndDate").text("The Start date is larger than the End date!").show();
        $("#EndDate").addClass("input-validation-error");
        return;
    }
    var data = gridHelper.getListingData("Grid");
    var evnModel = $('input[name=EnvMode]:checked').val();
    for (var i = 0;i < data.length; i++) {
        var F80_Id = data[i].F80_Id,
            F80_Type = data[i].F80_Type,
            F80_Name = data[i].F80_Name;
        var cfr = true;
        $.ajax({
            url: "/EnvironmentManagement/CalculationOfControlLimit/Search",
            type: "post",
            data: {
                id: F80_Id,
                type: F80_Type,
                name: F80_Name,
                envMode: evnModel,
                startDate:$("#StartDate").val(),
                endDate:$("#EndDate").val()
            },
            async: false,
            success: function (response) {
                // you will get response from your php page (what you echo or print)                 
                if (!response.Success) {
                    if (response.ErrorCode === -2) {
                        cfr = confirm(F80_Name.trim() + ":" + response.Message);
                    }
                } 
            },
            error: function (jqXHR, textStatus, errorThrown) {
                //console.log(textStatus, errorThrown);
            }
        });
        $.unblockUI();
        if (!cfr) {
            break;
        }
    }
    gridHelper.ReloadGrid("Grid");
}

function convertDate(dateValue) {
    var stringDateFormat = dateValue.split('/')[1] + '/' + dateValue.split('/')[0] + '/' + dateValue.split('/')[2];
    var result = moment(stringDateFormat).format("DD/MM/YYYY");
    return result;
}