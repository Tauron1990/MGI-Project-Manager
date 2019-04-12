// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, ViewEncapsulation, ChangeDetectionStrategy, NgModule, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule, MatListModule, MatIconModule, MatMenuModule, MatTooltipModule } from '@angular/material';

import { ThemeManager } from './theme-manager';
import { AppTheme } from '../../models/AppTheme';
import { ConfigurationService } from '../../services/configuration.service';

@Component({
  selector: 'app-theme-picker',
  templateUrl: 'theme-picker.component.html',
  styleUrls: ['theme-picker.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
  host: { 'aria-hidden': 'true' },
})
export class ThemePicker {
  @Input()
  tooltip = 'Theme';


  constructor(
    public themeManager: ThemeManager,
    private configuration: ConfigurationService
  ) {
    configuration.configurationImported$.subscribe(() => this.setTheme(this.currentTheme));
    this.setTheme(this.currentTheme);
  }

  get currentTheme(): AppTheme {
    return this.themeManager.getThemeByID(this.configuration.themeId);
  }

  setTheme(theme: AppTheme) {
    if (theme) {
      this.themeManager.installTheme(theme);
      this.configuration.themeId = theme.id;
    }
  }
}

@NgModule({
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatListModule,
    MatTooltipModule,
  ],
  exports: [ThemePicker],
  declarations: [ThemePicker],
  providers: [ThemeManager, ConfigurationService],
})
export class ThemePickerModule { }
