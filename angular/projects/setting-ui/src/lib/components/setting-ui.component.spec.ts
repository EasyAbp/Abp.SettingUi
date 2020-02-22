import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SettingUiComponent } from './setting-ui.component';

describe('SettingUiComponent', () => {
  let component: SettingUiComponent;
  let fixture: ComponentFixture<SettingUiComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SettingUiComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingUiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
