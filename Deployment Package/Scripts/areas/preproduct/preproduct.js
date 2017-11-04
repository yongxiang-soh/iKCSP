$(document).ready(function () {
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true); $("#btnAdd").prop("disabled", false);

    $('#PreproductGrid').on('change', 'input[type="radio"]', function () {
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        if (counterChecked > 0) {
            $('#btnDelete').prop("disabled", false);
            $('#btnUpdate').prop("disabled", false);
        } else {
            $('#btnDelete').prop("disabled", true);
            $('#btnUpdate').prop("disabled", true);
        }
    });

    $("#searchForm").keypress(function (e) {
        if (e.keyCode === 13) {
            searchPreProduct();
            return false;
        }
    });

    $("#txtPreProductCodeSearch").focus();
});

function searchPreProduct() {
    var request = { PreProductCode: $("#txtPreProductCodeSearch").val().trim() };
    if ($('#modificationArea').html() != "") {
        if (confirm("Unsaved data will be lost. Are you sure to search?"))
        {
            $("#modificationArea").html("");           
            gridHelper.searchObject("PreproductGrid", request);
        }  
    } else {
        gridHelper.searchObject("PreproductGrid", request);
    }
}

function addPreProduct() {
    $.ajax({        
        url: formUrlPreProduct.urlEdit,
        type: 'GET',
        data: { preProductCode: null },
        success: function (data) {
            disableButton();
            $("#modificationArea").html(data);
            $('#F03_PreProductCode').focus();
        }
    });
}

function updatePreProduct() {
    var preProductCode = $('#PreproductGrid').find('input:checked').val();    
    $.ajax({
        url: formUrlPreProduct.urlEdit,
        type: 'GET',
        data: { preProductCode: preProductCode },
        success: function (data) {
            disableButton();
            $("#modificationArea").html(data);
            $('#F03_PreProductName').focus();
        }
    });
}

function deletePreProduct() {
    var preProductCode = $('#PreproductGrid').find('input:checked').val();
    if (confirm("Are you sure to delete selected item?")) {
        $.ajax({
            url: formUrlPreProduct.urlDelete,
            type: 'POST',
            data: { preProductCode: preProductCode },
            success: function (response) {
                showMessage(response, "PreproductGrid", "");
                $('#btnDelete').prop("disabled", true);
                $('#btnUpdate').prop("disabled", true);
            }
        });
    };
}

function hideFormPreProduct(data) {
    $("#modificationArea").html("");
    var request = { PreProductCode: $("#txtPreProductCodeSearch").val().trim() };
    showMessage(data, "PreproductGrid", request);
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
}

function SavePreProduct() {
    //if (!validateMaterialTable()) {
    //        return;
    //    }
    // Validate add new pre-product form.
    // As the form is invalid. No action should be taken.
    
    var validate = validateMaterialTable();
    
    if ($("#addNewFormPreProduct").valid()===false || validate===false) {
        return;
    }
    // Validation is successful. Submit this form to service.
    $("#addNewFormPreProduct").submit();
}

function CancelPreproduct() {
    //$.ajax({
    //    url: formUrlPrePdtMkp.urlCancel,
    //    type: 'POST',
    //    data: { preProductCode:$("#F03_PreProductCode").val() },
    //    success: function (response) {
    //        $.unblockUI();
    //        $("#modificationArea").html("");
    //        enableButton("PreproductGrid");
    //    }
    //});

    location.reload();

}

function addMaterial() {
    var data = gridHelper.getListingData("PreproductMaterialGrid");
    if (data.length>=34) {
       alert("A pre-product can only be made up of 34 material or less !");
    } else 
    if ($("#addNewFormPreProduct #IsCreate").val() === "True" && $("#F03_PreProductCode").val()==="") {
        alert("You must add the Pre-Product Code  before adding Material!");
        
    } else {
        
        addUpdateMaterial(true);
    }   
}

function UpdateMaterial() {
    addUpdateMaterial(false);
}

