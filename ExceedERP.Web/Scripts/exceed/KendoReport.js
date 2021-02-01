
function createSingleSelectEditor(placeholder, options) {
    var dropDownElement = $(placeholder).html('<div ></div>');
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
                filter: "contains",
                serverFiltering: true,
                optionLabel:"All",
                change: onChange,
                //htmlAttributes: "class=customCSS"
               
                
            });

            dropDownList = $(dropDownElement).data("kendoDropDownList");
        }
    };
}

function createMultiSelectEditor(placeholder, options) {
    var multiSelectDropDown = $(placeholder).html('<div ></div>');
    var parameter,
        valueChangedCallback = options.parameterChanged,
        multiSelectDropDownList;

    function onChange() {
        var val = multiSelectDropDownList.value();
        valueChangedCallback(parameter, val);
    }

    return {
        beginEdit: function (param) {

            parameter = param;

            $(multiSelectDropDown).kendoMultiSelect({
                dataTextField: "name",
                dataValueField: "value",
                value: parameter.value,
                dataSource: parameter.availableValues,
                filter: "contains",
                serverFiltering: true,
                placeholder: "Select",
                change: onChange,
                //htmlAttributes: "class=customCSS"


            });

            multiSelectDropDownList = $(multiSelectDropDown).data("kendoMultiSelect");
            multiSelectDropDownList.value("");
            multiSelectDropDownList.trigger("change");
        }
    };
}

