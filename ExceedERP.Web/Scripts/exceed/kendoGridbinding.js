
    
    function dataBound() {
        this.expandRow(this.tbody.find("tr.k-master-row").first());

        //approved
        var grid = $("#grid").data("kendoGrid");
        var gridData = grid.dataSource.view();

        for (var i = 0; i < gridData.length; i++) {
            var currentUid = gridData[i].uid;
            if (gridData[i].isOnlineApproved) {
                var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
                var editButton = $(currenRow).find(".k-grid-edit");
                var deleteButton = $(currenRow).find(".k-grid-delete");
                editButton.hide();
                deleteButton.hide();
            }
        }
    }

    function dataBoundLines() {
       
        var grid = this;
        var gridData = grid.dataSource.view();

        for (var i = 0; i < gridData.length; i++) {
            var currentUid = gridData[i].uid;

            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            var editButton = $(currenRow).find(".k-grid-edit");
            var deleteButton = $(currenRow).find(".k-grid-delete");
           // editButton.hide();
           // deleteButton.hide();
        }

        
}

function dataBoundDistribution() {

    //approved
    var grid = this;
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;

        if ("#=IsOnlineTransferred#") {
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            //var editButton = $(currenRow).find(".k-grid-edit");
            //var deleteButton = $(currenRow).find(".k-grid-delete");
            //editButton.hide();
            //deleteButton.hide();
        }
    }


}

function dataBoundValidation() {

    //approved
    var grid = this;
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;

        var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
        //var editButton = $(currenRow).find(".k-grid-edit");
        //var deleteButton = $(currenRow).find(".k-grid-delete");
        //editButton.hide();
        //deleteButton.hide();
    }


}

function error_handler(e) {
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
    }
}

function sync_handler(e) {
    this.read();
}

