import { Injectable } from '@angular/core';
import { eLayoutType, addAbpRoutes, ABP } from '@abp/ng.core';
import { addSettingTab } from '@abp/ng.theme.shared';
import { SettingUiSettingsComponent } from '../components/setting-ui-settings.component';

@Injectable({
  providedIn: 'root',
})
export class SettingUiConfigService {
  constructor() {
    addAbpRoutes({
      name: 'SettingUi',
      path: 'setting-ui',
      layout: eLayoutType.application,
      order: 2,
    } as ABP.FullRoute);

    const route = addSettingTab({
      component: SettingUiSettingsComponent,
      name: 'SettingUi Settings',
      order: 1,
      requiredPolicy: '',
    });
  }
}
