function storeRequisitionValueMapper(options) {
    $.ajax({
        url: "/Inventory/DataProvider/StoreRequisitionId_ValueMapper/",
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


function fuelRequisitionValueMapper(options) {
    $.ajax({
        url: "/Inventory/DataProvider/FuelRequisitionId_ValueMapper/",
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

