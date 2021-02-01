$(document).ready(function () {

    //activating the moduleLink in cookie
    var active = Cookies.get('currentNavLi');
    $('#' + active).addClass('active');
    $('#' + active).parents('li').addClass('open active');
    //getting the clicked menu
    $('li.actionlinlk').click(function (e) {
        //remove previous
        var active = Cookies.get('currentNavLi');
        $('#' + active).removeClass('active');
        $('#' + active).parents('li').removeClass('open active');
        Cookies.remove('currentNavLi');
        //set new
        $(this).parents('li').addClass('open active');
        $(this).addClass('active');
        var id = $(this).attr('id');
        Cookies.set('currentNavLi', id);
    });


   
});