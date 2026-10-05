(function ($) {

    var service = easyAbp.abp.settingUi.settingUi;
    var l = abp.localization.getResource("EasyAbpAbpSettingUi");

    // Reloads the page and opens the current tab again.
    var reload_fn = function () {
        // get the index of current selected tab (the theme can have other active nav links, e.g. in its menu)
        var index = $("#tabs-nav .nav-item .nav-link").index($("#tabs-nav .nav-item .nav-link.active"));
        if (index >= 0) {
            location.href = "#" + index;
        }
        location.reload();
    }

    // Mirrors SettingHtmlInfo.GetValueSourceKey: the localization key suffix of the badge, or null.
    var valueSourceKey = function (settingInfo) {
        if (settingInfo.isValueSetHere) {
            return "SetHere";
        }

        switch (settingInfo.valueProviderName) {
            case null:
            case undefined:
                return "NotSet";
            case "D":
                return "Default";
            case "C":
                return "Configuration";
            case "G":
                return abp.currentTenant && abp.currentTenant.isAvailable ? "InheritedFromHost" : "Global";
            case "T":
                return "Tenant";
            case "U":
                return "User";
            default:
                return null;
        }
    }

    // After a save, shows where each value of the card now comes from, without a reload that would lose the
    // changes of the other cards.
    var refresh_fn = function (form) {
        service.groupSettingDefinitions().then(function (groups) {
            var settingInfos = {};
            $.each(groups, function (i, group) {
                $.each(group.settingInfos, function (j, settingInfo) {
                    settingInfos[settingInfo.name] = settingInfo;
                });
            });

            $(form).find(".setting-ui-setting").each(function () {
                var settingInfo = settingInfos[$(this).attr("data-setting-name")];
                if (!settingInfo) {
                    return;
                }

                var key = valueSourceKey(settingInfo);
                $(this).find(".setting-ui-value-source")
                    .text(key ? l("ValueSource:" + key) : settingInfo.valueProviderName)
                    .toggleClass("text-bg-primary", settingInfo.isValueSetHere)
                    .toggleClass("text-bg-light border", !settingInfo.isValueSetHere);
                $(this).find(".setting-ui-reset-setting").toggleClass("d-none", !settingInfo.isValueSetHere);

                var $input = $(document.getElementById(settingInfo.name));
                if (settingInfo.isEncrypted) {
                    // Masked and empty again: the saved value is kept until the box is changed.
                    $input.val("")
                        .attr("type", "password")
                        .attr("data-setting-ui-has-value", settingInfo.hasValue ? "true" : "false")
                        .attr("placeholder", l(settingInfo.hasValue ? "EncryptedValueKept" : "EncryptedValueNotSet"))
                        .removeData("settingUiLoaded");
                    setRevealed($(this).find(".setting-ui-reveal"), false);
                } else if ($input.is("input[data-datepicker]")) {
                    // A date picker is set through its hidden input, which formats the box and reads an instant
                    // in the browser's time zone, as when the page is rendered.
                    if ($input.val() === "" && settingInfo.value !== null) {
                        $input.closest("abp-date-picker").find("input[type=hidden][data-date]").val(settingInfo.value);
                    }
                } else if ($input.is("input[type=text], input[type=number], textarea, select") &&
                    $input.val() === "" && settingInfo.value !== null) {
                    // A cleared box was reset: show the value it inherits now.
                    $input.val(settingInfo.value);
                }
            });
        });
    }

    // A dateTime box shows its value in the browser's time zone, but its text carries no zone. The server would
    // read it in the current user's time zone, which comes from the Abp.Timing.TimeZone setting before the
    // browser's, and save another instant whenever the two differ. Post the instant the box shows instead, with
    // the browser's offset: a value stored without a zone is shown as its wall clock, and the offset lets the
    // server see that an untouched box still shows it.
    var setDateTimeInstants = function (form, input) {
        $(form).find("input[data-setting-ui-date-time]").each(function () {
            var $box = $(this);
            // serializeFormToObject camel-cases the names; a form name has no dots, so only its first letter.
            var key = abp.utils.toCamelCase(this.name);
            if (!$box.val() || !$box.data("format") || !input.hasOwnProperty(key)) {
                return; // an empty box resets the setting
            }

            // The picker writes the box in this format and reads it back the same way, in the browser's time zone.
            var instant = moment($box.val(), $box.data("format"));
            if (instant.isValid()) {
                input[key] = instant.toISOString(true); // 2026-01-15T17:00:00.000+08:00
            }
        });
    }

    var setRevealed = function ($button, revealed) {
        var text = l(revealed ? "HideValue" : "ShowValue");
        $button.attr("title", text).attr("aria-label", text);
        $button.find("i").toggleClass("fa-eye", !revealed).toggleClass("fa-eye-slash", revealed);
    }

    var event_fn = function (id = '') {
        $(id + " form.setting-ui").submit(function (e) {
            e.preventDefault();

            if (!$(e.currentTarget).valid()) {
                return;
            }

            var input = $(e.currentTarget).serializeFormToObject();
            setDateTimeInstants(e.currentTarget, input);
            service.setSettingValues(input)
                .then(function (result) {
                    //abp.notify.success(l("SuccessfullySaved"));
                    $(document).trigger("AbpSettingSaved");
                    refresh_fn(e.currentTarget);
                });
        });

        $(id + " form.setting-ui .reset").click(function (e) {
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
                                reload_fn();
                            });
                    }
                }
            );
        })

        // Resets one setting so it inherits its value again.
        $(id + " form.setting-ui").on("click", ".setting-ui-reset-setting", function (e) {
            var name = $(e.currentTarget).attr("data-setting-name");
            var displayName = $(e.currentTarget).closest(".setting-ui-setting").find("label.form-label").first().text();
            abp.message.confirm(
                l("ResetSettingConfirm", displayName),
                function (result) {
                    if (result) {
                        service.resetSettingValues([name])
                            .then(function (result) {
                                reload_fn();
                            });
                    }
                }
            );
        });

        // Shows or hides the value of an encrypted setting, loading it the first time.
        $(id + " form.setting-ui").on("click", ".setting-ui-reveal", function (e) {
            var $button = $(e.currentTarget);
            var $input = $button.closest(".input-group").find("input");

            if ($input.attr("type") !== "password") {
                $input.attr("type", "password");
                setRevealed($button, false);
                return;
            }

            var show = function () {
                $input.attr("type", "text");
                setRevealed($button, true);
            };

            // Load the value only into an untouched box; what the user typed is shown as it is.
            if ($input.data("settingUiLoaded") || $input.val() !== "" || $input.attr("data-setting-ui-has-value") !== "true") {
                show();
                return;
            }

            // The call can take a moment (it is audited): show it is loading and ignore more clicks meanwhile.
            $button.prop("disabled", true).find("i").removeClass("fa-eye").addClass("fa-spinner fa-spin");
            service.getSettingValue($button.attr("data-setting-name"))
                .then(function (value) {
                    $input.data("settingUiLoaded", true);
                    if ($input.val() === "") {
                        $input.val(value || "");
                    }
                    show();
                })
                .always(function () {
                    $button.prop("disabled", false).find("i").removeClass("fa-spinner fa-spin");
                    if (!$button.find("i").hasClass("fa-eye-slash")) {
                        $button.find("i").addClass("fa-eye");
                    }
                });
        });
    }

    var bind_nav_event_fn = function (element) {
        var btn_set = setInterval(function () {
            var _id = '#' + element.data('id').replace(/\./g, '-');
            if ($(_id + " form.setting-ui").length > 0) {
                event_fn(_id);
                clearInterval(btn_set);
                return;
            }

            // Rendering is done but not SettingUi.
            var target = element.attr('data-bs-target');
            if (target !== undefined && $(target).length > 0) {
                clearInterval(btn_set);
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
        $("#tabs-nav .nav-item .nav-link")[index].click();
        history.replaceState(null, null, ' ');  // remove hash from the location url
    }

    // The first nav will automatically click and loading before on this script.
    bind_nav_event_fn($("#tabs-nav .nav-item .nav-link").first());
})(jQuery);
