function DailyProcess() {
  
    $.ajax({
        url: "/Systemmanagement/DailyProcess/DailyProcess",
        type: 'POST',
        success: function (data) {
            showMessage(data, "DailyProcess", "");
        }
    });
}
