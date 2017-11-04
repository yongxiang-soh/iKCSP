
$(document).ready(function () {
  
    $("#btnDeletePdtPln").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#PdtPlnGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDeletePdtPln').prop("disabled", false) : $('#btnDeletePdtPln').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });
    $("#YearMonth").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $("#YearMonth").keyup(function (e) {
        if (e.keyCode === 27 && $("#YearMonth").val() != "") {
            $("#YearMonth").val("");
            search();
            return false;
        }
    });
   
});

function addNewPdtPln() {
    $.ajax({
        url: formUrlPdtPln.urlEdit,
        type: 'GET',
        data: { preProdCode: null, prodDate: null, line: $("input[name='KndLine']:checked").val() },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F39_PreProductCode').focus();
          //  $('#F39_KndEptBgnDate').val('');
            disableButton();
        }
    });
}
function edit() {
    var date =gridHelper.displayDate( $('#PdtPlnGrid').find('input:checked').val());
    var data = gridHelper.getSelectedItem("PdtPlnGrid");
    var status = gridHelper.getSelectedItem("PdtPlnGrid").StatusYet;
    if (status !== "Yet") {
        alert("You cannot update the item which [Status] is not “Yet”.");
        return;
    }
    $.ajax({
        url: formUrlPdtPln.urlEdit,
        type: 'GET',
        data: {
            date: date,
            preProdCode: data.F39_PreProductCode,
            line: $("input[name='KndLine']:checked").val()
        },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F39_PreProductName').focus();
            
        }
    });
}
function editPdtPln(preProdCode, prodDate) {
   $.ajax({
        url: formUrlPdtPln.urlEdit,
        type: 'GET',
        data: { preProdCode: preProdCode, prodDate: prodDate },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F39_PreProductName').focus();
            
        }
    });
}
function hideFormPdtPln() {
    $("#formPlaceholder").html("");
    enableButton("PdtPlnGrid");
}

function searchPdtPln() {
    tablePdtPln.search($("#txtF39_KndEptBgnDateSearch").val().trim()).draw();
}
function search() {
    if ($("#search").valid()) {
        var request = {
            YearMonth: $("#YearMonth").val(),
            KndLine: $("#KndLine:checked").val()
        };
        if (checkSearch()) {
            if (confirm("Unsaved data will be lost. Are you sure to search?")) {
                hideFormPdtPln();
                gridHelper.searchObject("PdtPlnGrid", request);
            }
        } else {
            gridHelper.searchObject("PdtPlnGrid", request);
        }
    }
}


function deleteItemPdtPln() {
    var date = gridHelper.displayDate($('#PdtPlnGrid').find('input:checked').val());
    var data = gridHelper.getSelectedItem("PdtPlnGrid").F39_PreProductCode;
    var status = gridHelper.getSelectedItem("PdtPlnGrid").StatusYet;
    if (status!=="Yet") {
        alert("You cannot delete the item which [Status] is not “Yet”.");
        return;
    }
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: formUrlPdtPln.urlDelete,
            type: 'POST',
            data: {
                date: date,
                code: data
            },
            success: function (data) {
                showMessage(data, "PdtPlnGrid");
            }
        });
    };
}

function SelectProductCode(el) {
    var productCode = $(el).val();
    $.ajax({
        url: formUrlPdtPln.urlGetProductName,
        type: 'GET',
        data: { preProductCode: productCode },
        success: function (data) {
            //$("#txtPreProductName").val(data.result.F03_PreProductName);
            $("#F03_PreProductName").val(data.result.F03_PreProductName);
        }
    });
}

function onSuccess(data, status, xhr) {
   showMessage(data, "PdtPlnGrid");
    $("#formPlaceholder").html("");
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    enableButton("PdtPlnGrid");

}