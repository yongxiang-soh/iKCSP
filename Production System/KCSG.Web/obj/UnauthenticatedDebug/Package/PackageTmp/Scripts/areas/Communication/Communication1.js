$().ready(function() {
    var d = new Date();
    var datestring = ("0" + d.getDate()).slice(-2) +
        "/" +
        ("0" + (d.getMonth() + 1)).slice(-2) +
        "/" +
        d.getFullYear() +
        " " +
        ("0" + d.getHours()).slice(-2) +
        ":" +
        ("0" + d.getMinutes()).slice(-2) +
        ":" +
        ("0" + d.getSeconds()).slice(-2);

    $("#EditLog").val(datestring + " Hello World. ");
    $("input[name='Status']").each(function() {

        if ($(this).val() === "2") {
            $(this).prop("disabled", true);
        }
    });
});

function ChangeViewSelected(el) {
    $("#dvLog").hide();
    $("#dvQueue").hide();
    $("#dvHistory").hide();
    disabledButton();
    var request = {
        date: $("#Date").val(),
        Terminal: $("#Terminal").val()
    }
    switch ($(el).val()) {
    case "0":
        $("#dvLog").show();
        //$("#Date").attr("disabled", true);
        //$("#Terminal").attr("disabled", true);
        //$("#btnFind").attr("disabled", true);
        break;
    case "1":
        $("#dvQueue").show();
        showButton("Q");
        gridHelper.searchObject("QueueGrid", request);
        break;
    case "2":
        $("#dvHistory").show();
        gridHelper.searchObject("HistoryGrid", request);
        showButton("");
        break;
    }
}

function disabledButton() {
    $("#btnDelete").attr("disabled", true);
    $("#btnCancel").attr("disabled", true);
    $("#btnEnd").attr("disabled", true);
    $("#Date").attr("disabled", true);
    $("#Terminal").attr("disabled", true);
    $("#btnFind").attr("disabled", true);
    $("#Date").val("");
    $("#Terminal").val("");
}

function showButton(a) {
    if (a === "Q") {
        $("#btnCancel").attr("disabled", false);
        $("#btnEnd").attr("disabled", false);
    }
    $("#Date").attr("disabled", false);
    $("#Terminal").attr("disabled", false);
    $("#btnFind").attr("disabled", false);
    $("#btnDelete").attr("disabled", false);

}

function ChangeStatus(el) {

    $.ajax({
        url: formActionUrl.urlStatus,
        data: { status: parseInt($(el).val()) },
        type: 'POST',
        success: function(data) {
            showMessage(data);
        }
    });
}

function ChangeStatusRequest(el) {
    $.ajax({
        url: formActionUrl.urlRequestTimer,
        data: {
            request: $(el).val()
        },
        type: 'POST',
        success: function(data) {
            showMessage(data);
        }
    });
}

function Conveyor() {
    var communication = $("#Communcation").val();
    $.ajax({
        url: formUrl.urlConveyor + "?communication=" + communication,
        type: 'GET',
        success: function(data) {
            $('#conveyor').modal('toggle');
            $("#conveyor .modal-body").html(data);
        }
    });
}

function onSucessfull(data) {
    showMessage(data, "ConveyorGrid");
}

function Delete() {
    if ($('#dvQueue').css('display') == 'block') {
        DeleteQueue();
    }
    if ($('#dvHistory').css('display') == 'block') {
        DeleteHistory();
    }
}

function DeleteQueue() {
    var status = gridHelper.getSelectedItem("QueueGrid").Status;
    if (parseInt(status) > 5) {
        if (confirm(mesage.MSG3)) {
            var commandNo = gridHelper.getSelectedItem("QueueGrid").CommandNo;
            var cmdSeqNo = gridHelper.getSelectedItem("QueueGrid").CommandSeqNo;
            $.ajax({
                url: formActionUrl.urlDelete,
                data: { commandNo: commandNo, cmdSeqNo: cmdSeqNo },
                type: 'POST',
                success: function(data) {
                    showMessage(data, "QueueGrid");
                }
            });
        }
    } else {
        var data = { Success: false, Message: "The command can’t be deleted." }
        showMessage(data);
    }
}

function DeleteHistory() {
    if (confirm(mesage.MSG5)) {
        $.ajax({
            url: formActionUrl.urlDeleteHistory,
            data: $("#indexFrom").serialize(),
            type: 'POST',
            success: function(data) {
                showMessage(data, "HistoryGrid");
            }
        });
    }
}

function End() {
    var status = gridHelper.getSelectedItem("QueueGrid").Status;
    if (parseInt(status) <= 5) {
        if (confirm(mesage.MSG6)) {
            var commandNo = gridHelper.getSelectedItem("QueueGrid").CommandNo;
            var cmdSeqNo = gridHelper.getSelectedItem("QueueGrid").CommandSeqNo;

            $.ajax({
                url: formActionUrl.urlEnd,
                data: { commandNo: commandNo, cmdSeqNo: cmdSeqNo },
                type: 'POST',
                success: function(data) {
                    showMessage(data, "QueueGrid");
                }
            });
        }
    } else {

        var data = { Success: false, Message: "The command can’t be ended." }
        showMessage(data);
    }
}

function Cancel() {
    var status = gridHelper.getSelectedItem("QueueGrid").Status;
    if (parseInt(status) <= 5) {
        if (confirm(mesage.MSG8)) {
            var commandNo = gridHelper.getSelectedItem("QueueGrid").CommandNo;
            var cmdSeqNo = gridHelper.getSelectedItem("QueueGrid").CommandSeqNo;
            $.ajax({
                url: formActionUrl.urlCancel,
                data: { commandNo: commandNo, cmdSeqNo: cmdSeqNo },
                type: 'POST',
                success: function(data) {
                    showMessage(data, "QueueGrid");
                }
            });
        }
    } else {

        var data = { Success: false, Message: "The command can’t be Cancelled." }
        showMessage(data);
    }
}

function Find() {
    if ($("#indexFrom").valid()) {
        var request = {
            date: $("#Date").val(),
            Terminal: $("#Terminal").val()
        }

        if ($('#dvQueue').css('display') == 'block') {

            gridHelper.searchObject("QueueGrid", request);

        }
        if ($('#dvHistory').css('display') == 'block') {

            gridHelper.searchObject("HistoryGrid", request);
        }
    }
}