function StartOrEnd() {
    var isConfirm = false;
    if ($("#IsStart").val()=="False") {
        var startStatus = $("#dvStart").find('input:checked').val();
        if (startStatus == "0") {
            isConfirm = confirm("Ready to start this system?");
        } else {
            isConfirm = confirm("Ready to re-start this system?");
            
        }
    } else {
      
        var statusEnd = $("#dvEnd").find('input:checked').val();
        if (statusEnd == "1") {
            isConfirm = confirm("Ready to end this system?");
          
        } else {
            isConfirm = confirm("Ready to force-end this system?");
        }
    }
 

    if (isConfirm) {
        $("#startOrEndForm").submit();
    }
}

function onSuccess(data) {
    
    $("#startOrEndForm").html(data);

}

$(function () {

    var notificationHub = $.connection.notificationHub;
    notificationHub.client.receiveSystemNotification = function (terminalNo, ipAddress, message) {
        toastr["info"](message);
    }
    $.connection.hub.start().done();
});