


var EnvironmentDataEdit = function() {
    return {
        init: function() {
            var environmentDate = $('#EnvironmentDate').val();
            var currentDate = moment().format("DD/MM/YYYY");

            if (environmentDate === currentDate) {
                setInterval(function() { EnvironmentDataEdit.onExecute() }, 300000); // For 5 minus
            } else {
                EnvironmentDataEdit.onExecute();
            }
            EnvironmentDataEdit.onExecute();
            // $('#go').click();
        },
        onExecute: function() {
            var dataPost = {
                envMode: $("input[name='EnvMode']:checked").val(),
                environmentDate: $('#EnvironmentDate').val(),
                location: $('#Location :selected').val()
            };
            $.ajax({
                url: '/EnvironmentManagement/EnvironmentDataEdit/Search',
                type: "post",
                data: dataPost,
                success: EnvironmentDataEdit.onSuccess
            });
        },
        onGo: function() {
            // 
            var environmentDate = $('#EnvironmentDate').val(),
                envMode = $("input[name='EnvMode']:checked").val(),
                locationname = $('#Location :selected').val();
            window.location.href = "/EnvironmentManagement/EnvironmentDataEdit/Index?environmentDate=" + environmentDate + "&envMode=" + envMode + "&locationName=" + locationname;
        },
        onSuccess: function(data) {
            //console.log(data);
            if (!data.Success) {
                $('#hHigh').val("");
                $('#hLow').val("");
                $('#hRanger').val("");
                $('#hMean').val("");
                $('#hSignma').val("");

                $('#temHigh').val("");
                $('#temLow').val("");
                $('#temRanger').val("");
                $('#temMean').val("");
                $('#temSignma').val("");

                if (data.ErrorCode === -2) {
                    showNormanMessage(data);
                } else {
                    $.unblockUI();
                }
            }
            if (data.Success) {

                $("#temHigh").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temHigh").autoNumeric("set", data.humid_hi);
                $("#temLow").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temLow").autoNumeric("set", data.temp_lo);
                $("#temRanger").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temRanger").autoNumeric("set", data.temp_range);
                $("#temMean").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temMean").autoNumeric("set", data.temp_mean);
                $("#temSignma").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temSignma").autoNumeric("set", data.temp_sigma);
                $("#temLow").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#temLow").autoNumeric("set", data.temp_lo);

                var chartTemper = document.getElementById("chartTemper");
                var temperature = new Chart(chartTemper,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'top',
                            text: 'Temperature (C)'
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
                                        labelString: "Temperature"
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
                        labels: JSON.parse(data.tem_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.tem_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor: "RGB(0, 0, 255)",
                                fill: false
                            },
                            {
                                label: "DEC MEAN",
                                data: JSON.parse(data.tem_fld_mean),
                                backgroundColor: "RGB(0, 255, 0)",
                                lineTension: 0,
                                borderColor: "RGB(0, 255, 0)",
                                fill: false
                            },
                            {
                                label: "DEC UPPER",
                                data: JSON.parse(data.tem_fld_upper),
                                backgroundColor: "RGB(255, 0, 0)",
                                lineTension: 0,
                                borderColor: "RGB(255, 0, 0)",
                                fill: false
                            },
                            {
                                label: "DEC LOWER",
                                data: JSON.parse(data.tem_fld_lower),
                                backgroundColor: "RGB(128, 128,0)",
                                lineTension: 0,
                                borderColor: "RGB(128, 128, 0)",
                                fill: false
                            }
                        ]
                    }
                });
                $("#hHigh").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#hHigh").autoNumeric("set", data.humid_hi);
                $("#hLow").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#hLow").autoNumeric("set", data.humid_lo);
                $("#hRanger").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#hRanger").autoNumeric("set", data.humid_range);
                $("#hMean").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#hMean").autoNumeric("set", data.humid_mean);
                $("#hSignma").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999', mDec: 2 });;
                $("#hSignma").autoNumeric("set", data.humid_sigma);
                //$("#hHigh").autoNumeric("set", data.humid_hi);
                //$("#hLow").autoNumeric("set", data.humid_lo);
                //$("#hRanger").autoNumeric("set", data.humid_range);
                //$("#hMean").autoNumeric("set", data.humid_mean);
                //$("#hSignma").autoNumeric("set", data.humid_sigma);
                var humidity = document.getElementById("humidity");
                var humidityChart = new Chart(humidity,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'top',
                            text: 'Humidity (%RH)'
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
                                        labelString: "Humidity"
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
                        labels: JSON.parse(data.h_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.h_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor: "RGB(0, 0, 255)",
                                fill: false
                            },
                            {
                                label: "DEC MEAN",
                                data: JSON.parse(data.h_fld_mean),
                                backgroundColor: "RGB(0, 255, 0)",
                                lineTension: 0,
                                borderColor: "RGB(0, 255, 0)",
                                fill: false
                            },
                            {
                                label: "DEC UPPER",
                                data: JSON.parse(data.h_fld_upper),
                                backgroundColor: "RGB(255, 0, 0)",
                                lineTension: 0,
                                borderColor: "RGB(255,0, 0)",
                                fill: false
                            },
                            {
                                label: "DEC LOWER",
                                data: JSON.parse(data.h_fld_lower),
                                backgroundColor: "RGB(128, 128, 0)",
                                lineTension: 0,
                                borderColor: "RGB(128, 128, 0)",
                                fill: false
                            }
                        ]
                    }
                });
                $.unblockUI();
            };

        },
    };
}();
EnvironmentDataEdit.init();

$("#dvTime").on('dp.change', function() { LoadData(); });

function Update() {
    if ($("#frmUpdate").valid()) {
        var time = $("#Time").val();
        var temperature = $("#Temperature").val();
        var humidity = $("#Humidity").val();
        var locationId = $("#Location option:selected").val();
        var environmentDate = $("#EnvironmentDate").val();
        $.ajax({
            url: "/EnvironmentManagement/EnvironmentDataEdit/Edit",
            type: "post",
            data: {
                locationId: locationId,
                environmentDate: environmentDate,
                time: time,
                temperature: temperature,
                humidity: humidity
            },
            success: function (data) {
                $.unblockUI();
                if (data.Success) {
                    showMessage(data);
                    setTimeout(function() {
                        EnvironmentDataEdit.init();
                        $("#Time").val("");
                        $("#Temperature").val("");
                        $("#Humidity").val("");
                    }, 3000);
                }
            }
        });
    }
}


function LoadData() {
    var time = $("#Time").val();
    var environmentDate = $("#EnvironmentDate").val();
    var locationValue = $("#Location option:selected").val();
    $.ajax({
        url: "/EnvironmentManagement/EnvironmentDataEdit/GetEnvTempData",
        type: "post",
        data: {
            time: time,
            environmentDate: environmentDate,
            locationValue: locationValue
        },
        success: function (data) {
            $.unblockUI();
            if (data.Item2 != null) {
                $("#timeError").text("").hide();
                $("#Temperature").val(data.Item2.F81_Temp);
                if (data.Item1 == 1) {
                    $("#Humidity").val(data.Item2.F81_Humidity);
                } else {
                    $("#Humidity").val("");
                }
            } else {
                $("#Temperature").val("0");
                $("#Humidity").val("0");
                $("#timeError").text("Record not found!").show();
            }
        }
    });
}