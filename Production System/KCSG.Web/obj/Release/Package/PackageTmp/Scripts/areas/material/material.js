$(document).ready(function () {
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
   // Handle click on checkbox to set state of "Select all" control
  $('#MaterialGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
        counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
    });
    $("#txtMaterialCodeSearch").focus();
    $("#searchForm").keypress(function (e) {
       if (e.keyCode === 13) {
            search();
            return false;
        }
    });
    $("#SimulationType").change(function() {
        alert(this.val());
    });
});

function addNew() {
    edit(null);
}

function edit(id) {
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
    enableButton("MaterialGrid");
    $("#formPlaceholder").html("");
    
}


function search() {
        var request = {
            MaterialCode: $("#txtMaterialCodeSearch").val().trim()
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

function UpdateItem() {
    var id = $('#MaterialGrid').find('input:checked').val();
    edit(id);
}
    function deleteItem() {
        var id = $('#MaterialGrid').find('input:checked').val();
        if(confirm("Are you sure to delete selected item(s)?")) {
            $.ajax({
                url: formUrl.urlDelete,
                type: 'POST',
                data: { id: id },
                success: function (data) {
                    var request = {
                        MaterialCode: $("#txtMaterialCodeSearch").val().trim()
                    };
                    showMessage(data, "MaterialGrid", request);
                    //gridHelper.searchObject("MaterialGrid");
                    $('#btnDelete').prop("disabled", true);
                    $('#btnUpdate').prop("disabled", true);
                }
            });
        };
    }

    function onSuccess(data, status, xhr) {
        var request = {
            MaterialCode: $("#txtMaterialCodeSearch").val().trim()
        };
        showMessage(data, "MaterialGrid", request);
    $("#formPlaceholder").html("");
    enableButton("MaterialGrid");
    $("[id^='btnUpdate'").prop("disabled", true);
    $("[id^='btnDelete'").prop("disabled", true);
}


