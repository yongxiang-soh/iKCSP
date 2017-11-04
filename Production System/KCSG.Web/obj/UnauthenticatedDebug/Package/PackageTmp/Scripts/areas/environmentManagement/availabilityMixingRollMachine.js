
var AvailabilityMixingRollMachine = function () {
    var chartAvailability = document.getElementById("chartAvailability");
    return {
        init: function () {
            $('#availabilityMixingRollMachine').submit();
        },
        onSuccess: function (data) {
            //console.log(data);
            //var result = JSON.parse(data.grapdata);
            $.unblockUI();
            if (data.Success) {
                $('#avaitext1').val(data.avai1tex);
                $('#avaitext2').val(data.avai2tex);
                $('#avaitotaltext').val(data.avaitotaltext);
                var result = JSON.parse(data.grapdata);
                AvailabilityMixingRollMachine.onDrawChart(result);
            } else {
                $('#avaitext1').val("");
                $('#avaitext2').val("");
                $('#avaitotaltext').val("");
                showNormanMessage(data);
                var result = [];
                AvailabilityMixingRollMachine.onDrawChart(result);
            }
            
        },
        onDrawChart: function (result) {
            var datasets = [];
            var labels = [];
            for (i = 0; i < result.length; i++) {
                var datai = [];
                for (var j = 0; j < result[i].length; j++) {
                    var obj = { x: result[i][j].time, y: result[i][j].val };
                    datai.push(obj);
                }
                var label = result[i][0].Ser;
                //console.log(label);
                //label = label.substring(0, label.length - 1);
                var ob = {
                    label: label,
                    data: datai,
                    backgroundColor: result[i][0].col,
                    lineTension: 0,
                    borderColor: result[i][0].col,
                    fill: false
                }
                datasets.push(ob);
            }
            var chart = new Chart(chartAvailability,
            {
                type: 'line',
                options: {
                    title: {
                        display: true,
                        position: 'top',
                        fontSize: 20,
                        text: 'Roll Machine Availability'
                    },
                    legend: {
                        display: true,
                        position: "top"
                    },
                    tooltips: {
                        callbacks: {
                            label: function (tooltipItem) {
                                return tooltipItem.yLabel;
                            }
                        }
                    },
                    scales: {
                        yAxes: [
                            {
                                ticks: {
                                    beginAtZero: true,
                                },
                                scaleLabel: {
                                    display: true,
                                    fontSize: 20,
                                    labelString: "Mixing Roll M/C Number"
                                }
                            }
                        ],
                        xAxes: [
                            {
                                type: 'time',
                                display: result.length,
                                //unit: "hour",
                                time: {
                                    displayFormats: {
                                        minute: 'h:mm:ss'
                                    }
                                },
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
                    datasets: datasets
                }
            });
        }
    };
}();
AvailabilityMixingRollMachine.init();