
$(document).ready(function () {
    $("#btnDelete").prop("disabled", true);
    $("#btnUpdate").prop("disabled", true);
    // Handle click on checkbox to set state of "Select all" control Normal    
    $('#NormalGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnOKProdCer').prop("disabled", false) : $('#btnOKProdCer').prop("disabled", true);
        counterChecked > 0 ? $('#btnNGProdCer').prop("disabled", false) : $('#btnNGProdCer').prop("disabled", true);
    });
    //
    $('#OutOfPlanGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnOKProdCer').prop("disabled", false) : $('#btnOKProdCer').prop("disabled", true);
        counterChecked > 0 ? $('#btnNGProdCer').prop("disabled", false) : $('#btnNGProdCer').prop("disabled", true);
    });
    //
    $('#SampleGrid').on('change', 'input[type="radio"]', function () {
        // If checkbox is not checked
        var counterChecked = 0;
        this.checked ? counterChecked++ : counterChecked--;
        counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
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

    $('#YearMonth').change(function () {
        var status = $('input[name=StorageOfProductStatus]:checked').val();
        var yearmonth = $('#YearMonth').val();
        var model = {
            StorageOfProductStatus: status,
            YearMonth: yearmonth
        }
        
        if (status === "0") {
            $('#dvNormal').show();
            $('#dvOutOfPlan').hide();
            $('#dvSample').hide();

            gridHelper.searchObject("NormalGrid", model);
            $('#btnAdd').prop('disabled', true);
            $('#btnUpdate').prop('disabled', true);
            $('#btnDelete').prop('disabled', true);
            if (!$('#NormalGrid').empty) {
                $('#btnOKProdCer').prop('disabled', false);
                $('#btnNGProdCer').prop('disabled', false);
            }

        }
        if (status === "1") {
            $('#dvOutOfPlan').show();
            $('#dvNormal').hide();
            $('#dvSample').hide();

            gridHelper.searchObject("OutOfPlanGrid", model);
            $('#btnAdd').prop('disabled', true);
            $('#btnUpdate').prop('disabled', true);
            $('#btnDelete').prop('disabled', true);
            if (!$('#OutOfPlanGrid').empty) {
                $('#btnOKProdCer').prop('disabled', false);
                $('#btnNGProdCer').prop('disabled', false);
            }
        }
        if (status === "2") {
            $('#dvSample').show();
            $('#dvNormal').hide();
            $('#dvOutOfPlan').hide();

            gridHelper.searchObject("SampleGrid", model);
            $('#btnOKProdCer').prop('disabled', true);
            $('#btnNGProdCer').prop('disabled', true);
            $('#btnAdd').prop('disabled', false);
            if (!$('#SampleGrid').empty) {
                $('#btnUpdate').prop('disabled', false);
                $('#btnDelete').prop('disabled', false);
            }
        }
    });
   
});

function addNewProdCer() {    
    $.ajax({
        //url: formUrl.urlEdit,
        url: "/ProductCertification/Edit",
        type: 'GET',
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F67_ProductCode').focus();
            //  $('#F39_KndEptBgnDate').val('');
            disableButton();
        }
    });
}

function edit() {

    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var gridid = 'NormalGrid';    
    if (status === "1") {
        gridid = 'OutOfPlanGrid';
    }
    if (status === "2") {
        gridid = 'SampleGrid';
    }

    
    var date = gridHelper.displayDate($('#' + gridid).find('input:checked').val());    
    var data = gridHelper.getSelectedItem(gridid);
    $.ajax({
        //url: formUrlProdCer.urlEdit,
        url: "/ProductCertification/Edit",
        type: 'GET',
        data: {
            //date: date,
            prodCode: data.F67_ProductCode,
            prePdtLotNo: data.F67_PrePdtLotNo,
            productFlg: data.F67_ProductFlg
        },
        success: function (data) {
            disableButton();
            $("#formPlaceholder").html(data);
            $('#F67_ProductCode').focus();
            
        }
    });
}

