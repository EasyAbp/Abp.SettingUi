﻿(function ($) {

    var service = easyAbp.abp.settingUi.settingUi;
    var l = abp.localization.getResource("SettingUi");

    $("form").submit(function (e) {
        e.preventDefault();

        if (!$(e.currentTarget).valid()) {
            return;
        }

        var input = $(e.currentTarget).serializeFormToObject();
        service.setSettingValues(input)
            .then(function (result) {
                abp.notify.success(l("SuccessfullySaved"));
            });
    });

    $(".reset").click(function (e) {
        var form = e.currentTarget.closest("form");
        abp.message.confirm(
            l("ResetConfirm", $(form).find("h4").text()),
            function (result) {
                if (result) {
                    var input = $(form)
                        .find(":input[id]")
                        .map(function() {return this.id;})
                        .get();
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
