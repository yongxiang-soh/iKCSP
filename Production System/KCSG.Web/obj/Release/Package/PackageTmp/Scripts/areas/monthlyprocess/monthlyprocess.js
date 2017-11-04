function DailyProcess() {
    $.ajax({
        url: "/Systemmanagement/MonthlyProcess/MonthlyProcess",
        type: 'POST',
        data: { terminalNo: terminalNo },
        success: function (data) {
            showMessage(data, "DailyProcess");
        }
    });
}
