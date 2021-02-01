
var masterDataItem;
var masterRow;
var masterGrid;

var codeid;
var isapproved;
var issecondapproved;
var isSendForApproval;
var isOnlineTransferred;

function detailExpand(e) {

    masterRow = e.masterRow;
    masterGrid = $("#grid").data("kendoGrid");
    masterDataItem = masterGrid.dataItem(masterRow);
    //codeid = masterDataItem.StoreRequisitionID;
    isapproved = masterDataItem.IsOnlineApproved;
    isSendForApproval = masterDataItem.IsSendForApproval;
    isOnlineTransferred = masterDataItem.IsOnlineTransferred;
    issecondapproved = masterDataItem.IsSecondOnlineApproved;

}
function onDataBoundMaster(e) {
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
function onDataBoundLines(e) {

    var detailGrid = "#" + e.sender.element[0].id;// "#StoreRequisitionItem_" + codeid;
    if (isapproved || isSendForApproval) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }
}
function onDataBoundFisrtValidation(e) {

    var detailGrid = "#" + e.sender.element[0].id;// "#StoreRequisitionValidation_" + codeid;

    if (isapproved || !isSendForApproval) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }
   
    this.table.find(".k-grid-edit").hide();
}
function onDataBoundSecondValidation(e) {
    var detailGrid = "#" + e.sender.element[0].id;

    if (issecondapproved) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }

    this.table.find(".k-grid-edit").hide();
}
function onDataBoundDistribution(e) {

    var detailGrid = "#" + e.sender.element[0].id;

    if (isOnlineTransferred) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }

    //alert(isOnlineTransferred);
}

function onDataBoundDistributionFinance(e) {

    var detailGrid = "#" + e.sender.element[0].id;

    if (isOnlineTransferred) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }

    alert(isOnlineTransferred);
}


function onDataBoundLinesSecond(e) {

    var detailGrid = "#" + e.sender.element[0].id;// "#StoreRequisitionItem_" + codeid;
    if (issecondapproved) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }
}

function onDataBoundGoodReceiveInspectionLines(e) {
    var detailGrid = "#" + e.sender.element[0].id;// "#StoreRequisitionPropertyValidation_" + codeid;

    if (isapproved) {

        $(".k-grid-add", detailGrid).hide();
        $(".k-grid-delete", detailGrid).hide();
        $(".k-grid-edit", detailGrid).hide();
    }

    this.table.find(".k-grid-edit").hide();
}

