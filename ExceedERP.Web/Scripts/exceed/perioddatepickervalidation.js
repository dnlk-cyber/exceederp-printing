//not applied
function onSelectedPeriodById(period, date) {
    alert("#" + period);
    if ($("#" + period).data("kendoDropDownList")) {
        console.log(period, date);
        var periodName = $("#" + period).data("kendoDropDownList").value();
        var min = "";
        var max = "";

        $.ajax({
            type: "POST",
            url: "/Inventory/DataProvider/GetPeriodDateRangeById/",
            data: { id: periodName },
            success: function (result) {
                min = result.Min;
                max = result.Max;
                //update
                var datepicker = $("#" + date).data("kendoDatePicker");
                datepicker.setOptions({
                    format: "yyyy-MM-dd",
                    min: min,
                    max: max,
                    value: new Date(),
                    change: onDOBChange
                });
            }, //end succ
            error: function (result) {
                alert(result);
            }

        });
    }
    


}

function onDOBChange(e) {
    var dt = e.sender;
    var value = dt.value();

    if (value === null) {
        value = kendo.parseDate(dt.element.val(), dt.options.parseFormats);
    }

    if (value < dt.min()) {
        dt.value(dt.min());
    } else if (value > dt.max()) {
        dt.value(dt.max());
    }
}