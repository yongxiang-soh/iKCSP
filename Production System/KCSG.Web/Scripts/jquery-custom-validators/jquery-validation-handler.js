/**
 * This function is for analyzing jquery validation messages.
 * When validation error messages are detected, this function will query to DOM and render related error message defined by jquery-validation.unobtrusive.js
 * @returns {} 
 */
handleJQueryValidationMessages = function (responseJson) {

    // Find fields have validation errors from responded json data.
    var keys = Object.keys(responseJson);

    // Go through every key to render its validation error messages.
    for (var i = 0; i < keys.length; i++) {
        var key = keys[i];

        // 
        for (var j = 0; j < responseJson[key].length; j++) {
            var validationMessage = responseJson[key][j];

            // Input fields should be rendered with red border.
            $('span[data-valmsg-for="' + key + '"]')
                .removeClass("field-validation-valid")
                .addClass("field-validation-error")
                .text(validationMessage);

            // Validation message notification should be displayed.
            $('input[name="' + key + '"]')
                .removeClass("field-validation-valid")
                .addClass("field-validation-error");

            break;
        }
    }
}