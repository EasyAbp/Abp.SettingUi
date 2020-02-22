import { NgModule, APP_INITIALIZER } from '@angular/core';
import { SettingUiConfigService } from './services/setting-ui-config.service';
import { noop } from '@abp/ng.core';
import { SettingUiSettingsComponent } from './components/setting-ui-settings.component';

@NgModule({
  declarations: [SettingUiSettingsComponent],
  providers: [{ provide: APP_INITIALIZER, deps: [SettingUiConfigService], multi: true, useFactory: noop }],
  exports: [SettingUiSettingsComponent],
  entryComponents: [SettingUiSettingsComponent],
})
export class SettingUiConfigModule {}
