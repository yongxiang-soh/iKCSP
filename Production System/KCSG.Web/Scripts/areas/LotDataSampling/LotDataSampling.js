
$().ready(function() {
    $("#btnDelete").prop("disabled", true);

    $('#Time').rules("add", "CheckTime");
    
    $('#NewLotNumber').change(function () {
        ChangeProduct();
    });
});


//function ChangeDate() {
//    setTimeout(function() {
//        $("#Temperature").val("");
//        $("#lsterrorTime").text("").hide();
//        $("#Time").removeClass("input-validation-error");
//        $.ajax({
//            url: '/EnvironmentManagement/LotDataSampling/ValidateTime',
//            data: {
//                time: $("#Time").val(),
//                product: $("#Product").val(),
//                date: $("#Date").val()
//            },
//            type: "Options",
//            async: false,
//            success: function(response) {
//                if (response.IsSuccess) {
//                    $("#Temperature").val(gridHelper.formatNumber(response.value));
//                } else {
//                    $("#lsterrorTime").text("Record for specified time not found!").show();
//                    $("#Time").addClass("input-validation-error");
//                }
//            }
//        });
//    }, 500);

//}


$.validator.addMethod("CheckTime",
    function (value, element, params) {
        $("#Temperature").val("");
        var isSuccess = false;

        $.ajax({
            url: '/EnvironmentManagement/LotDataSampling/ValidateTime',
            data: {
                time: value,
                product: $("#Product").val(),
                date: $("#Date").val()
            },
            type: "Options",
            async: false,
            success: function(response) {
                if (response.IsSuccess) {
                    $("#Temperature").val(gridHelper.formatNumber(response.value));
                    isSuccess = true;
                } else {
                    isSuccess = false;
                }
            }
        });
        return isSuccess;

    },
    "Record for specified time not found!");



function drawChart(data) {
    var chartTemper = $("#chartTemper");
    if (data.length == 0) {
        var humidityChart = new Chart(chartTemper,
         {
             type: 'line',
             options: {
                 title: {
                     display: true,
                     position: 'top',
                     fontSize: 20,
                     text: 'Temperature (℃)'
                 },
                 scales: {
                     yAxes: [
                         {
                             ticks: {
                                 beginAtZero: true
                             },
                             scaleLabel: {
                                 display: true,
                                 fontSize: 20,
                                 labelString: 'Temperature (℃)'
                             }
                         }
                     ],
                     xAxes: [
                         {
                             scaleLabel: {
                                 display: true,
                                 fontSize: 20,
                                 labelString: 'Time'
                             }
                         }
                     ]
                 }

             },
             data: {
                 labels: [],
                 datasets: []
             }
         });
    } else {
         var humidityChart = new Chart(chartTemper,
         {
             type: 'line',
             options: {
                 title: {
                     display: true,
                     position: 'top',
                     fontSize: 20,
                     text: 'Temperature (℃)'
                 },
                 scales: {
                     yAxes: [
                         {
                             ticks: {
                                 beginAtZero: true
                             },
                             scaleLabel: {
                                 display: true,
                                 fontSize: 20,
                                 labelString: 'Temperature (℃)'
                             }
                         }
                     ],
                     xAxes: [
                         {
                             scaleLabel: {
                                 display: true,
                                 fontSize: 20,
                                 labelString: 'Time'
                             }
                         }
                     ]
                 }

             },
             data: {
                 labels: data.dt_dtm,
                 datasets: [
                     {
                         label: "DEC DATA",
                         data: data.Dec_data,
                         backgroundColor: "RGB(0, 0, 255)",
                         lineTension: 0,
                         borderColor: "RGB(0, 0, 255)",
                         fill: false
                     },
                     {
                         label: "DEC MEAN",
                         data: data.Dec_mean,
                         backgroundColor: "RGB(0, 255, 0)",
                         lineTension: 0,
                         borderColor: "RGB(0, 255, 0)",
                         fill: false
                     },
                     {
                         label: "DEC UPPER",
                         data: data.Dec_upper,
                         backgroundColor: "RGB(255, 0, 0)",
                         lineTension: 0,
                         borderColor: "RGB(255,0, 0)",
                         fill: false
                     },
                     {
                         label: "DEC LOWER",
                         data: data.Dec_lower,
                         backgroundColor: "RGB(128, 128, 0)",
                         lineTension: 0,
                         borderColor: "RGB(128, 128, 0)",
                         fill: false
                     }
                 ]
             }
         });
    }
    
}

