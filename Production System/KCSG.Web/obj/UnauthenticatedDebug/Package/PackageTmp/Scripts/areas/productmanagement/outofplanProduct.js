var OutOfPlanProduct = function() {
    var checkvalidation = function() {
        var checked = true;
        if ($('#txtProductCodeSearch').val() === "") {
            $("#errorProductCode").text(message.MSG01).show();
            checked = false;
        }
        if (checked === false)
            return false;
        return true;
    };
    return {
        init: function() {
            $("#btnDelete").prop("disabled", true);
            $("#btnUpdate").prop("disabled", true);

            // Handle click on checkbox to set state of "Select all" control
            $('#OutOfPlanProductGrid').on('change', 'input[type="radio"]', function() {
                // If checkbox is not checked
                var counterChecked = 0;
                this.checked ? counterChecked++ : counterChecked--;
                counterChecked > 0 ? $('#btnDelete').prop("disabled", false) : $('#btnDelete').prop("disabled", true);
                counterChecked > 0 ? $('#btnUpdate').prop("disabled", false) : $('#btnUpdate').prop("disabled", true);
            });
            $("#txtProductCodeSearch").focus();
            $("#searchForm").keypress(function(e) {
                if (e.keyCode === 13) {
                    OutOfPlanProduct.onSearch();
                    return false;
                }
            });
            $('#txtProductCodeSearch').keyup(function() {
                var hasValue = this.value.length > 0;
                $("#errorProductCode").text("");
            });

            $('body').on("submit", "#editForm", function(e) {
                var mess = message.MSG28;
                if ($('#editForm #IsCreate').val() == "True") {
                    mess = message.MSG22;
                }
                if (confirm(mess)) {
                    return true;
                } else return false;
                //e.preventDefault();
            });
        },
        hideForm: function() {
            $("#formPlaceholder").html("");
            enableButton("OutOfPlanProductGrid");
        },
        onSearch: function() {
            //if (!checkvalidation())
            //    return;
            var request = {
                productCode: $("#txtProductCodeSearch").val().trim()
            };
            gridHelper.searchObject("OutOfPlanProductGrid", request);
        },
        addNew: function() {
            $.ajax({
                url: formUrl.urlEdit,
                type: 'GET',
                data: { productCode: null },
                success: function(data) {
                    $("#formPlaceholder").html(data);
                    disableButton();
                    $('#PrePdtLotNo').focus();
                }
            });
        },
        deleteItem: function() {
            var productcode = gridHelper.getSelectedItem("OutOfPlanProductGrid").F58_ProductCode;
            var prepdtlotno = gridHelper.getSelectedItem("OutOfPlanProductGrid").F58_PrePdtLotNo;
            if (confirm("Ready to delete?")) {
                $.ajax({
                    url: formUrl.urlDelete,
                    type: 'POST',
                    data: {
                        productcode: productcode,
                        prepdtlotno: prepdtlotno
                    },
                    success: function(data) {
                        showMessage(data, "OutOfPlanProductGrid", "");
                        $("#btnDelete").prop("disabled", true);
                        $("#btnUpdate").prop("disabled", true);
                    }
                });
            };
        },
        updateItem: function() {
            var productcode = gridHelper.getSelectedItem("OutOfPlanProductGrid").F58_ProductCode;
            var prePdtlotNo = gridHelper.getSelectedItem("OutOfPlanProductGrid").F58_PrePdtLotNo;
            $.ajax({
                url: formUrl.urlEdit,
                type: 'GET',
                data: {
                    productCode: productcode,
                    prePdtLotNo: prePdtlotNo
                },
                success: function(data) {
                    $("#formPlaceholder").html(data);
                    $('#PrePdtLotNo').focus();

                }
            });
        },
        onSuccess: function(data, status, xhr) {
            showMessage(data, "OutOfPlanProductGrid", "");
            enableButton("OutOfPlanProductGrid");
            $("#btnDelete").prop("disabled", true);
            $("#btnUpdate").prop("disabled", true);
            if (data.Success) {
                $("#formPlaceholder").html("");
            }
        }
    };
}();
OutOfPlanProduct.init();