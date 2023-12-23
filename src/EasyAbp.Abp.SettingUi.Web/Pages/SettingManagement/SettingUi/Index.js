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

    var bind_nav_event_fn = function (element) {
        var btn_set = setInterval(function () {
            var _id = '#' + element.data('id') + " ";
            event_fn(_id);
            if ($(_id + " form").length > 0) {
                clearInterval(btn_set);
            } else {
                var target = element.attr('data-bs-target');
                if (target !== undefined && $(target).length > 0) {
                    clearInterval(btn_set);
                }
            }
        }, 200);
    }

    $('#tabs-nav .nav-item .nav-link').click(function () {
        var _this = $(this);
        if (_this.attr('data-bs-target') !== undefined) {
            return;
        }
        bind_nav_event_fn(_this)
    });

    if (location.hash) {
        var index = location.hash.substring(1);
        $("#SettingManagementWrapper li.nav-item > a.nav-link")[index].click();
        history.replaceState(null, null, ' ');  // remove hash from the location url
    }

    // The first nav will automatically click and loading before on this script.
    bind_nav_event_fn($("#SettingManagementWrapper li.nav-item > a.nav-link").first());
})(jQuery);
