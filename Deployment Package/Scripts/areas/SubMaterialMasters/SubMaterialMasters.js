$(document).ready(function () {
  
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control
    $('#MaterialGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $("#btnUpdate").prop("disabled", false) : $("#btnUpdate").prop("disabled", true);
       
    });
    
    $("#txtCodeSearch").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
});

function addNew() {
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: null },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F01_MaterialCode').focus();
            disableButton();
        }
    });
}

function UpdateItem() {
    var id = $('#MaterialGrid').find('input:checked').val();
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: id },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F01_MaterialDsp').focus();
            disableButton();
        }
    });
}
function hideForm() {
    $("#formPlaceholder").html("");
    enableButton("MaterialGrid");
}

function search() {
    var request = {
        MaterialCode: $("#txtCodeSearch").val()
    };
    if (checkSearch()) {
        if (confirm("Unsaved data will be lost. Are you sure to search?")) {
            hideForm();
            gridHelper.searchObject("MaterialGrid", request);
        }
    } else {
        gridHelper.searchObject("MaterialGrid", request);
    }
}

function onSuccess(data, status, xhr) {
    var request = {
        MaterialCode: $("#txtCodeSearch").val()
    };
    showMessage(data, "MaterialGrid", request);
    $("#formPlaceholder").html("");
    enableButton("MaterialGrid");
    $("#btnUpdate").prop("disabled", true);
    $("#btnDelete").prop("disabled", true);

}

function deleteItem() {
    var id = $('#MaterialGrid').find('input:checked').val();
    if (confirm("Are you sure to delete selected item(s)?")) {
        $.ajax({
            url: formUrl.urlDelete,
            type: 'POST',
            data: { id: id },
            success: function (data) {
                showMessage(data, "MaterialGrid", "");
                enableButton("MaterialGrid");
                $("#btnUpdate").prop("disabled", true);
                $("#btnDelete").prop("disabled", true);
            }
        });
    };
}


