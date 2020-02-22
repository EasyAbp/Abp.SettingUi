import { NgModule } from '@angular/core';
import { SettingUiComponent } from './components/setting-ui.component';
import { SettingUiRoutingModule } from './setting-ui-routing.module';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CoreModule } from '@abp/ng.core';

@NgModule({
  declarations: [SettingUiComponent],
  imports: [CoreModule, ThemeSharedModule, SettingUiRoutingModule],
  exports: [SettingUiComponent],
})
export class SettingUiModule {}
