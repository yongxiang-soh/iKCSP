
function DrawChart(chartName, data) {
    var charter = $("#" + chartName);
    if (data != null) {
       var chart = new Chart(charter,
         {
             type: 'line',
             options: {
                 title: {
                     display: true,
                     position: 'top',
                     fontSize: 20,
                     text: chartName
                 },
                 scales: {
                     yAxes: [{
                       scaleLabel: {
                             display: true,
                             fontSize: 20,
                             labelString:chartName === "Average"?'Temperature (C)':"Range" 
                         }
                     }],
                     xAxes: [{
                         scaleLabel: {
                             display: true,
                             fontSize: 20,
                             labelString: 'Time'
                         }
                     }]
                 }
             },
             data: {
                 labels: data.LstTime,
                 datasets: [
                     {
                         label: "Dec Data",
                         data: data.LstData,
                         backgroundColor: "RGB(0, 0, 255)",
                         lineTension: 0,
                         borderColor: "RGB(0, 0, 255)",
                         fill: false
                     }
                 ]
             }
         });
        $("#" + chartName + "High").val(data.High);
        $("#" + chartName + "Low").val(data.Low);
        $("#" + chartName + "Range").val(data.Range);
        $("#" + chartName + "Cp").val(data.Cp);
        $("#" + chartName + "Cpk").val(data.Cpk);
        $("#" + chartName + "LCL").val(data.LCL);
        $("#" + chartName + "Mean").val(data.Mean);
        $("#" + chartName + "Sigma").val(data.Sigma);
        $("#" + chartName + "UCL").val(data.UCL);
    }
    //else {
       
    //    var temperature = new Chart(charter,
    //    {
    //        type: 'line',
    //        data: {
    //            labels: ['00:00'],
    //            datasets: [
    //                {
    //                    data: [0],
    //                    backgroundColor: "#e4d1d1"
    //                }
    //            ]
    //        },
    //        options: {
    //            title: {
    //                display: true,
    //                position: 'top',
    //                fontSize: 20,
    //                text: chartName
    //            },
    //            scales: {
    //                yAxes: [{
    //                    ticks: {
    //                        beginAtZero: true
    //                    },
    //                    scaleLabel: {
    //                        display: true,
    //                        fontSize: 20,
    //                        labelString:chartName === "Average"?'Temperature (C)':"Range" 
    //                    }
    //                }],
    //                xAxes: [{
    //                    scaleLabel: {
    //                        display: true,
    //                        fontSize: 20,
    //                        labelString: 'Time'
    //                    }
    //                }]
    //            }
    //        }
    //    })
    //    ;
    //}
   
  
}

$().ready(function() {
    DrawChart("Average");
    DrawChart("Humidity");
    $("#StartDate").rules("add", "CompareStartDateEndDate");
});                  
$.validator.addMethod("CompareStartDateEndDate", function (value, element, params) {
    if ($("#EndDate").val() !== "") {
        
        var startDate =  moment(value, "DD/MM/YYYY").toDate();
        var endDate = moment($("#EndDate").val(), "DD/MM/YYYY").toDate();
        if (startDate > endDate) {
            return false;
        } else {
            return true;
        }

    } else {
        return true;
    }
    

}, "The Start date is larger than the End date!");
function SearchEnv() {
    
}

function onSuccessfull(data) {
    if (data != null) {
        DrawChart("Average", data.TempModel);
        DrawChart("Humidity", data.HimIdModel);
    }
}