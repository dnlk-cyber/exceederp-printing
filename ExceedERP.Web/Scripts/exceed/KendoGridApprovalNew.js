var masterDataItem;
var masterRow;
var masterGrid;

var codeid;
var isapproved;
var issecondapproved;
var isSendForApproval;
var isOnlineTransferred;
var nextApprover;
var currentUser;

function detailExpandMasterNew(e) {

    masterRow = e.masterRow;
    masterGrid = $("#grid").data("kendoGrid");
    masterDataItem = masterGrid.dataItem(masterRow);
    //codeid = masterDataItem.StoreRequisitionID;
    isapproved = masterDataItem.IsOnlineApproved;
    isSendForApproval = masterDataItem.IsSendForApproval;
    isOnlineTransferred = masterDataItem.IsOnlineTransferred;
    issecondapproved = masterDataItem.IsSecondOnlineApproved;
    nextApprover = masterDataItem.ForwardedTo;

}
function onDataBoundMasterNew(e) {
    //this.expandRow(this.tbody.find("tr.k-master-row").first());

    //approved
    var grid = $("#grid").data("kendoGrid");
    var gridData = grid.dataSource.view();

    for (var i = 0; i < gridData.length; i++) {
        var currentUid = gridData[i].uid;
        if (gridData[i].IsOnlineApproved || gridData[i].IsSendForApproval) {
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            var editButton = $(currenRow).find(".k-grid-edit");
            var deleteButton = $(currenRow).find(".k-grid-delete");
            editButton.hide();
            deleteButton.hide();
        }
    }


    //changing colors
    var rows = e.sender.tbody.children();
    for (var j = 0; j < rows.length; j++) {
        var row = $(rows[j]);
        var dataItem = e.sender.dataItem(row);
        if (dataItem.IsSendForApproval && !dataItem.IsOnlineApproved) {
            row.css("background-color", "lightgreen");
        }
        if (!dataItem.IsSendForApproval && !dataItem.IsOnlineApproved) {
            row.css("background-color", "lightskyblue");
        }

        if (dataItem.IsVoid) {
            row.css("background-color", "slategray");
            row.css("color", "black");
        }
    }

}
function onDataBoundLineNew(e) {

    var detailGrid = "#" + e.sender.element[0].id;// "#StoreRequisitionItem_" + codeid;
    if (isapproved || isSendForApproval) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }
}


function onDataBoundValidationNew(e) {
    $.ajax({
        type: "GET",
        url: "/Common/GlobalDataProvider/GetCurentUser/",
        data: { id: codeid },
        success: function (result) {
            //alert(result.User);
            currentUser = result.User;
            //console.log(1+result.User);
            // console.log(2+currentUser);
        }, //end succ
        error: function (result) {
            alert(result);
        }
    });

    var detailGrid = "#" + e.sender.element[0].id;

    if (isapproved || !isSendForApproval) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }
   
    if (nextApprover.toString() !== currentUser.toString()) {
        $(".k-grid-add", detailGrid).hide();
    }

   

    this.table.find(".k-grid-edit").hide();
}
function onDataBoundDistributionNew(e) {

    var detailGrid = "#" + e.sender.element[0].id;

    if (isOnlineTransferred) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }


}