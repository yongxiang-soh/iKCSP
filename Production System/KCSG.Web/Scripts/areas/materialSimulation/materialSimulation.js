
$(document).ready(function () {
   $("input[name='SimulationType']").change(function () {
         if ($(this).val() == 0) {
             $("#dvMaterial").show();
             var requet = {
                 pageSize: '5'
             }
             gridHelper.searchObject("MaterialGrid", requet);
             $("#IsPlan").val(false);
         } else {
             $("#dvMaterial").hide();
             $("#IsPlan").val(true);
         }
         $("#formPlaceholder").html("");
       });

    $("#From").focus();
});

function Generate() {
    var id = $('#MaterialGrid').find('input:checked').val();
    if ($("#gerenaFrom").valid()) {
        var type = $('input[name=SimulationType]:checked').val();
        
        if (id == undefined && type == 0) {
            $('#MaterialGrid').addClass("input-validation-error");
            $("#erSelectMaterial").show();
           return;
        }
        $('#MaterialGrid').removeClass("input-validation-error");
        $("#erSelectMaterial").hide();
        var request = $("#gerenaFrom").serialize();
        request = request +  id;
        $.ajax({
            url: formUrl.urlMaterial,
            type: 'POST',
            data: request,
            success: function (data) {
                $("#formPlaceholder").html(data);
            }
        });
    }
}
function addNew(isPlan) {
    if ($("#materialCriteria").valid()) {
       if (isPlan) {
            $.ajax({
                url: formUrl.urlProductPlan,
                type: 'GET',
                success: function (data) {
                    $("#formPlaceholder").html(data);
                }
            });
        } else {
            $.ajax({
                url: formUrl.urlMaterial,
                type: 'GET',
                success: function (data) {
                    $("#formPlaceholder").html(data);
                
                }
            });
        }

    }
}

function onSelectCell(el) {
    
    $(el).parent().parent().find("td").each(function () {
         $(this).removeClass("td-selected");
    });
     $(el).addClass("td-selected");
}
function ShowPopUp() {
  var  date = $(".td-selected").children("input[id='date']").val();
  var code = $(".td-selected").children("input[id='Code']").val();
  var name = $(".td-selected").children("input[id='productName']").val();
  var lot = $(".td-selected").children("input[id='lot']").val();
  var from = $("#From").val();
  var to = $("#To").val();
  var InRetrieval = $("#InventoryUnderRetrieval:checked").val();
  var AMOnly = $("#AcceptedMaterialOnly:checked").val();
  var MaCommands = $("#MaterialUsedInOtherCommands:checked").val();
 if (date != "" && code != "") {
        $.ajax({
            url: formUrl.urlPopUp,
            type: 'GET',
            data: { from: from, to: to, inRetrieval: InRetrieval, acceptedMaterial: AMOnly, mCommand: MaCommands, preProductCode: code,preProductName:name, productDate: date, lot: lot },
            success: function (data) {
                $('#popup').modal('toggle');
                $("#popup .modal-body").html(data);
            }
        });
    }
  
}