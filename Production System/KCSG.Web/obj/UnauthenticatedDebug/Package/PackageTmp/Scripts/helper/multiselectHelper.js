multiselectHelper = {
    getData: function(id) {
        var options = $("#" + id + "_to options");
        var items = [];
        for (var index = 0; index < options.length; index++) {
            var text = $(options[i]).text();
            var value = $(options[i]).val();
            items.push({
                Id: value,
                Name: text,
            });
        }
        return items;
    },
    clear: function(id) {
        clearFrom(id);
        clearTo(id);
    },
    clearFrom: function (id) {
       $("#" + id + "_from options").remove();
    },
    clearTo: function (id) {
       $("#" + id + "_to options").remove();
    },
    setSelectionData: function(id, data) {
        clearFrom(id);
    }
}