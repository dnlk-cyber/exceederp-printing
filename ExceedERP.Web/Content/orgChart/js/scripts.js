'use strict';


$('#chart-container').orgchart({
    'data': '/HRM/OrganizationChart/GetData',
    'depth': 3,
    'nodeTitle': 'name',
    'nodeContent': 'name',
    'exportButton': true,
    'exportFilename': 'Org Chart',
    'createNode': function ($node, data) {
        var secondMenuIcon = $('<i>', {
            'class': 'fa fa-info-circle second-menu-icon',
            click: function () {
                $(this).siblings('.second-menu').toggle();
            }
        });
        //var secondMenu = '<div class="second-menu"><img class="avatar" src="../img/avatar/' + data.id + '.jpg"></div>';
        var secondMenu = '<div class="second-menu"><img height="10" width="10" class="avatar" src="../Content/orgChart/person.png"></div>';
        $node.append(secondMenuIcon).append(secondMenu);
    }
});
