numberHelper = {
    getData: function (id) {
        return $("#" + id).autoNumeric('get');
    },
    setData: function (id, value) {
        $("#" + id).autoNumeric('set', value);
    },
    updateOption: function (id, option) {
        $("#" + id).autoNumeric('update', option);
    }
}