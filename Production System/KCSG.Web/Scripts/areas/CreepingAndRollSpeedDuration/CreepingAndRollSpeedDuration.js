$().ready(function() {
    DrawChart("Left", null, "Creeping", "Left Creeping");
    DrawChart("Right", null, "Creeping","Right Creeping");
    DrawChart("Roll ", null, "Roll speed", "Roll speed");
    $("#EndDate").rules("add", "CheckEndDate");
    $("#StartDate").rules("add", "CompareStartDateEndDate",null ,"#EndDate");
    $("#StartDate").rules("add", "CompareStartDateEndDate2",null ,"#EndDate");
});
$.validator.addMethod("CheckEndDate", function (value, element, params) {
    
    var d1 = moment(value,"DD/MM/YYYY");
    var d2 = moment();
    return d1 < d2;
}, "The End Date is invalid");
$.validator.addMethod("CompareStartDateEndDate", function (value, element, params) {
    // 
    if ($("#EndDate").val()!=="") {
        if (moment(value, "DD/MM/YYYY") > moment($("#EndDate").val(), "DD/MM/YYYY")) {
            return false;
        } 
    } 
    return true;
}, "The Start date is larger than the End date!");
$.validator.addMethod("CompareStartDateEndDate2", function (value, element, params) {
    
    if ($("#EndDate").val() !== "") {
        //var startdate = moment(moment(value, "DD/MM/YYYY").getTime() + (90 * 24 * 60 * 60 * 1000));
        var d1 = moment(value, "DD/MM/YYYY");
        var d2 = moment($("#EndDate").val(), "DD/MM/YYYY");
        if (d2.diff(d1, 'days') >= 90) {
            return false;
        }
    }
    return true;

}, "The different of dates is more than 90 days!");


function toDate(dateStr) {
    var parts = dateStr.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0]);
}
function DrawChart(chartId, data, chartName, title) {
    //
    var charter = $("#" + chartId);
    if (data != null) {
       var chart = new Chart(charter,
         {
             type: 'line',
             options: {
                 title: {
                     display: true,
                     position: 'bottom',
                     text: data.ChartName
                 },

             },
             data: {
                 labels: data.lstTime,
                 datasets: [
                           {
                               label: "DEC DATA",
                               data: data.Data1,
                               backgroundColor: "RGB(0, 0, 255)",
                               lineTension: 0,
                               borderColor: "RGB(0, 0, 255)",
                               fill: false
                           },
                           {
                               label: "DEC MEAN",
                               data: data.Data2,
                               backgroundColor: "RGB(0, 255, 0)",
                               lineTension: 0,
                               borderColor: "RGB(0, 255, 0)",
                               fill: false
                           },
                           {
                               label: "DEC UPPER",
                               data: data.Data3,
                               backgroundColor: "RGB(255, 0, 0)",
                               lineTension: 0,
                               borderColor: "RGB(255, 0, 0)",
                               fill: false
                           },
                           {
                               label: "DEC LOWER",
                               data: data.Data4,
                               backgroundColor: "RGB(128, 128,0)",
                               lineTension: 0,
                               borderColor: "RGB(128, 128, 0)",
                               fill: false
                           }
                 ]
             }
         });
         
       $("#" + chartId + "High").val(data.High);
       $("#" + chartId + "Low").val(data.Low);
       $("#" + chartId + "Range").val(data.Range);
       $("#" + chartId + "Cp").val(data.Cp);
       $("#" + chartId + "Cpk").val(data.Cpk);
       $("#" + chartId + "LCL").val(data.LCL);
       $("#" + chartId + "Mean").val(data.Mean);
       $("#" + chartId + "Sigma").val(data.Sigma);
       $("#" + chartId + "UCL").val(data.UCL);
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
    //                text: title
    //            },
    //            scales: {
    //                yAxes: [{
    //                    ticks: {
    //                        beginAtZero: true
    //                    },
    //                    scaleLabel: {
    //                        display: true,
    //                        fontSize: 20,
    //                        labelString: chartName
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

function onSuccess(data) {
    //console.log(data);
    if (data != null) {
        DrawChart("Left", data.LeftModel, "Creeping", "Left Creeping");
        DrawChart("Right", data.RightModel, "Creeping", "Right Creeping");
        DrawChart("Roll", data.RollModel, "Roll speed", "Roll speed");
    }
   
}