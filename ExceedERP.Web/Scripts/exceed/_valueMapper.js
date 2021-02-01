function valueMapper(options) {
    $.ajax({
        url: "/HRM/HRMDataProvider/Employee_ValueMapper/",
        data: {
            value: options.value || 0,
            status: 1,
            excludeId: -1
        },
        success: function (data) {
            options.success(data);
        }
    });
}

function convertValues(value) {
    var data = {};

    value = $.isArray(value) ? value : [value];

    for (var idx = 0; idx < value.length; idx++) {
        data["values[" + idx + "]"] = value[idx];
    }

    return data;
}