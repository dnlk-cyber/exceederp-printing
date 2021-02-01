function error_handler(gridName) {
    return function (args) {
        if (args.errors) {

            var grid = $("#" + gridName).data("kendoGrid");
            grid.one("dataBinding", function(e) {
                e.preventDefault(); // cancel grid rebind if error occurs

                for (var error in args.errors) {
                    alert(args.errors[error].errors[0]);
                    //showMessage(grid.editable.element, error, args.errors[error].errors);
                }
            });
        }
       
    }
}
function onRequestEnd(gridName) {
    return function (args) {
        var grid = $("#" + gridName).data("kendoGrid");
        var data = grid.dataSource;
        if (args.type === "create" || args.type === "update") {
            if (!args.response.Errors) {
                data.read();
                $(".k-edit-form-container .k-grid-update").removeClass("k-state-disabled");
                $(".k-edit-form-container .k-grid-cancel").removeClass("k-state-disabled");
               $(".k-grid .k-grid-update").removeClass("k-state-disabled");
                $(".k-grid .k-grid-edit").removeClass("k-state-disabled");
                $(".k-grid .k-grid-delete").removeClass("k-state-disabled");
                $(".k-grid .k-grid-cancel").removeClass("k-state-disabled");
                //$(".k-grid .k-state-disabled").addClass("k-grid-add").removeClass("k-state-disabled");
                //console.log("show");
            }else
            {
                //console.log("I had some issues");
                $(".k-edit-form-container .k-grid-update").removeClass("k-state-disabled");
                $(".k-edit-form-container .k-grid-cancel").removeClass("k-state-disabled");
                $(".k-grid .k-grid-update").removeClass("k-state-disabled");
                $(".k-grid .k-grid-edit").removeClass("k-state-disabled");
                $(".k-grid .k-grid-delete").removeClass("k-state-disabled");
                $(".k-grid .k-grid-cancel").removeClass("k-state-disabled");
               
            }
            
        }
    }
}

function onRequestEndWithAddRow(gridName) {
    return function (args) {
        var grid = $("#" + gridName).data("kendoGrid");
        var data = grid.dataSource;
        if (args.type === "create" || args.type === "update") {
            if (!args.response.Errors) {
                data.read();
                $(".k-edit-form-container .k-grid-update").removeClass("k-state-disabled");
                $(".k-edit-form-container .k-grid-cancel").removeClass("k-state-disabled");
                $(".k-grid .k-grid-update").removeClass("k-state-disabled");
                $(".k-grid .k-grid-edit").removeClass("k-state-disabled");
                $(".k-grid .k-grid-delete").removeClass("k-state-disabled");
                $(".k-grid .k-grid-cancel").removeClass("k-state-disabled");
                //$(".k-grid .k-state-disabled").addClass("k-grid-add").removeClass("k-state-disabled");
                //console.log("show");

                grid.one("dataBound", function () {
                    var that = this;
                    setTimeout(function () {
                        that.addRow();
                    }, 500);
                });

            } else {
                console.log("I had some issues");
                $(".k-edit-form-container .k-grid-update").removeClass("k-state-disabled");
                $(".k-edit-form-container .k-grid-cancel").removeClass("k-state-disabled");
                $(".k-grid .k-grid-update").removeClass("k-state-disabled");
                $(".k-grid .k-grid-edit").removeClass("k-state-disabled");
                $(".k-grid .k-grid-delete").removeClass("k-state-disabled");
                $(".k-grid .k-grid-cancel").removeClass("k-state-disabled");

            }

        }
    }
}

function onRequestStart(gridName) {
    return function (args) {

        $(".k-edit-form-container .k-grid-update").addClass("k-state-disabled");//.removeClass("k-grid-update");
        $(".k-edit-form-container .k-grid-cancel").addClass("k-state-disabled");//.removeClass("k-grid-cancel");
        $(".k-grid  .k-grid-update").addClass("k-state-disabled");//.removeClass("k-grid-update");
        $(".k-grid  .k-grid-edit").addClass("k-state-disabled");//.removeClass("k-grid-edit");
        $(".k-grid  .k-grid-delete").addClass("k-state-disabled");//.removeClass("k-grid-delete");
        $(".k-grid  .k-grid-cancel").addClass("k-state-disabled");//.removeClass("k-grid-cancel");
       
        //console.log("Hide");
    }
}


   
