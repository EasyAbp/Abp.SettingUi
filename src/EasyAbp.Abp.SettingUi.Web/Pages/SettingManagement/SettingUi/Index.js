(function ($) {

    var service = easyAbp.abp.settingUi.settingUi;
    var l = abp.localization.getResource("EasyAbpAbpSettingUi");

    var event_fn = function (id = '') {
        $(id + "form").submit(function (e) {
            e.preventDefault();
            // Skip abp `EmailSettingsForm`
            if (e.currentTarget.id == 'EmailSettingsForm') {
                return;
            }

            if (!$(e.currentTarget).valid()) {
                return;
            }

            var input = $(e.currentTarget).serializeFormToObject();
            service.setSettingValues(input)
                .then(function (result) {
                    //abp.notify.success(l("SuccessfullySaved"));
                    $(document).trigger("AbpSettingSaved");
                });
        });

        $(id + ".reset").click(function (e) {
            var form = e.currentTarget.closest("form");
            abp.message.confirm(
                l("ResetConfirm", $(form).find("h4").text()),
                function (result) {
                    if (result) {
                        var input = $(form)
                            .find(":input[id]")
                            .map(function () { return this.id; })
                            .get();
                        service.resetSettingValues(input)
                            .then(function (result) {
                                // get the index of current selected tab
                                var index = $("a.nav-link.active").parents().index()
                                location.href = "#" + index;
                                location.reload();
                            });
                    }
                }
            );
        })
    }

    if ($('.nav-item').parent().attr('id') == 'tabs-nav') {
        //Support versions after 6.0 binding
        $('#tabs-nav .nav-item .nav-link').click(function () {
            var _this = $(this);
            if (_this.attr('data-bs-target') !== undefined) {
                return;
            }
            var btn_set = setInterval(function () {
                var _id = '#' + _this.data('id') + " ";
                event_fn(_id);
                if ($(_id + " form").length > 0) {
                    clearInterval(btn_set);
                }
            }, 200);
        });
    }
    else {
        //Support versions before 6.0 binding
        event_fn();
    }

    if (location.hash) {
        var index = location.hash.substring(1);
        $("#SettingManagementWrapper li.nav-item > a.nav-link")[index].click();
        history.replaceState(null, null, ' ');  // remove hash from the location url
    }
})(jQuery);
