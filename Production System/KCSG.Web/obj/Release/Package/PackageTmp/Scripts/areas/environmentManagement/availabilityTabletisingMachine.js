
var AvailabilityTabletisingMachine = function () {
    var chartAvailability = document.getElementById("chartAvailability");
    return {
        init: function () {
            $('#go').click();
        },
        onSuccess: function (data) {
            //console.log(data);
            //var result = JSON.parse(data.grapdata);
            $.unblockUI();
            if (data.Success) {
                $('#avaitext3').val(data.avai3tex);
                $('#avaitext4').val(data.avai4tex);
                $('#avaitext5').val(data.avai5tex);
                $('#avaitext6').val(data.avai6tex);
                $('#avaitext7').val(data.avai7tex);
                $('#avaitext8').val(data.avai8tex);
                $('#avaitext9').val(data.avai9tex);
                $('#sumAvai').val(data.sumAvai);
                var result = JSON.parse(data.grapdata);
                AvailabilityTabletisingMachine.onDrawChart(result);
                
                
            } else
            {
                $('#avaitext3').val("");
                $('#avaitext4').val("");
                $('#avaitext5').val("");
                $('#avaitext6').val("");
                $('#avaitext7').val("");
                $('#avaitext8').val("");
                $('#avaitext9').val("");
                $('#sumAvai').val("");
                showNormanMessage(data);
                var result = [];
                AvailabilityTabletisingMachine.onDrawChart(result);
            }
        },
        onDrawChart: function (result) {
            var datasets = [];
            var labels = [];
            for (var i = 0; i < result.length; i++) {
                var datai = [];
                for (var j = 0; j < result[i].length; j++) {
                    var obj = { x: result[i][j].time, y: result[i][j].val };
                    datai.push(obj);
                }
                var label = result[i][0].Ser;
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
                        text: 'Tabletising Machine Availability'
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
                        yAxes: [{
                            ticks: {
                                beginAtZero: true
                            },
                            scaleLabel: {
                                display: true,
                                fontSize: 20,
                                labelString: "Tabletising M/C Number"
                            }
                        }],
                        xAxes: [
                            {
                                type: 'time',
                                display: result.length,
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
AvailabilityTabletisingMachine.init();