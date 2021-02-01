function filterInventoryItems() {
    
    return {
        itemCategoryCode: $("#ItemCategoryCode").data("kendoDropDownList").value(),
        text: $("#ItemCode").data("kendoDropDownList").filterInput.val()
    };
}

function onSelectItemCode(e) {
    
    var dataItem = this.dataItem(e.item);
    //begin
    var item = dataItem.ItemCode;
    $.ajax({
        url: '/Inventory/DataProvider/GetItemPartNo',
        type: 'POST',
        data: { id: item },
        success: function (result) {
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    $("#PartNo").val(result[i].PartNo).trigger("change");
                    $("#UOM").val(result[i].UoM).trigger("change");
                }
            }
            else {

            }

        }, //end succ
        error: function (result) {
            alert(result);
        }
    }); //end post
    //end
}

function onSelectStoreRequisitionID(e) {

    var dataItem = this.dataItem(e.item);
    //begin
    var item = dataItem.StoreRequisitionID;
    $.ajax({
        url: '/Inventory/DataProvider/GetStoreCodeFromSrNo',
        type: 'POST',
        data: { id: item },
        success: function (result) {
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    $("#StoreCode").val(result[i].StoreCode).trigger("change");
                }
            }
            else {

            }

        }, //end succ
        error: function (result) {
           // alert(result);
        }
    }); //end post
    //end
}

function onSelectFixedAssetRequisitionId(e) {

    var dataItem = this.dataItem(e.item);
    //begin
    var item = dataItem.FixedAssetRequisitionId;
    $.ajax({
        url: '/FixedAsset/FixedAssetDataProvider/GetStoreCodeFromSrNo',
        type: 'POST',
        data: { id: item },
        success: function (result) {
            if (result.length > 0) {
                for (var i = 0; i < result.length; i++) {
                    $("#StoreCode").val(result[i].StoreCode).trigger("change");
                }
            }
            else {

            }

        }, //end succ
        error: function (result) {
            //alert(result);
        }
    }); //end post
    //end
}