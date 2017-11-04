$().ready(function () {
    $.validator.unobtrusive.parse('#conveyorFrom');
    $("#UsingBuffer").rules("add", "CheckUsingBuffer");
});
$.validator.addMethod("CheckUsingBuffer", function() {
    var max = parseInt($("#MaxBuffer").val());
    var using = parseInt($("#UsingBuffer").autoNumeric("get"));
    if (max < using) {
        return false;
    }
    return true;
}, "Using Buffer cannot higher than Max Buffer.");

function LoadConveyorSuccess() {
    $('#ConveyorGrid').find("tr").attr("onclick", "onSelectedGrid()");
}

function onSelectedGrid() {
    var code = gridHelper.getSelectedItem("ConveyorGrid").F05_ConveyorCode;
    $("#btnUpdate").prop("disabled", false);
    $.ajax({
        url: urlConveryor.urlEdit,
        type: 'GET',
        data: { code: code },
        success: function (data) {
            $("#ConveyorCode").val(data.F05_ConveyorCode);
            $("#UsingBuffer").autoNumeric("set",data.F05_BufferUsing);
            $("#MaxBuffer").val(data.F05_MaxBuffer);
            $("input[name=ConveyorStatus][value=" + data.F05_StrRtrSts + "]").prop('checked', true);
        }
    });
}

function UpdateConveyor() {
    if ($("#conveyorFrom").valid()) {
        $.ajax({
            url: urlConveryor.urlSave,
            type: 'POST',
            data: $("#conveyorFrom").serialize(),
            success: function(data) {
                showMessage(data, "ConveyorGrid");
            }
        });
    }

}