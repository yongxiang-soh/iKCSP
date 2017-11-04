$(document).ready(function () {
    $("#btnStore").prop("disabled", true);
    $("#btnInventory").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control

    $('#txtSubMaterialCodeSearch').keyup(function () {
        var hasValue = this.value.length > 0;
        $("#errorSubMaterialCode").text("").hide();
    });

    $('#MaterialGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnStore').prop("disabled", false) : $('#btnStore').prop("disabled", true);
        counterChecked > 0 ? $('#btnInventory').prop("disabled", false) : $('#btnInventory').prop("disabled", true);
    });
    $("#txtSubMaterialCodeSearch").focus();
    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            search();
            return false;
        }
    });
});


function StoreItem() {
    
    var id = $('#MaterialGrid').find('input:checked').val();
    edit(id,true);
}

function edit(id, isStore) {    
    $.ajax({
        url: formUrl.urlEdit,
        type: 'GET',
        data: { id: id, isStore: isStore },
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F15_MaterialDsp').focus();
        }
    });
}

function hideForm() {
    $("#formPlaceholder").html("");
}

function search() {
    if ($('#txtSubMaterialCodeSearch').val() === "" || $('#txtSubMaterialCodeSearch').val()===null) {
        $("#errorSubMaterialCode").text(message.MSG02).show();
        return;
    }
    $("#errorSubMaterialCode").text("").hide();
    var request = {
        SubMaterialCode: $("#txtSubMaterialCodeSearch").val().trim()
    };
    gridHelper.searchObject("MaterialGrid", request);
}

function InventoryItem() {
    var id = $('#MaterialGrid').find('input:checked').val();
    edit(id,false);
}


function onSuccess(data, status, xhr) {
    //search();   
    showMessage(data, 'MaterialGrid', "");
    $("#formPlaceholder").html("");
    $("#btnStore").prop("disabled", true);
    $("#btnInventory").prop("disabled", true);
}

//Check validation 
function CheckValidation() {
    
    var checked = true;
    if (parseFloat($("#Fraction").val()) >= parseFloat($("#F15_PackingUnit").val())) {
        $("#FractionError").text(message.MSG25).show();
        checked = false;
    }
    var addquantity = parseFloat($("#AddQuantity").val());
    var b = findRealNumber($("#InventoryQuantity").val());  
    var inventory = parseFloat(b.replace(/[^\d\.\-eE+]/g, ""));
    
    if ((addquantity + inventory) > 1000000) {
        $("#InventoryQuantityError").text(message.MSG26).show();
        checked = false;
    }
    if (checked === false)
        return false;
    return true;
}

function OK() {
    
    if ($("#addNewForm").valid()) {
        $("#errorList").text("").hide();
        $("#InventoryQuantityError").text("").hide();
        $("#FractionError").text("").hide();
        var test = $("#IsStore").val();
        if (test === 'true') {
            if (!CheckValidation())
                return;
        }        
        $("#addNewForm").submit();
    }
}

findRealNumber = function (number) {

    // Replace all separators.
    number = number.replace(/','/g, '');

    // Find index of point.
    var pointIndex = number.indexOf('.');

    if (pointIndex == -1)
        return number;
    return number.substring(0, pointIndex);
}

function CaculateAddQuantity() {
    var isStore = $("#IsStore").val();
    if (isStore == "true") {
        var packQuantity = parseFloat($("#PackQuantity").autoNumeric('get'));
        var packUnit = parseFloat($("#F15_PackingUnit").autoNumeric('get'));
        var fraction = parseFloat($("#Fraction").autoNumeric('get'));
        var addQuantity = (packQuantity * packUnit) + fraction;
        $("#AddQuantity").val(parseFloat(addQuantity).toFixed(2).replace(/(\d)(?=(\d\d\d)+(?!\d))/g, "$1,"));
    }
}


