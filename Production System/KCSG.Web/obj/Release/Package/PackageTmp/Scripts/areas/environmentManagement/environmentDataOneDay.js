
var EnvironmentDataOneDay = function () {
    
    return {
        init: function () {
            var environmentDate = $('#EnvironmentDate').val();
            var currentDate = moment().format("DD/MM/YYYY");
            if (environmentDate === currentDate) {
                setInterval(function () { EnvironmentDataOneDay.onExecute() }, 300000); // For 5 minus
            } else {
                EnvironmentDataOneDay.onExecute();
            }
            EnvironmentDataOneDay.onExecute();
            //$('#go').click();
        },
        onExecute: function () {
            var dataPost = {
                EnvMode : $("input[name='EnvMode']:checked").val(),
                EnvironmentDate: $('#EnvironmentDate').val(),
                Location: $('#Location :selected').val()
            };
            $.ajax({
                url: '/EnvironmentManagement/EnvironmentDataOneDay/Search',
                type: "post",
                data: dataPost,
                success: EnvironmentDataOneDay.onSuccess
            });
        }, 
        onGo: function () {
            // 
            var environmentDate = $('#EnvironmentDate').val(),
                envMode = $("input[name='EnvMode']:checked").val(),
                location = $('#Location :selected').val();
            window.location.href = "/EnvironmentManagement/EnvironmentDataOneDay/Index?environmentDate=" + environmentDate + "&envMode=" + envMode + "&location=" + location;
        },
        onSuccess: function (data) {
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
                var chartContent = document.getElementById('chartTemperContent');
                chartContent.innerHTML = '&nbsp;';
                $('#chartTemperContent').append('<canvas id="chartTemper" width="400" height="150"></canvas>');

                var chartHumContent = document.getElementById('chartHumContent');
                chartHumContent.innerHTML = '&nbsp;';
                $('#chartHumContent').append('<canvas id="humidity" width="400" height="150"></canvas>');

            }
            if (data.Success) {
                $('#temHigh').val(data.temp_hi);
                $('#temLow').val(data.temp_lo);
                $('#temRanger').val(data.temp_range);
                $('#temMean').val(data.temp_mean);
                $('#temSignma').val(data.temp_sigma);
                var chartTemper = document.getElementById("chartTemper");
                var temperature = new Chart(chartTemper,
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
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                },
                                scaleLabel: {
                                    display: true,
                                    fontSize: 20,
                                    labelString: "Temperature"
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
                        labels: JSON.parse(data.tem_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.tem_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor:"RGB(0, 0, 255)",
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

                $('#hHigh').val(data.humid_hi);
                $('#hLow').val(data.humid_lo);
                $('#hRanger').val(data.humid_range);
                $('#hMean').val(data.humid_mean);
                $('#hSignma').val(data.humid_sigma);
                var humidity = document.getElementById("humidity");
                var humidityChart = new Chart(humidity,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'top',
                            fontSize: 20,
                            text: 'Humidity (%RH)'
                        },
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                },
                                scaleLabel: {
                                    display: true,
                                    fontSize: 20,
                                    labelString: "Humidity"
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
EnvironmentDataOneDay.init();