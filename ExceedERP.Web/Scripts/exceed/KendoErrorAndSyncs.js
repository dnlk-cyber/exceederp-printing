function error_handler_one(e) {
    if (e.errors) {
        var message = "Errors:\n";
        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += this + "\n";
                });
            }
        });
        alert(message);
        //toastr.warning(message, "Warning");

        //var grid = $("#grid").data("kendoGrid");
        //grid.one("dataBinding", function (e) {
        //    e.preventDefault();
        //});
    }
}

function sync_handler(e) {
    if (e.errors) {
    }
    else {
        this.read();
    }
}

function error_handler_two(gridName) {
    return function(e) {
        if (e.errors) {
            var message = "Errors:\n";
            $.each(e.errors,
                function(key, value) {
                    if ('errors' in value) {
                        $.each(value.errors,
                            function() {
                                message += this + "\n";
                            });
                    }
                });
            //alert(message);
            toastr.warning(message, "Warning");

            var grid = $("#" + gridName).data("kendoGrid");
            grid.one("dataBinding", function (e) {
                e.preventDefault();
            });
        }
    }
}


