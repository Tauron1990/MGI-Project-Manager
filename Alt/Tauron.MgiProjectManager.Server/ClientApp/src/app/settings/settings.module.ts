// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { NgModule } from '@angular/core';

import { SharedModule } from '../shared/shared.module';

import { SettingsComponent } from './settings.component';
import { UserPreferencesComponent } from './user-preferences/user-preferences.component';

@NgModule({
  imports: [
    SharedModule
  ],
  exports: [
    SettingsComponent
  ],
  declarations: [
    SettingsComponent,
    UserPreferencesComponent
  ]
})
export class SettingsModule { }
