

//var accountCodeTextBox;

//function SelectAccounts(e) {

//    accountCodeTextBox = $(e).prev('input').attr('id');

//    var wnd = $("#AccountCodeWindow").data("kendoWindow");
//    wnd.center().open();
//}


//function SelectionSubmitHandler(e) {

//    var account = $("#Accounts").data("kendoComboBox").value();
//    var subaccount = $("#SubAccounts").data("kendoComboBox").value();
//    var company = $("#Company").data("kendoComboBox").value();
//    var costCenter = $("#CostCent").data("kendoComboBox").value();
//    var glAcc = account + "-" + company + "-" + costCenter + "-" + subaccount;
//    if (account !== "" && subaccount !== "" && company !== "") {

//        $("#" + accountCodeTextBox).val(glAcc).trigger("change");


//        var window = $("#AccountCodeWindow").data("kendoWindow");

//        window.close();
//    } else {
//        alert("Please select all the neccessary Fields");
//    }

//}



