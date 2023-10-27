




function filterSubCategoryNames() {

    return {
        subCategoryCode: $("#FixedAssetSubCategoryID").data("kendoDropDownList").value(),
        text: $("#FixedAssetNameID").data("kendoDropDownList").filterInput.val()
    };
}

function filterSubCategories() {

    return {
        categoryCode: $("#FixedAssetCategoryID").data("kendoDropDownList").value(),
        text: $("#FixedAssetSubCategoryID").data("kendoDropDownList").filterInput.val()
    };
}

function filterAssets() {

    return {
        subCategoryCode: $("#FixedAssetSubCategoryID").data("kendoDropDownList").value()
    };
}
