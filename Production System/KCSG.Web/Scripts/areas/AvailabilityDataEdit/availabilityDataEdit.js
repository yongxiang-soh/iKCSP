
var AvailabilityDataEdit = function() {
    return {
        init: function() {
            var environmentDate = $('#EnvironmentDate').val();
            var currentDate = moment().format("DD/MM/YYYY");
            if (environmentDate === currentDate) {
                setInterval(function() { AvailabilityDataEdit.onExecute() }, 300000); // For 5 minus
            } else {
                AvailabilityDataEdit.onExecute();
            }
            AvailabilityDataEdit.onExecute();
        },
        onExecute:function() {
            var dataPost = {
                Mode: $("input[name='EnvMode']:checked").val(),
                EnvironmentDate: $('#EnvironmentDate').val(),
                Machine: $('#Machine :selected').val()
            };
            $.ajax({
                url: '/EnvironmentManagement/AvailabilityDataEdit/Search',
                type: "post",
                data: dataPost,
                success: AvailabilityDataEdit.onSuccess
            });
        },
        onGo: function () {
            // 
            var environmentDate = $('#EnvironmentDate').val(),
                mode = $("input[name='EnvMode']:checked").val(),
                machine = $('#Machine :selected').val();
            window.location.href = "/EnvironmentManagement/AvailabilityDataEdit/Index?environmentDate=" + environmentDate + "&envMode=" + mode + "&machine=" + machine;
        },
        onSuccess: function(data) {
            //console.log(data);
            //var result = JSON.parse(data.grapdata);
            $.unblockUI();
            if (data.Success) {
                //
                var result = JSON.parse(data.grapdata);
                var chartAvailability = document.getElementById("chartAvailability");
                //if (result.length > 0) {
                $('#avaitext').val(data.sumAvai);
                $('#colorChart').show();
                var datasets = [];
                var labels = [];
                for (i = 0; i < result.length; i++) {
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
                            text: 'Roll Machine Availability'
                        },
                        legend: {
                            display: true
                        },
                        tooltips: {
                            callbacks: {
                                label: function(tooltipItem) {
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
                                    labelString: "Maxing Roll M\C Number"
                                }
                            }],
                            xAxes: [
                                {
                                    type: 'time',
                                    display: true,
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
                //} else {
                //    $('#avaitext').val("");
                //    $('#sumAvai').val("");
                //    $('#colorChart').hide();
                //}

            }

            $.unblockUI();
        },
    };
}();
AvailabilityDataEdit.init();

function Update() {
    $("#statusError").text("").hide();
    var isChecked = CheckRadioButtonIsCheckedOrNot();
    if ($("#availability-data-edit").valid() && isChecked) {
        var status = $('input[name="environmentStatus"]:checked').val();

        var machineValue = $("#Machine option:selected").val();
        var environmentTime = $("#EnvironmentDate").val();
        var time = $("#Time").val();
        $.ajax({
            url: "/EnvironmentManagement/AvailabilityDataEdit/Edit",
            type: "post",
            data: {
                status: status,
                environmentTime: environmentTime,
                time: time,
                machineValue: machineValue
            },
            success: function(data) {
                $.unblockUI();
                if (data.Success) {
                    showMessage(data);
                    ResetData();
                }
            }
        });
    }
}

$("#dvTime").on('dp.change', function() {
    GetStatus(); 
});




function GetStatus() {
    $("#rdRotate").prop('checked', false);
    $("#rdEmpty").prop('checked', false);
    $("#rdStop").prop('checked', false);

    var environmentDate = $("#EnvironmentDate").val();
    var time = $("#Time").val();
    var machineValue = $("#Machine option:selected").val();
    $.ajax({
        url: "/EnvironmentManagement/AvailabilityDataEdit/GetStatus",
        type: "post",
        data: {
            machineValue: machineValue,
            environmentDate: environmentDate,
            time: time
        },
        success: function (data) {
            
            $.unblockUI();
            if (data.result != null) {
                $("#errorTime").text("").hide();
                if (data.result == "O" || data.result == "A") {
                    $("input[value=" + data.result + "]").prop('checked', true);
                } else {
                    $("input[value=S]").prop('checked', true);
                }
            } else {
                $("#rdRotate").prop('checked', false);
                $("#rdEmpty").prop('checked', false);
                $("#rdStop").prop('checked', false);
                $("#errorTime").text("Record not found!").show();
            }
        }
    });

}

function CheckRadioButtonIsCheckedOrNot() {
    if ($("#rdRotate").is(":not(:checked)") && $("#rdEmpty").is(":not(:checked)") && $("#rdStop").is(":not(:checked)")) {
        $("#statusError").text("Invalid Status.").show();
        return false;
    }
    $("#statusError").text("").hide();
    return true;
}

function ResetData() {
    $("#Time").val("");
    $("#rdRotate").prop('checked', false);
    $("#rdEmpty").prop('checked', false);
    $("#rdStop").prop('checked', false);
}