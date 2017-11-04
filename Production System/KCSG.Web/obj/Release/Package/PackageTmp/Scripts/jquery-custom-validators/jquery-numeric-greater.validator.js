jQuery.validator.addMethod('greater', function(value, element, params) {

    // By default, input should be null.
    var input = null;

    try {
        input = parseFloat(value);
    } catch (exception) {
        
    }

    // No value has been inputed.
    if (input == null)
        return true;

    // Value is smaller or equal than milestone.
    if (input <= params['milestone'])
        return false;

    return true;
}, '');

jQuery.validator.unobtrusive.adapters.add('greater', ['milestone'], function(options) {
    options.rules['greater'] = options.params;
    options.messages['greater'] = options.message;
})