function deleteItemProdCer() {

    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var gridid = 'NormalGrid';
    if (status === "1") {
        gridid = 'OutOfPlanGrid';
    }
    if (status === "2") {
        gridid = 'SampleGrid';
    }

    
    var date = gridHelper.displayDate($('#' + gridid).find('input:checked').val());
    //var data = gridHelper.getSelectedItem(gridid).F67_ProductCode;
    var data = gridHelper.getSelectedItem(gridid);
    if (confirm("Ready to delete?")) {
        $.ajax({
            //url: formUrlPdtPln.urlDelete,
            url: "/ProductCertification/Delete",
            type: 'POST',
            data: {                
                prodCode: data.F67_ProductCode,
                prePdtLotNo: data.F67_PrePdtLotNo,
                productFlg: data.F67_ProductFlg
            },
            success: function (data) {
                showMessage(data, gridid, "");
                //disableButton();
                $("#btnDelete").prop("disabled", true);
                $("#btnUpdate").prop("disabled", true);
            }
        });
    };
}

function hideFormProdCer() {
    $("#formPlaceholder").html("");
    enableButton("PdtPlnGrid");
}

function searchProdCer() {
    tablePdtPln.search($("#txtF39_KndEptBgnDateSearch").val().trim()).draw();
}

function search() {
    var request = {
        YearMonth: $("#YearMonth").val(),
        KndLine: $("#KndLine").val()
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
    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var gridid = 'NormalGrid';
    if (status === "1") {
        gridid = 'OutOfPlanGrid';
    }
    if (status === "2") {
        gridid = 'SampleGrid';
    }

    var request = {
        MaterialCode: $("#txtCodeSearch").val()
    };
    showMessage(data, gridid, request);    
    enableButton(gridid);
    //$('#selectedValue').val(null);
    //var request = {
    //    selectedValue: null
    //}

    //gridHelper.searchObject(gridid, request);
    $('#btnUpdate').attr("disabled", "disabled");
    $('#btnDelete').attr("disabled", "disabled");
    $("#formPlaceholder").html("");

}


/**
 * Find the selected product certification on the screen.
 * @returns {} 
 */
findSelectedProductCertification = function(name) {
    return $("input[type='radio'][name='" + name + "']:checked").val();
}

/**
 * Find the selected certification date on the screen.
 * @returns {} 
 */
findCertificationDate = function() {
    //return $('#dvYearMonth').data('DateTimePicker').date();
    return $("#YearMonth").val();
}
/**
 * Select a product and tell service to make its state be ok.
 * @returns {} 
 */
makeProductOk = function () {
    
    var certificationDate = findCertificationDate();
    if (certificationDate === '') {
        $("#errorlist").text("Please input data for this required field.").show();
        return;
    }

    if (!confirm('Do you make sure this product is' + ' "Certification OK"?'))
        return;

    // Find the selected item 
    //var item = findProductCertificationItem();
    //if (item == null)
    //    return;
    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var gridid = 'NormalGrid';
    if (status === "1") {
        gridid = 'OutOfPlanGrid';
    }
    if (status === "2") {
        gridid = 'SampleGrid';
    }
    var data = gridHelper.getSelectedItem(gridid);
    $("#errorlist").hide();
    if (gridid === 'NormalGrid') {                
        $.ajax({
            url: "/ProductCertification/MakeProductCertificationOk",
            type: "post",
            data: {
                //certificationFlag: data.F56_CertificationFlag,
                certificationFlag: status,
                productCode: data.F56_ProductCode,
                prePdtLotNo: data.F56_PrePdtLotNo,
                productLotNo: data.F56_ProductLotNo,
                quantity: data.F56_TbtCmdEndAmt,
                certificationDate: certificationDate,
                status: status
            },
            success: function (data) {
                if (!data.Success) {                    
                    var a = data.Message;
                    showMessage(data, gridid, "");
                    return false;
                }

                // Reload the screen.
                gridHelper.searchObject(gridid, null);
                //showMessage(data, gridid, "");
                $("#YearMonth").val("");                
                $('#btnOKProdCer').prop('disabled', true);
                $('#btnNGProdCer').prop('disabled', true);
                //window.location.reload();
            }
        });
    } else {
        // Find the selected item 
        //var data = gridHelper.getSelectedItem(gridid);
        //var certificationDate = findCertificationDate();
        $.ajax({
            url: "/ProductCertification/MakeProductCertificationOk",
            type: "POST",
            data: {
                //certificationFlag: findSelectedProductCertification(),
                certificationFlag: data.F58_CertificationFlag,
                productCode: data.F58_ProductCode,
                prePdtLotNo: data.F58_PrePdtLotNo,
                productLotNo: data.F58_ProductLotNo,
                quantity: data.F58_TbtCmdEndAmt,
                certificationDate: certificationDate,
                status: status
            },
            success: function (data) {
                if (!data.Success) {
                    var a = data.Message;
                    showMessage(data, gridid, "");
                    return false;
                }
                // Reload the screen.

                gridHelper.searchObject(gridid, null);
                //showMessage(data, gridid, "");
                $("#YearMonth").val("");
                $('#btnOKProdCer').prop('disabled', true);
                $('#btnNGProdCer').prop('disabled', true);
                //window.location.reload();               
            }
        });
    }
    
}

/**
 * Select a product and tell service to make its state be ng.
 * @returns {} 
 */
makeProductNg = function () {
    var certificationDate = findCertificationDate();
    
    if (certificationDate === '') {
        $("#errorlist").text("Please input data for this required field.").show();
        return;
    }
    if (!confirm('Do you make sure this product is' + ' "Certification NG"?'))
        return;

    var status = $('input[name=StorageOfProductStatus]:checked').val();                
    var gridid = 'NormalGrid';
    if (status === "1") {
        gridid = 'OutOfPlanGrid';
    }
    if (status === "2") {
        gridid = 'SampleGrid';
    }
    
    var data = gridHelper.getSelectedItem(gridid);
    $("#errorlist").hide();
    if (gridid === 'NormalGrid') {
        // Find the selected item                 
        $.ajax({
            url: "/ProductCertification/MakeProductCertificationNg",
            type: "POST",
            data: {
                //certificationFlag: findSelectedProductCertification(),
                //certificationFlag: data.F56_CertificationFlag,
                certificationFlag: status,
                productCode: data.F56_ProductCode,
                prePdtLotNo: data.F56_PrePdtLotNo,
                productLotNo: data.F56_ProductLotNo,
                quantity: data.F56_TbtCmdEndAmt,
                certificationDate: certificationDate,
                status : status
    },
            success: function (data) {
                if (!data.Success) {
                    var a = data.Message;
                    showMessage(data, gridid, "");
                    return false;
                }

                // Reload the screen.
                gridHelper.searchObject(gridid, null);
                //showMessage(data, gridid, "");
                $("#YearMonth").val("");
                $('#btnOKProdCer').prop('disabled', true);
                $('#btnNGProdCer').prop('disabled', true);
                //window.location.reload();
            }
        });
    } else {
        // Find the selected item 
        //var data = gridHelper.getSelectedItem(gridid);
        //var certificationDate = findCertificationDate();
        $.ajax({
            url: "/ProductCertification/MakeProductCertificationNg",
            type: "POST",
            data: {
                //certificationFlag: findSelectedProductCertification(),
                certificationFlag: data.F58_CertificationFlag,
                productCode: data.F58_ProductCode,
                prePdtLotNo: data.F58_PrePdtLotNo,
                productLotNo: data.F58_ProductLotNo,
                quantity: data.F58_TbtCmdEndAmt,
                certificationDate: certificationDate,
                status: status
            },
            success: function (data) {
                if (!data.Success) {
                    var a = data.Message;
                    showMessage(data, gridid, "");
                    return false;
                }

                // Reload the screen.
                gridHelper.searchObject(gridid, null);
                //showMessage(data, gridid, "");
                $("#YearMonth").val("");
                $('#btnOKProdCer').prop('disabled', true);
                $('#btnNGProdCer').prop('disabled', true);
                //window.location.reload();
            }
        });
    }
    
}


/**
 * Change grid.
 * @returns {} 
 */
function OnChangeGrid() {
    //$("#YearMonth").val("");
    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var yearmonth = $('#YearMonth').val();
    var model = {
        StorageOfProductStatus: status,
        YearMonth: yearmonth
    }
    
       
    if (status === "0") {
        $('#dvNormal').show();
        $('#dvOutOfPlan').hide();
        $('#dvSample').hide();


        if ($('#formPlaceholder').html() != "") {
            if (confirm("Unsaved data will be lost. Are you sure to change?")) {
                $("#formPlaceholder").html("");
                gridHelper.searchObject("NormalGrid", model);
            }
        } else {
            gridHelper.searchObject("NormalGrid", model);
        }


        //gridHelper.searchObject("NormalGrid", model);
        $('#btnAdd').prop('disabled', true);
        $('#btnUpdate').prop('disabled', true);
        $('#btnDelete').prop('disabled', true);
        
        
        $('#btnOKProdCer').prop('disabled', true);
        $('#btnNGProdCer').prop('disabled', true);
        //if ($("#NormalGrid .jsgrid-nodata-row").text() === '') {
        //    $('#btnOKProdCer').prop('disabled', false);
        //    $('#btnNGProdCer').prop('disabled', false);
        //} else {
        //    $('#btnOKProdCer').prop('disabled', true);
        //    $('#btnNGProdCer').prop('disabled', true);
        //}

    }
    if (status === "1") {
        $('#dvOutOfPlan').show();
        $('#dvNormal').hide();
        $('#dvSample').hide();

        if ($('#formPlaceholder').html() != "") {
            if (confirm("Unsaved data will be lost. Are you sure to change?")) {
                $("#formPlaceholder").html("");
                gridHelper.searchObject("OutOfPlanGrid", model);
            }
        } else {
            gridHelper.searchObject("OutOfPlanGrid", model);
        }

        $('#btnAdd').prop('disabled', true);
        $('#btnUpdate').prop('disabled', true);
        $('#btnDelete').prop('disabled', true);
        
        $('#btnOKProdCer').prop('disabled', true);
        $('#btnNGProdCer').prop('disabled', true);
        //if ($("#OutOfPlanGrid .jsgrid-nodata-row").text() === "") {
        //        $('#btnOKProdCer').prop('disabled', false);
        //        $('#btnNGProdCer').prop('disabled', false);
        //    } else {
        //        $('#btnOKProdCer').prop('disabled', true);
        //        $('#btnNGProdCer').prop('disabled', true);
        //    }
        //}  
    }
    if (status === "2") {
        $('#dvSample').show();
        $('#dvNormal').hide();
        $('#dvOutOfPlan').hide();

        if ($('#formPlaceholder').html() != "") {
            if (confirm("Unsaved data will be lost. Are you sure to change?")) {
                $("#formPlaceholder").html("");
                gridHelper.searchObject("SampleGrid", model);
            }
        } else {
            gridHelper.searchObject("SampleGrid", model);
        }


        //gridHelper.searchObject("SampleGrid", model);
        $('#btnOKProdCer').prop('disabled', true);
        $('#btnNGProdCer').prop('disabled', true);
        $('#btnAdd').prop('disabled', false);
        if (!$('#SampleGrid').empty) {
            $('#btnUpdate').prop('disabled', false);
            $('#btnDelete').prop('disabled', false);
        }
    }
}


/**
 * Change certification date.
 * @returns {} 
 */
function OnChangeYearMonth() {

    var status = $('input[name=StorageOfProductStatus]:checked').val();
    var yearmonth = $('#YearMonth').val();
    var model = {
        StorageOfProductStatus: status,
        YearMonth: yearmonth
    }
    
    if (status === "0") {
        $('#dvNormal').show();
        $('#dvOutOfPlan').hide();
        $('#dvSample').hide();

        gridHelper.searchObject("NormalGrid", model);
        $('#btnAdd').prop('disabled', true);
        $('#btnUpdate').prop('disabled', true);
        $('#btnDelete').prop('disabled', true);
        if (!$('#NormalGrid').empty) {
            $('#btnOKProdCer').prop('disabled', false);
            $('#btnNGProdCer').prop('disabled', false);
        }

    }
    if (status === "1") {
        $('#dvOutOfPlan').show();
        $('#dvNormal').hide();
        $('#dvSample').hide();

        gridHelper.searchObject("OutOfPlanGrid", model);
        $('#btnAdd').prop('disabled', true);
        $('#btnUpdate').prop('disabled', true);
        $('#btnDelete').prop('disabled', true);
        if (!$('#OutOfPlanGrid').empty) {
            $('#btnOKProdCer').prop('disabled', false);
            $('#btnNGProdCer').prop('disabled', false);
        }
    }
    if (status === "2") {
        $('#dvSample').show();
        $('#dvNormal').hide();
        $('#dvOutOfPlan').hide();

        gridHelper.searchObject("SampleGrid", model);
        $('#btnOKProdCer').prop('disabled', true);
        $('#btnNGProdCer').prop('disabled', true);
        $('#btnAdd').prop('disabled', false);
        if (!$('#SampleGrid').empty) {
            $('#btnUpdate').prop('disabled', false);
            $('#btnDelete').prop('disabled', false);
        }
    }
}

/**
 * Find the selected product certification item 
 * @returns {} 
 */
findProductCertificationItem = function() {
    var gridType = $("input[name='StorageOfProductStatus']:checked").val();
    if (gridType == 0)
        return gridHelper.findSelectedRadioItem("NormalGrid");
    else if (gridType == 1)
        return gridHelper.findSelectedRadioItem("OutOfPlanGrid");
    else if (gridType == 2)
        return gridHelper.findSelectedRadioItem("SampleGrid");

    return null;
}

/**
 * Find product certification mode selection.
 * @returns {} 
 */
findProductCertificationMode = function() {
    return $("input[type='radio'][name='StorageOfProductStatus']:checked").val();
    //var status = $('input[name=PrintOptions]:checked').val();
}

/**
 * Base on mode to select search conditions.
 * @returns {} 
 */
findGridSearchCondition = function() {
    var mode = findProductCertificationMode();
    if (mode == 0)
        return gridHelper.getSearchCondition("NormalGrid");
    if (mode == 2)
        return gridHelper.getSearchCondition("OutOfPlanGrid");
    return gridHelper.getSearchCondition("SampleGrid");
}
/**
 * This function is for exporting the product certification items.
 * @returns {} 
 */
exportProductCertification = function() {
    
    var startDate = $("#StartDate").val();
    var endDate = $("#EndDate").val();
    if (findDatePickerValue($("#dvStartDate")) > findDatePickerValue($("#dvEndDate"))) {
        //$("#DatetimeError").text('Start date is later than end date!').show();
        //var errorMessage = { Success: false, Message: "Start date is later than end date!" }
        //showMessage(errorMessage, "", "");
        $("#DatetimeError").text("Start date is later than end date!").show();
        return;
    }  

    // Display confirmation dialog.
    if (!confirm("Ready to print?")) {
        return;
    };
    $("#DatetimeError").hide();
    
    //var status= findProductCertificationMode(); 
    var status = $('input[name=PrintProductCertificationStatus]:checked').val();
    //var settings = findGridSearchCondition();
    
    $.ajax({
        url: "/ProductCertification/ExportDetail",
        type: "post",
        data: {
            printProductCertificationStatus: status,
            //mode:status,
            //settings: settings,
            startDate: startDate,
            endDate: endDate
        },
        
        success: function (response) {
            if (response.Success === false) {
                var message = response.Message;
                var errorMessage = { Success: false, Message: message }
                showMessage(errorMessage, "", "");
            }
            var render = response.render;

            if (render != null) {

                $("#productCertificationPrintArea")
                    .html(render)
                    .show()
                    .print()
                    .empty()
                    .hide();
            }
        }
    });

}

function callExport() {      
    $.ajax({
        //url: formUrl.urlEdit,
        url: "/ProductCertification/Export",
        type: 'GET',
        success: function (data) {
            $("#formPlaceholder").html(data);
            $('#F67_ProductCode').focus();
            //  $('#F39_KndEptBgnDate').val('');
            //disableButton();
        }
    });
}

findDatePickerValue = function (datepicker) {
    return datepicker.data("DateTimePicker").date().toDate();
}
