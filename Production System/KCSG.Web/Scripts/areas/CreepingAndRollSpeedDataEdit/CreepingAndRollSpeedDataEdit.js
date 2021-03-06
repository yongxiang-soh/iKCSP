﻿

var CreepingAndRollSpeedDataEdit = function () {

    return {
        init: function () {
            //var environmentDate = $('#EnvironmentDate').val();
            //var currentDate = moment().format("DD/MM/YYYY");
            //if(environmentDate === currentDate) {
            //    setInterval(function () { $('#go').click(); }, 300000); // For 5 minus
            //}
            //$('#go').click();
            CreepingAndRollSpeedDataEdit.onExecute();
        },
        onExecute: function () {
            var machine = $('#Machine :selected').val();
            var environmentDate = $('#EnvironmentDate').val();
           
            
            if (machine != null && machine != "" && environmentDate != null && environmentDate != "") {
                var dataPost = {
                    envMode: $("input[name='EnvMode']:checked").val(),
                    environmentDate: $('#EnvironmentDate').val(),
                    machine: $('#Machine :selected').val()
                };
            $.ajax({
                url: '/EnvironmentManagement/CreepingAndRollSpeedDataEdit/Search',
                type: "post",
                data: dataPost,
                success: CreepingAndRollSpeedDataEdit.onSuccess
            });
            }
        },
        onGo: function () {
            if (!$("#formCreepingAndRollSpeedDataEdit").valid())
                return;
                var environmentDate = $('#EnvironmentDate').val(),
                    envMode = $("input[name='EnvMode']:checked").val(),
                    machine = $('#Machine :selected').val();
                window.location.href = "/EnvironmentManagement/CreepingAndRollSpeedDataEdit/Index?environmentDate=" + environmentDate + "&envMode=" + envMode + "&machine=" + machine;
        },
        onSuccess: function (data) {
            //console.log(data);
            if (!data.Success) {
                $('#leftCreepHight').val("");
                $('#leftCreepLow').val("");
                $('#leftCreepRanger').val("");
                $('#leftCreepMean').val("");
                $('#leftCreepSigma').val("");

                $('#rightCreepHight').val("");
                $('#rightCreepLow').val("");
                $('#rightCreepRanger').val("");
                $('#rightCreepMean').val("");
                $('#rightCreepSigma').val("");

                $('#rollSpeedHigh').val("");
                $('#rollSpeedLow').val("");
                $('#rollSpeedRanger').val("");
                $('#rollSpeedMean').val("");
                $('#rollSpeedSigma').val("");

                if (data.ErrorCode === -2) {
                    showNormanMessage(data);
                } else {
                    $.unblockUI();
                }
            }
            if (data.Success) {
                //console.log(data);
                $('#leftCreepHight').val(data.left_high);
                $('#leftCreepLow').val(data.left_low);
                $('#leftCreepRanger').val(data.left_range);
                $('#leftCreepMean').val(data.left_mean);
                $('#leftCreepSigma').val(data.left_sigma);

                $('#rightCreepHight').val(data.right_high);
                $('#rightCreepLow').val(data.right_low);
                $('#rightCreepRanger').val(data.right_range);
                $('#rightCreepMean').val(data.right_mean);
                $('#rightCreepSigma').val(data.right_sigma);

                $('#rollSpeedHigh').val(data.spd_high);
                $('#rollSpeedLow').val(data.spd_low);
                $('#rollSpeedRanger').val(data.spd_range);
                $('#rollSpeedMean').val(data.spd_mean);
                $('#rollSpeedSigma').val(data.spd_sigma);

                var leftCreep = document.getElementById("leftCreep");
                var leftCreepChart = new Chart(leftCreep,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'left',
                            text: 'Left Creeping'
                        },

                    },
                    data: {
                        labels: JSON.parse(data.left_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.left_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor: "RGB(0, 0, 255)",
                                fill: false
                            },
                            {
                                label: "DEC MEAN",
                                data: JSON.parse(data.left_fld_mean),
                                backgroundColor: "RGB(0, 255, 0)",
                                lineTension: 0,
                                borderColor: "RGB(0, 255, 0)",
                                fill: false
                            },
                            {
                                label: "DEC UPPER",
                                data: JSON.parse(data.left_fld_upper),
                                backgroundColor: "RGB(255, 0, 0)",
                                lineTension: 0,
                                borderColor: "RGB(255, 0, 0)",
                                fill: false
                            },
                            {
                                label: "DEC LOWER",
                                data: JSON.parse(data.left_fld_lower),
                                backgroundColor: "RGB(128, 128,0)",
                                lineTension: 0,
                                borderColor: "RGB(128, 128, 0)",
                                fill: false
                            }
                        ]
                    }
                });


                var rightCreep = document.getElementById("rightCreep");
                var rightCreepChart = new Chart(rightCreep,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'left',
                            text: 'Right Creeping'
                        },
                    },
                    data: {
                        labels: JSON.parse(data.right_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.right_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor: "RGB(0, 0, 255)",
                                fill: false
                            },
                            {
                                label: "DEC MEAN",
                                data: JSON.parse(data.right_fld_mean),
                                backgroundColor: "RGB(0, 255, 0)",
                                lineTension: 0,
                                borderColor: "RGB(0, 255, 0)",
                                fill: false
                            },
                            {
                                label: "DEC UPPER",
                                data: JSON.parse(data.right_fld_upper),
                                backgroundColor: "RGB(255, 0, 0)",
                                lineTension: 0,
                                borderColor: "RGB(255,0, 0)",
                                fill: false
                            },
                            {
                                label: "DEC LOWER",
                                data: JSON.parse(data.right_fld_lower),
                                backgroundColor: "RGB(128, 128, 0)",
                                lineTension: 0,
                                borderColor: "RGB(128, 128, 0)",
                                fill: false
                            }
                        ]
                    }
                });
                console.log(data.right_fld_lower);
                var rollSpeed = document.getElementById("rollSpeed");
                var rollSpeedChart = new Chart(rollSpeed,
                {
                    type: 'line',
                    options: {
                        title: {
                            display: true,
                            position: 'left',
                            text: 'Roll speed'
                        },
                    },
                    data: {
                        labels: JSON.parse(data.spd_yAxis),
                        datasets: [
                            {
                                label: "DEC DATA",
                                data: JSON.parse(data.spd_fld_data),
                                backgroundColor: "RGB(0, 0, 255)",
                                lineTension: 0,
                                borderColor: "RGB(0, 0, 255)",
                                fill: false
                            },
                            {
                                label: "DEC MEAN",
                                data: JSON.parse(data.spd_fld_mean),
                                backgroundColor: "RGB(0, 255, 0)",
                                lineTension: 0,
                                borderColor: "RGB(0, 255, 0)",
                                fill: false
                            },
                            {
                                label: "DEC UPPER",
                                data: JSON.parse(data.spd_fld_upper),
                                backgroundColor: "RGB(255, 0, 0)",
                                lineTension: 0,
                                borderColor: "RGB(255,0, 0)",
                                fill: false
                            },
                            {
                                label: "DEC LOWER",
                                data: JSON.parse(data.spd_fld_lower),
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
CreepingAndRollSpeedDataEdit.init();




function Update() {
    if (!$("#frmUpdate").valid())
        return;
        var machine = $("#Location option:selected").text();
        var id1 = 0, id2 = 0, id3 = 0;
        if (machine == "18 Inch Mixing Roll Machine") {
            id1 = 1;
            id2 = 2;
            id3 = 5;
        } else {
            id1 = 3;
            id2 = 4;
            id3 = 6;
        }
        var environmentDate = $("#EnvironmentDate").val();
        var time = $("#Time").val();
        var leftCreeping = parseFloat($("#LeftCreeping").val());
        var rightCreeping = parseFloat($("#RightCreeping").val());
        var rollSpeed = parseFloat($("#RollSpeed").val());

        $.ajax({
            url: "/EnvironmentManagement/CreepingAndRollSpeedDataEdit/Edit",
            type: "post",
            data: {
                id1: id1,
                id2: id2,
                id3: id3,
                environmentDate: environmentDate,
                time: time,
                leftCreeping: leftCreeping,
                rightCreeping: rightCreeping,
                rollSpeed: rollSpeed
            },
            success: function (data) {
                $.unblockUI();
                if (data.Success) {
                    showMessage(data);
                    setTimeout(function() {
                        CreepingAndRollSpeedDataEdit.init();
                    }, 3000);
                }
            }
        });
    
}

$("#dvTime").on('dp.change', function() {
    ChangeValueOfTime();
});

function ChangeValueOfTime() {
    var machine = $("#Location option:selected").text();
    var id1 = 0, id2 = 0, id3 = 0;
    if (machine == "18 Inch Mixing Roll Machine") {
        id1 = 1;
        id2 = 2;
        id3 = 5;
    } else {
        id1 = 3;
        id2 = 4;
        id3 = 6;
    }
    var environmentDate = $("#EnvironmentDate").val();
    var time = $("#Time").val();

    $.ajax({
        url: "/EnvironmentManagement/CreepingAndRollSpeedDataEdit/ChangeValueOfTime",
        type: "post",
        data: {
            environmentDate: environmentDate,
            time: time,
            id1: id1,
            id2: id2,
            id3: id3
        },
        success: function (data) {
            $.unblockUI();
          

            if (data.Item1 != null || data.Item2 != null || data.Item3 != null) {
                $("#lstError").text("").hide();
                $("#Time").removeClass('input-validation-error');
                if (data.Item1 != null) {
                    $("#LeftCreeping").autoNumeric("set", data.Item1.F83_Value);
                } else {
                    $("#LeftCreeping").val("");
                }

                if (data.Item2 != null) {
                    $("#RightCreeping").autoNumeric("set", data.Item2.F83_Value);
                } else {
                    $("#RightCreeping").val("");
                }

                if (data.Item3 != null) {
                    $("#RollSpeed").autoNumeric("set", data.Item3.F83_Value);
                } else {
                    $("#RollSpeed").val("");
                }
            } else {
                $("#lstError").text("Record not found!").show();
                $("#Time").addClass('input-validation-error');
            }
        }
    });
}

