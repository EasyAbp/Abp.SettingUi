/* This file is automatically generated by ABP framework to use MVC Controllers from javascript. */


// module settingUi

(function(){

  // controller easyAbp.abp.settingUi.settingUi

  (function(){

    abp.utils.createNamespace(window, 'easyAbp.abp.settingUi.settingUi');

    easyAbp.abp.settingUi.settingUi.groupSettingDefinitions = function(ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/setting-ui',
        type: 'GET'
      }, ajaxParams));
    };

    easyAbp.abp.settingUi.settingUi.setSettingValues = function(settingValues, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/setting-ui/set-setting-values',
        type: 'PUT',
        dataType: null,
        data: JSON.stringify(settingValues)
      }, ajaxParams));
    };

    easyAbp.abp.settingUi.settingUi.resetSettingValues = function(settingNames, ajaxParams) {
      return abp.ajax($.extend(true, {
        url: abp.appPath + 'api/setting-ui/reset-setting-values',
        type: 'PUT',
        dataType: null,
        data: JSON.stringify(settingNames)
      }, ajaxParams));
    };

  })();

})();

