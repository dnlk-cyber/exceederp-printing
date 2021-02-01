function createSingleSelectEditor(placeholder, options) {
    var dropDownElement = $(placeholder).html('<div></div>');
    var parameter,
          valueChangedCallback = options.parameterChanged,
          dropDownList;

    function onChange() {
        var val = dropDownList.value();
        valueChangedCallback(parameter, val);
    }

    return {
        beginEdit: function (param) {

            parameter = param;

            $(dropDownElement).kendoDropDownList({
                dataTextField: "name",
                dataValueField: "value",
                value: parameter.value,
                dataSource: parameter.availableValues,
                change: onChange
            });

            dropDownList = $(dropDownElement).data("kendoDropDownList");
        }
    };
}

///custom

function customMatch(parameter) {
    return Boolean(parameter.availableValues)
        && !parameter.multivalue
        && parameter.type === telerikReportViewer.ParameterTypes.STRING;
}

function createCustomEditor(placeholder, options) {
    var dropDownElement = $(placeholder).html('<div></div>');
    var parameter,
          valueChangedCallback = options.parameterChanged,
          dropDownList;

    function onChange() {
        var val = dropDownList.value();
        valueChangedCallback(parameter, val);
    }

    return {
        beginEdit: function (param) {

            parameter = param;

            $(dropDownElement).kendoDropDownList({
                dataTextField: "name",
                dataValueField: "value",
                value: parameter.value,
                dataSource: parameter.availableValues,
                change: onChange
            });

            dropDownList = $(dropDownElement).data("kendoDropDownList");
        }
    };
}