function deleteMaterial() {
    var materialCode = $('#PreproductMaterialGrid').find('input:checked').val();
    var thrawSeqNo = gridHelper.getSelectedItem("PreproductMaterialGrid").F02_ThrawSeqNo;
    if (confirm("Are you sure to delete selected item?")) {
        $.ajax({
            url: formUrlPrePdtMkp.urlDelete,
            type: 'POST',
            data: {
                preProductCode: $("#F03_PreProductCode").val(),
                materialCode: materialCode,
                thrawSeqNo: thrawSeqNo
            },
            success: function (response) {
                var request = {
                    preProductId: $("#F03_PreProductCode").val()
                }
                showMessage(response, "PreproductMaterialGrid", request);
                $('#btnDeleteMaterial').prop("disabled", true);
                $('#btnUpdateMaterial').prop("disabled", true);
            }
        });
    };
}

function addUpdateMaterial(isCreate) {
    
    $("#btnSavePreProduct").prop('disabled', true);
    $("#btnCancelPreProduct").prop('disabled', true);

    var materialCode = "";
    var thrawSeqNo = "";
    var preproductName = $("#F03_PreProductName").val();
    var productcode = $("#F03_PreProductCode").val();
    if (!isCreate) {
        materialCode = $('#PreproductMaterialGrid').find('input:checked').val();
       
      //  thrawSeqNo = gridHelper.getSelectedItem("PreproductMaterialGrid").F02_ThrawSeqNo;
    } 
    $.ajax({
        url: formUrlPrePdtMkp.urlEdit,
        type: 'GET',
        data: {
            preProductCode: productcode,
            materialCode: materialCode,
         //   thrawSeqNo: thrawSeqNo,
            preproductName: preproductName
        },
        success: function (data) {
            disableButton();
            $("#editMaterialArea").html(data);
            $('#F02_MaterialCode').focus();
        }
    });
}

function displayMaterialListError(object, message) {
    alert(message);
    object.addClass("error-line");
}