function Search() {
    gridHelper.searchObject("Grid", {
        productCode: $("#Product").val()
    });
}

function onSuccess(data) {
    if (data.IsSuccess === true) {
        drawChart(data.Data);
        $("#HighTemp").autoNumeric("set",data.Data.HighTemp);
        $("#LowTemp").autoNumeric("set",data.Data.LowTemp);
        $("#RangeTemp").autoNumeric("set",data.Data.RangeTemp);
        $("#SigmaTemp").autoNumeric("set",data.Data.SigmaTemp);
        $("#MeanTemp").autoNumeric("set",data.Data.MeanTemp);
        Search();

    } else {
        drawChart([]);
        $("#HighTemp").val("");
        $("#LowTemp").val("");
        $("#RangeTemp").val("");
        $("#SigmaTemp").val("");
        $("#MeanTemp").val("");
        alert("Location is not found for the selected product!");
    }

    $("#hdfProduct").val($("#Product").val());
    $("#hdfLotNo").val($("#LotNo").val());
    $("#hdfDate").val($("#Date").val());
    $('#btnAdd').prop("disabled", false);
}


function GetTe84_Env_Lot() {

    $.ajax({
        url: '/EnvironmentManagement/LotDataSampling/GetTe84EnvLot',
        data: {
            productCode: $("#Product option:selected").val(),
            lotNo: $("#LotNo option:selected").val(),
            date: $("#Date").val()
},
        type: "Options",
        async: false,
        success: function (response) {
            for (var i = 1; i < 4; i++) {
                if (response[i-1]){
                    $("#Time" + i).val(gridHelper.displayDateTimeOnly(response[i - 1].F84_S_Time));
                    $("#Temperature" + i).autoNumeric("set", response[i - 1].F84_Temp);
                }
            }
        }
    });
}
function LoadGridSuccess() {
    $("#Time1").val("00:00");
    $("#Temperature1").val("0.00");
    $("#Time2").val("00:00");
    $("#Temperature2").val("0.00");
    $("#Time3").val("00:00");
    $("#Temperature3").val("0.00");
    //var lstData = gridHelper.getListingData("Grid");
    //for (var i = 1; i < 4; i++) {
    //    if (lstData[i - 1] != null) {
    //        $("#Time" + i).val(gridHelper.displayDateTime(lstData[i - 1].F84_S_Time));
    //        $("#Temperature" + i).autoNumeric("set", lstData[i - 1].F84_Temp);
    //    }
    //}
    GetTe84_Env_Lot();
    $("#Grid").find("tr").click(function() {
        $("#btnDelete").prop("disabled", false);
    });
}

function ChangeProduct() {
    $("#LotNo").find('option').remove();
    $.ajax({
        url: '/EnvironmentManagement/LotDataSampling/GetLotNo',
        data: {
            productCode: $("#Product").val(),
            newCheck: $("#NewLotNumber").is(':checked')
        },
        type: "Options",
        async: false,
        success: function(response) {
            $("#LotNo").append(
                $('<option></option>').attr("value", "").html("-- Please select --")
            );
            $.each(response, function(value, text) {
                $("#LotNo").append(
                    $('<option></option>').attr("value", text.Text).html(text.Text)
                );
            });
        }
    });
}

function ChangeLotNo() {
    $.ajax({
        url: '/EnvironmentManagement/LotDataSampling/GetDate',
        type: "post",
        data: {
            lotNo: $("#LotNo").val()
        },
        async: false,
        success: function (response) {
            $.unblockUI();

            $("#Date").val(response);

        }
    });
}


function Delete() {
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: '/EnvironmentManagement/LotDataSampling/Delete',
            data: {
                id: gridHelper.getSelectedItem("Grid").F84_Id,
            },
            type: "Post",
            async: false,
            success: function(response) {
                showMessage(response, "Grid");
            }
        });
    }
}

function Add() {
    //if (gridHelper.getListingData("Grid").length >= 10) {
    //    showNormanMessage({
    //        Success: false,
    //        message: "Number of lot sampling has exceeded 10 records!"
    //    });
    //} else {
        $("#fromAdd").submit();
    //}

}

function onAddSuccess(data) {
    showNormanMessage(data);
    setTimeout(function() {
        Search();
    },3000)
}