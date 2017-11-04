
function ChangeViewSelected(el) {
    $("#dvMonitor").hide();
    $("#dvMaterial").hide();
    $("#dvPreproduct").hide();
    $("#dvRetrieval").hide();
    $("#dvKneadingCommand").hide();
    $("#dvKneadingResult").hide();
   
    switch ($(el).val()) {
    case "0":
        $("#dvMaterial").show();
       
        
        break;
    case "1":
        $("#dvKneadingCommand").show();
        showButton();
       
        break;
    case "2":
        $("#dvPreproduct").show();
       
        showButton();
        break;
    case "3":
        $("#dvKneadingResult").show();
       
        showButton();
        break;
    case "4":
        $("#dvRetrieval").show();
       
        showButton();
        break;
    case "5":
        $("#dvMonitor").show();
        //gridHelper.searchObject("HistoryGrid", request);
        showButton();
        break;
    }
    Search();
}

function Search() {
    var request = {
        selectData: $("#SelectData:checked").val(),
        codeNo: $("#CodeNo").val(),
        date: $("#Date").val(),
        line:$("#Line").val()
}
    
    switch ($("#ViewSelect:checked").val()) {
    case "0":
       
        gridHelper.searchObject("Material", request);

        break;
    case "1":
       gridHelper.searchObject("KndCommand", request);
        break;
    case "2":
       gridHelper.searchObject("Preproduct", request);
       break;
    case "3":
      gridHelper.searchObject("KndResult", request);
       break;
    case "4":
       gridHelper.searchObject("Retrieval", request);
        break;
    }
}
function ChangeStatusRequest() {
    $("#btnGo").click();
}
function showButton() {
    
}