/**
 * Callback which is fired when ajax request is sent to webserver.
 */
$(document).ajaxSend(function (evt, request, settings) {
    if (settings.url.indexOf('LockScreen') != -1)
        return;

    if (settings.type == "POST"||settings.type == "GET") {
        blockScreenAccess('Please wait ...', settings);
    }
});

$(document).ajaxStop(function () {
  //$.unblockUI();
});

$(document).ajaxError(function (xhr, props) {
    $.unblockUI();
    if (props.status === 401) {
        location.reload();
    }
});

$(document).ajaxSuccess(function (event, request, settings) {
    if (request.getResponseHeader('REQUIRES_AUTH') === '1') {
        window.location.href = request.getResponseHeader('LOGIN_URL') + "?isSessionTimeout=true";
    }
   
    if (request.responseJSON != null &&request.responseJSON.Success != undefined) {
        //// $.growlUI('Growl Notification', 'Have a nice day!');
        //$('#Message').find('label').text(request.responseJSON.Message);
        //$.blockUI({
        //    message: $('#Message'),
        //    timeout: 2000,
        //    onUnblock: function () { $(".jsgrid").jsGrid("search"); },
        //    css: {
        //        width: '350px',
        //        height: '80px',
                
        //    }
        //});
        //$('.blockOverlay').attr('title', 'Click to unblock').click($.unblockUI);
    } else {
        if (settings.async) {

            var lowerCased = settings.url.toLowerCase();
            if (lowerCased == null)
                return;

            if (lowerCased.indexOf('/lockscreen') !== -1)
                return;

            $.unblockUI();
        }
    }
});
function showNormanMessageAndReloadUc789(response) {
    if (response != null) {
        if (Boolean(response.Success)) {
            $('#lblMessage').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvSucssesfull"),
                timeout: 3000,
                css: {
                    width: '350px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },
                onUnblock: function() {
                    var location = window.location.pathname;
                    if (location === "/Inquiry/InquiryByShelfStatus") {
                        var searchCondition = $('input[name=SearchCondition]:checked').val();
                        var executingClassification = $('input[name=ExecutingClassification]:checked').val();
                        var type = $('#Type').val();
                        $.ajax({
                            url: "/Inquiry/InquiryByShelfStatus/ShowButonCommand",
                            type: "POST",
                            data: {
                                searchCondition: searchCondition,
                                executingClassification: executingClassification,
                                type: type
                            },
                            success: function (data1) {
                                $("#formPlaceholder").html(data1);
                            }
                        });
                    }
                }

            });
        } else {
            $('#lblMessageError').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvError"),
                timeout: 3000,
                css: {
                    width: '350px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },

            });
        }
    }
}

function showNormanMessageCustom(response,controlId) {
    if (response != null) {
        if (Boolean(response.Success)) {
            $('#lblMessage').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvSucssesfull"),
                timeout: 3000,
                css: {
                    width: '350px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },
                onUnblock: function () {
                    gridHelper.ReloadGrid(controlId);

                }
            });
        } else {
            $('#lblMessageError').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvError"),
                timeout: 3000,
                css: {
                    width: '350px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },
                onUnblock: function () {
                    gridHelper.ReloadGrid(controlId);

                }
            });
        }
    }
}

function showNormanMessage(response) {
    
    if (response != null) {
        if (Boolean(response.Success)) {
            $('#lblMessage').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvSucssesfull"),
                timeout: 3000,
                css: {
                    width: '380px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },

            });
        } else {
            $('#lblMessageError').text(response.Message);
            $.blockUI({
                baseZ: 2000,
                message: $("#dvError"),
                timeout: 3000,
                css: {
                    width: '380px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                },
            });
        }
    }
}

function showMessage(response, tableIdReload, request) {
    if (response != null) {
       if (Boolean(response.Success)) {
            $('#lblMessage').text(response.Message);
            $.blockUI({
                baseZ: 5000,
                message: $("#dvSucssesfull"),
                timeout: 3000,
                onUnblock: function () { loadGrid(tableIdReload, request) },
               
                 css: {
                       width: '350px',
                       height: '80px',
                       border: 'none',
                       backgroundColor: 'none',
                       '-webkit-border-radius': '10px',
                       '-moz-border-radius': '10px',
                       opacity:0.8,
                       color: '#fff'
                      
                 }

            });
       } else {
            $('#lblMessageError').text(response.Message);
            $.blockUI({
                baseZ: 5000,
                message: $("#dvError"),
                timeout: 3000,
                onUnblock: function () { loadGrid(tableIdReload, request) },
                css: {
                    width: '350px',
                    height: '80px',
                    border: 'none',
                    backgroundColor: 'none',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: 0.8,
                    color: '#fff'
                  
                }

            });
        }
      
        $('.blockOverlay').attr('title', 'Click to unblock').click($.unblockUI);
    }
}

function loadGrid(tableIdReload, request) {
    if (tableIdReload!=="") {
        if (request == undefined || request == "") {
            gridHelper.ReloadGrid(tableIdReload);
        } else {
            gridHelper.searchObject(tableIdReload, request);
        }
    }
    
}
function Clear(btn) {
    $(btn).parent().parent().find('input[type=text]').each(function () {
        $(this).val("");
     });
 //  $("button[id^='btnSearch']").click();
}

function Clear(btn, gridIdReload) {
    $(btn).parent().parent().find('input[type=text]').each(function() {
        $(this).val("");
    });
    gridHelper.searchObject(gridIdReload, null);
}


function checkSearch() {
        return $('#formPlaceholder').html() != "";
}

function confirmMessageOKButton(){
    return confirm("Are you sure to save all data?");
}

function UPPERCASE(el) {
    $(el).val($(el).val().toUpperCase());
}

function disableButton() {
    
    $("[id^='btnAdd']").prop("disabled", true);
    $("[id^='btnUpdate'").prop("disabled", true);
    $("[id^='btnDelete'").prop("disabled", true);
}

function enableButton(gridId) {
    var id = $("#" + gridId).find('input:checked').val();
    $("[id^='btnAdd']").prop("disabled", false);
    if (id !== undefined) {
        $("[id^='btnUpdate'").prop("disabled", false);
        $("[id^='btnDelete'").prop("disabled", false);
    }
    
}
function leftPad(number, targetLength) {
    var output = number + '';
    while (output.length < targetLength) {
        output = '0' + output;
    }
    return output;
}

/**
 * This function is for preventing users from accessing screen elements by displaying an overlay.
 * @returns {} 
 */
blockScreenAccess = function (screenMessage) {
    var message = '<span>' + screenMessage + '</span>';
    message += '<br />';
    message += '<img src="/Images/loading-Red.gif" />';
    $.blockUI({
        message: message,
        timeout: 0,
        css: {
            'z-index': 5000,
            width: '300px',
            height: '140px',
            border: 'none',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: 0.8,
            left:'40%',
            color: 'red',
            'font-weight':'bold'
        }

    });
}

