﻿(function ($) {

    var service = easyAbp.abp.settingUi.settingUi;
    var l = abp.localization.getResource("AbpSettingManagement");

    $("form").submit(function (e) {
        e.preventDefault();

        if (!$(e.currentTarget).valid()) {
            return;
        }

        var input = $(e.currentTarget).serializeFormToObject();
        abp.log.input(input);
        service.setSettingValues(input)
            .then(function (result) {
                abp.notify.success(l("SuccessfullySaved"));
            });
    });

    $(".reset").click(function (e) {
        abp.message.confirm(
            l("ResetConfirm"),
            function (result) {
                if (result) {
                    var form = e.target.closest("form")[0];
                    var input = $(form).map(function() {return this.id;}).get();
                    abp.log.debug(input);
                    service.resetSettingValues(input)
                        .then(function (result) {
                            location.reload();
                        });
                }
            }
        );
    })
})(jQuery);
