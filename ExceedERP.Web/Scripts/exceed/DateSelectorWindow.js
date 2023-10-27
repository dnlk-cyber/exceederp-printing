

var DateTextBox;
var DateOne;
var list = [];
function SelectDate(e) {

    DateTextBox = $(e).prev('input').attr('id');

    var wnd = $("#DateWindow").data("kendoWindow");
    wnd.center().open();
}



function AssignDates() {
    var index;
    for (var l in list)
    {
        if (list[l].id === DateTextBox) {
            index = l.toString();
            list.splice(index, 1);
        }
        
       
    }
    
        list.push({ id: DateTextBox, date: DateOne });
    
    return list;
}
function SetDate(date) {
    DateOne = date;

}

    $(document).ready(function () {

        var calendar = $.calendars.instance("ethiopian", "am");
        $(".et-popup-picker").calendarsPicker({
            calendar: calendar,
            yearRange: "+69:-69",
            popupContainer: 'body',
            position: 'relative',
            alignment: 'bottom',
            zIndex: 14000,
            onSelect: function SelectionSubmitHandler(dates) {

                var Datee = dates[0]._day + "/" + dates[0]._month + "/" + dates[0]._year;

                $("#" + DateTextBox + "").val(Datee).trigger("change");
                SetDate(Datee);
                AssignDates();
                var window = $("#DateWindow").data("kendoWindow");
                window.close();


            }

        });
    });




