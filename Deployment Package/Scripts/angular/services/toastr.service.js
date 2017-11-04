angular.module("ToastrNotification", [])
    .service("toastrService",
        function () {

            // This function is for initialize toastr notification with settings.
            this.initializeToastr = function (closeButton, debug, newestOnTop,
                progressBar, positionClass, preventDuplicate,
                onclick, showDuration, hideDuration, timeOut,
                extendedTimeout, showEasing, hideEasing, showMethod, hideMethod) {

                toastr.options = {
                    "closeButton": closeButton,
                    "debug": debug,
                    "newestOnTop": newestOnTop,
                    "progressBar": progressBar,
                    "positionClass": positionClass,
                    "preventDuplicates": preventDuplicate,
                    "onclick": onclick,
                    "showDuration": showDuration,
                    "hideDuration": hideDuration,
                    "timeOut": timeOut,
                    "extendedTimeOut": extendedTimeout,
                    "showEasing": showEasing,
                    "hideEasing": hideEasing,
                    "showMethod": showMethod,
                    "hideMethod": hideMethod
                }
            }

            // Initialize toast with a collection of options.
            this.initializeToastr = function(options) {
                toastr.options = options;
            }

            // Show validation error messages with toastr options.
            // As options are null, default one will be initialized.
            this.initializeToastrValidationMessages = function (validationMessages, options) {

                if (options == null) {
                    // This is the default 
                    toastr.options = {
                        "closeButton": true,
                        "debug": false,
                        "newestOnTop": false,
                        "progressBar": false,
                        "positionClass": "toast-top-full-width",
                        "preventDuplicates": true,
                        "onclick": null,
                        "showDuration": "0",
                        "hideDuration": "0",
                        "timeOut": "0",
                        "extendedTimeOut": "0",
                        "showEasing": "swing",
                        "hideEasing": "linear",
                        "showMethod": "fadeIn",
                        "hideMethod": "fadeOut"
                    };
                } else {
                    toastr.options = options;
                }

                var innerHtml = '';
                innerHtml += '<ul>';

                // Find the keys in the validation message.
                var keys = Object.keys(validationMessages);
                for (var i = 0; i < keys.length; i++) {
                    var key = keys[i];
                    innerHtml += '<li>' + key + '</li>';

                    innerHtml += '<ul>'
                    for (var v = 0; v < validationMessages[key].length; v++) {
                        innerHtml += '<li>' + validationMessages[key][v] + '</li>';
                    }
                    innerHtml += '</ul>';

                }
                
                innerHtml += '</ul>';
                
                // Call toastr command to display list of error.
                toastr["error"](innerHtml);
            }
        });