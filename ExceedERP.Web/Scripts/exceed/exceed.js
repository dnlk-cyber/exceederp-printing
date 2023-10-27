function appendZeros(num) {
    var length = 7;
    var numStr = num.toString();
    while (numStr.length < length) {
        numStr = '0' + numStr;
    }

    return numStr;
}



function displayUserName(id) {
    var approverElement = document.getElementById("user_name_display");

    $.ajax({
        type: "POST",
        url: '/Common/GlobalDataProvider/GetUserFullName',
        data: {
            id: id
        },
        success: function (result) {
            // toastr.warnning(result.Message);
            approverElement.innerText = "-" + result.Name;

        }, //end succ
        error: function (result) {
            alert(result);
        }
    });


}

var nextApproverId;
var popup;
var slectedUser;
function SelectNextApprover(e, docid, doctype) {

    nextApproverId = $(e).prev('input').attr('id');
    popup = window.open('/Common/GlobalDataProvider/GetNextApprover?id=' + docid+"&t="+doctype, 'WindowPopup', 'top=100, left=400, width=400, height=230,resizable=no,');
    popup.focus();
    return false;
}

function setSelectedUser(user) {
    slectedUser = user;
    $("#" + nextApproverId).val(slectedUser).trigger("change");
}