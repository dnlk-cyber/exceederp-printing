

var accountCodeTextBox;
var popup;
var aCode;
var previosAccountCode;
function SelectAccCode(e) {

    accountCodeTextBox = $(e).prev('input').attr('id'); 
    previosAccountCode = $("#" + accountCodeTextBox).val(); console.log("prev:"+previosAccountCode);
  //get setting
    $.ajax({
        type: "POST",
        url: "/Finance/FinanceDataProvider/GetCoAType/",
        success: function (result) {
            if (result.Type === "No") {
                popup = window.open('/Finance/AccountCode/GLAccountSelector?q=' + previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
                popup.focus();

                return false;

            } else {
                //alert("auto");
                popup = window.open('/Finance/AccountCodeSelector/GLAccountSelector?q=' + previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
                popup.focus();
                return false;
            }
        }, //end succ
        error: function (result) {
            alert(result);
        }
    });
    //popup = window.open('/Finance/AccountCode/GLAccountSelector?q='+previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
    //popup.focus();

    //return false;
}

function SelectAccCodeZZZ(e) {

    accountCodeTextBox = $(e).prev('input').attr('id');
    previosAccountCode = $("#" + accountCodeTextBox).val();
    console.log("pre"+previosAccountCode);
    //get setting
    $.ajax({
        type: "POST",
        url: "/Finance/FinanceDataProvider/GetCoAType/",
        success: function (result) {
            if (result.Type === "No") {
                popup = window.open('/Finance/AccountCode/GLAccountSelector?q=' + previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
                popup.focus();

                return false;

            } else {
                //alert("auto");
                popup = window.open('/Finance/AccountCodeSelector/GLAccountSelector?s=true&q='+ previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
                popup.focus();
                return false;
            }
        }, //end succ
        error: function (result) {
            alert(result);
        }
    });
    //popup = window.open('/Finance/AccountCode/GLAccountSelector?q='+previosAccountCode, 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
    //popup.focus();

    //return false;
}

function SelectDynamicAccCode(e) {

    accountCodeTextBox = $(e).prev('input').attr('id');
    popup = window.open('/Finance/AccountCodeSelector/GLAccountSelector', 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
    popup.focus();
    return false;
}

//old with id of popup wrapper
function SelectAccCodeold(e) {
    var pid = $(e).parent()[0].id;
    accountCodeTextBox = $("#" + pid + " :input")[0].id;

    //alert(accountCodeTextBox);
    popup = window.open('/Finance/AccountCode/GLAccountSelector', 'WindowPopup', 'top=100, left=400, width=850 , height=400,resizable=no,');
    popup.focus();
    return false;
}

function setLang(accCode) {
    aCode = accCode;
    $("#" + accountCodeTextBox).val(aCode).trigger("change");
}

