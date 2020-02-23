﻿(function ($) {

    var service = easyAbp.abp.settingUi.settingUi;
    var l = abp.localization.getResource('AbpSettingManagement');

    $("form").submit(function (e) {
        e.preventDefault();

        if (!$(e.currentTarget).valid()) {
            return;
        }

        var input = $(e.currentTarget).serializeFormToObject();
        abp.log.info(input);
        service.setSettingValues(input)
            .then(function (result) {
                abp.notify.success(l("SuccessfullySaved"));
            });
    });
})(jQuery);