function validateMaterialTable() {
    
    if (gridHelper.getListingData("PreproductMaterialGrid").length < 1) {
        $("#materialListError").text("Please add at least one record.").show();
        return false;
    }
    
   //    •	If [C_Seq] is not “1” on “Material” table, the system will show message as MSG 19 and highlight this record in red.

    if ($.trim($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr:first td:nth-child(5)").children().html()) !== "1") {
        displayMaterialListError($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr:first"), "The first Charge Seq. No. must be 1.");
        return false;
    }
    var firstPSeq = $.trim($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr:first td:nth-child(6)").children().html());
    var firstWSeq = $.trim($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr:first td:nth-child(7)").children().html());
    //if (firstPSeq !== "") {
    //    if (firstWSeq !== "1") {
            
    //        displayMaterialListError($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr:first"), "The first Weigh Seq. No. of a Pot Seq. No. must be 1.");
    //        return false;
    //    }
    //}
    if (firstPSeq === "") {
        if (firstWSeq !== "") {
            displayMaterialListError($(this), "When Pot Seq. No. is blank, the Weigh Seq. No. must be blank.");
            return false;
        }
    }
    var loop = true;
    var lstMaterial = gridHelper.getListingData("PreproductMaterialGrid");
    if ($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr").length > 1) {
        $("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr").slice(1).each(function() {
            //o	From the 2nd record to the last record, for each record:
            var currentCSeq = $.trim($(this).find("td:nth-child(5)").children().html());
            var prevCSeq = $.trim($(this).prev().find("td:nth-child(5)").children().html());
            var currentPSeq = $.trim($(this).find("td:nth-child(6)").children().html());
            var prevPSeq = $.trim($(this).prev().find("td:nth-child(6)").children().html());
            var currentWSeq = $.trim($(this).find("td:nth-child(7)").children().html());
            var prevWSeq = $.trim($(this).prev().find("td:nth-child(7)").children().html());
            var currentCPri = $.trim($(this).find("td:nth-child(4)").children().html());
            var prevCPri = $.trim($(this).prev().find("td:nth-child(4)").children().html());
            //•	If [P_Seq] is blank: if [W_Seq] is not blank, the system will show message as MSG 26.
            //if (currentPSeq === "") {
            //    if (currentWSeq !== "") {
            //        displayMaterialListError($(this), "When Pot Seq. No. is blank, the Weigh Seq. No. must be blank.");
            //        loop = false;
            //        return false;
            //    }
            //}
            //if (currentPSeq !== "") {
            //    if (currentWSeq !== "1") {
            //        displayMaterialListError($(this), "The first Weigh Seq. No. of a Pot Seq. No. must be 1.");
            //        loop = false;
            //        return false;
            //    }
            //}

            if (parseInt(currentCSeq) - parseInt(prevCSeq) !== 1 && parseInt(currentCSeq) - parseInt(prevCSeq) !== 0) {
                displayMaterialListError($(this), "The Charge Seq. No. must be continuous.");
                loop = false;
                return false;
            }


         
            //which have the same [C_Seq]:
            //- If all of their [C_Prj] are not the same, the system will show message as MSG 20.
            //if (currentCSeq === prevCSeq) {
            //    if (currentCPri !== prevCPri) {
            //        displayMaterialListError($(this), "The Charge Seq. No. must be continuous.");
            //        loop = false;
            //        return false;

            //    }
            //}

            //if (currentPSeq === "" && prevPSeq === "") {
            //    if ((parseInt(currentCSeq) - parseInt(prevCSeq)) !== 1) {
            //        displayMaterialListError($(this), "When Pot Seq. No. is blank, the charge seq. no. must be unique and continuous.");

            //        loop = false;
            //        return false;

            //    }
            //}
            //if (currentPSeq !== "" && prevPSeq !== "" && currentPSeq === prevPSeq) {
            //    if ((parseInt(currentCSeq) - parseInt(prevCSeq)) !== 0) {
            //        displayMaterialListError($(this), "When Pot Seq. No. is equal, the charge seq. no. must be equal.");
            //        loop = false;
            //        return false;

            //    }
            //    if ((parseInt(currentWSeq) - parseInt(prevWSeq)) !== 1) {
            //        displayMaterialListError($(this), "The Weigh Seq. No. must be continuous.");
            //        loop = false;
            //        return false;

            //    }

            //}

            //if (currentWSeq !== ""  && currentPSeq !== prevPSeq) {

            //    if (currentWSeq !== "1") {
            //        displayMaterialListError($(this), "The first Weigh Seq. No. of a Pot Seq. No. must be 1.");
            //        loop = false;
            //        return false;

            //    }
            //}
        });
    };
    if (loop === true) {
        for (var i = 0; i < lstMaterial.length; i++) {
            var flag = false;
            var ls_PotSeqNo = lstMaterial[i].F02_PotSeqNo;
            for (var j = i + 1; j < lstMaterial.length; j++) {
                var ls_NewPotSeqNo = lstMaterial[j].F02_PotSeqNo;
                if ((ls_PotSeqNo !== null && $.trim(ls_PotSeqNo !== "")) && (parseInt(ls_PotSeqNo) === parseInt(ls_NewPotSeqNo))) {
                    if (flag) {
                        displayMaterialListError($("#PreproductMaterialGrid .jsgrid-grid-body .jsgrid-table tbody tr").eq(j), "When Charge Seq. No. is not equal, the Pot Seq. no. cannot be equal.");
                        loop = false;
                        return false;
                    }

                } else {
                    flag = true;
                }
            }
        }
    }
    return loop;
}

function OnChangePreproductName() {
    $("#txtPreProductName").val($("#F03_PreProductName").val());
}
function OnChangePreproductCode() {
    $("#F02_PreProductCode").val($("#F03_PreProductCode").val());
}