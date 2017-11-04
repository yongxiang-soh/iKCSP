
var val1m_05 = new Array(new Array(2));
val1m_05[0] = new Array(10);
val1m_05[1] = new Array(10);
var val1m_5 = new Array(new Array(2));
val1m_5[0] = new Array(10);
val1m_5[1] = new Array(10);
var val5cm_03 = new Array(new Array(2));
val5cm_03[0] = new Array(10);
val5cm_03[1] = new Array(10);
var val5cm_05 = new Array(new Array(2));
val5cm_05[0] = new Array(10);
val5cm_05[1] = new Array(10);
var val5cm_1 = new Array(new Array(2));
val5cm_1[0] = new Array(10);
val5cm_1[1] = new Array(10);
var val5cm_5 = new Array(new Array(2));
val5cm_5[0] = new Array(10);
val5cm_5[1] = new Array(10);
$().ready(function () {
    $(".number").autoNumeric('init', { aSep: ',', aDec: '.', vMax: '9999999.99', mDec: 2 });
    $("input[type = 'submit']").hide();
    $("#Location").change(function () {
        Clear();
        var conv_txtB = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];
        var mega_txtB = ["K", "L", "M", "N", "O", "P", "Q", "R", "S", "T"];
        var conv_txtS = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j"];
        var mega_txtS = ["k", "l", "m", "n", "o", "p", "q", "r", "s", "t"];
        $("#dw_1m .active").each(function (t) {
            if ($("#Location").val() === "0") {
                $(this).html(conv_txtB[t]);
            } else {
                $(this).html(mega_txtB[t]);
            }
            
        });
        $("#dw_5cm .active").each(function(t) {
            if ($("#Location").val() === "0") {
                $(this).html(conv_txtS[t]);
            } else {
                $(this).html(mega_txtS[t]);
            }
        });
    });
    $("#dw_1m .number").change(function () {
        $(this).removeClass("input-validation-error");
        var row = $(this).attr("row");
        var col = $(this).attr("col");
        var i = 0;
        
        if ($("#Location").val() !== "0") {
            i = 1;
        }
        switch(row) {
            case "1":
                val1m_05[i][col] = $(this).autoNumeric("get");
                break;
            case "2":
                val1m_5[i][col] = $(this).autoNumeric("get");
                    break;
        }
    });
    
    $("#dw_5cm .number").change(function () {
        $(this).removeClass("input-validation-error");
        var row = $(this).attr("row");
        var col = $(this).attr("col");
        var i = 0;
        if ($("#Location").val() !== "0") {
            i = 1;
        } 
        switch(row) {
            case "1":
                val5cm_03[i][col] = $(this).autoNumeric("get");
                break;
            case "2":
                val5cm_05[i][col] = $(this).autoNumeric("get");
                break;
            case "3":
                val5cm_1[i][col] = $(this).autoNumeric("get");
                break;
            case "4":
                val5cm_5[i][col] = $(this).autoNumeric("get");
                    break;
        }
    });
});

function Print() {
    var validate = true;
    $("#dw_1m .number").each(function (i) {
       if ($(this).autoNumeric("get") == "") {
            $(this).addClass("input-validation-error");
            validate = false;
             
        }
    });
    $("#dw_5cm .number").each(function (i) {
        if ($(this).autoNumeric("get") == "") {
            validate = false;
            $(this).addClass("input-validation-error");
        }
    });
    if (validate) {
        var date = $("#StartDate").val();
        $.ajax({
            url: "/EnvironmentManagement/CleanlinessDataInput/Print",
            type: 'Post',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ val1m05: val1m_05, val1m5: val1m_5, val5cm03: val5cm_03, val5cm05: val5cm_05, val5cm1: val5cm_1, val5cm5: val5cm_5, inputDate: date }),
            success: function (respone) {

                $("#printAreaInput")
                       .html(respone)
                       .show()
                       .print()
                       .empty()
                       .hide();
            }, error: function (xhr, textStatus, errorThrown) {
                $("#printAreaInput")
                       .html(xhr.responseText)
                       .show()
                       .print()
                       .empty()
                       .hide();
            }
        });
    }
    
}

function Clear() {
    $(".number").autoNumeric("set", "");
}