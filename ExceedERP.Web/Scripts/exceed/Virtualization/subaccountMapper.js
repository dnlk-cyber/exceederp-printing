function subAccountValueMapper(options) {
    $.ajax({
        url: "/Finance/AccountCode/VirtualSubAccount_Read/",
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