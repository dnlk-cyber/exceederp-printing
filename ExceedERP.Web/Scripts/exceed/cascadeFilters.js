


function filterPeriods() {

    return {
        year: $("#GlFiscalYearId").data("kendoDropDownList").value()
    };
}

function filterServiceItems() {

    return {
        itemCategoryCode: $("#ServiceCategoryCode").data("kendoDropDownList").value(),
        text: $("#ServiceItemCode").data("kendoDropDownList").filterInput.val()
    };
}




function filterServiceItemPurchase() {

    return {
        itemCategoryCode: $("#CatCode").data("kendoDropDownList").value(),
        text: $("#ItmCode").data("kendoDropDownList").filterInput.val()
    };
}

function filterGenericCategories() {

    return {
        itemtype: $("#Type").data("kendoDropDownList").value(),
        text: $("#CatCode").data("kendoDropDownList").filterInput.val()
    };
}

function filterGenericItems() {

    return {
        itemtype: $("#Type").data("kendoDropDownList").value(),
        catcode: $("#CatCode").data("kendoDropDownList").value(),
        text: $("#ItmCode").data("kendoDropDownList").filterInput.val()
    };
}

function filterGenericItemNames() {

    return {
        itemtype: $("#Type").data("kendoDropDownList").value(),
        subcatcode: $("#ItmCode").data("kendoDropDownList").value(),
        text: $("#Name").data("kendoDropDownList").filterInput.val()
    };
}

function filterGlPeriods() {

    return {
        year: $("#GlFiscalYearId").data("kendoDropDownList").value()
    };
}

function filterStoresByBranch() {

    return {
        id: $("#BranchId").data("kendoDropDownList").value(),
        text: $("#StoreCode").data("kendoDropDownList").filterInput.val()
    };
}

function filterPeriodsByStore() {
    return {
        store: $("#StoreCode").data("kendoDropDownList").value(),
        text: $("#StoreCode").data("kendoDropDownList").filterInput.val()
    }
}





