// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { ConfigurationService } from '../../services/configuration.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { AccountService } from '../../services/account.service';
import { Utilities } from '../../services/utilities';

export interface PageInfo {
  title: string;
  icon: string;
  path: string;
  isDefault: boolean;
}

export interface LanguagePreference {
  name: string;
  locale: string;
  isDefault: boolean;
}

@Component({
  selector: 'user-preferences',
  templateUrl: './user-preferences.component.html',
  styleUrls: ['./user-preferences.component.scss']
})
export class UserPreferencesComponent {
  languages: LanguagePreference[] = [
    { name: 'English', locale: 'en', isDefault: true },
    { name: 'French', locale: 'fr', isDefault: false },
    { name: 'German', locale: 'de', isDefault: false },
    { name: 'Portuguese', locale: 'pt', isDefault: false },
    { name: 'Arabic', locale: 'ar', isDefault: false },
    { name: 'Korean', locale: 'ko', isDefault: false }
  ];

  homePages: PageInfo[] = [
    { title: 'Dashboard', icon: 'dashboard', path: '/', isDefault: true },
    { title: 'About', icon: 'info', path: '/about', isDefault: false },
    { title: 'Settings', icon: 'settings', path: '/settings', isDefault: false }
  ];

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private snackBar: MatSnackBar,
    public configurations: ConfigurationService
  ) { }

  get currentHomePage(): PageInfo {
    return this.homePages.find(x => x.path == this.configurations.homeUrl) || this.homePages[0];
  }

  reload() {
    this.snackBar.open('Reload preferences?', 'RELOAD', { duration: 5000 })
      .onAction().subscribe(() => {
        this.alertService.startLoadingMessage();

        this.accountService.getUserPreferences()
          .subscribe(results => {
            this.alertService.stopLoadingMessage();

            this.configurations.import(results);

            this.alertService.showMessage('Defaults loaded!', '', MessageSeverity.info);

          },
            error => {
              this.alertService.stopLoadingMessage();
              this.alertService.showStickyMessage('Load Error', `Unable to retrieve user preferences from the server.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
                MessageSeverity.error, error);
            });
      });
  }

  save() {
    this.snackBar.open('Save preferences?', 'SAVE', { duration: 5000 })
      .onAction().subscribe(() => {
        this.alertService.startLoadingMessage('', 'Saving new defaults');

        this.accountService.updateUserPreferences(this.configurations.export())
          .subscribe(response => {
            this.alertService.stopLoadingMessage();
            this.alertService.showMessage('New Defaults', 'Account defaults updated successfully', MessageSeverity.success);

          },
            error => {
              this.alertService.stopLoadingMessage();
              this.alertService.showStickyMessage('Save Error', `An error occured whilst saving configuration defaults.\r\nErrors: "${Utilities.getHttpResponseMessages(error)}"`,
                MessageSeverity.error, error);
            });
      });
  }

  reset() {
    this.snackBar.open('Resst defaults?', 'RESET', { duration: 5000 })
      .onAction().subscribe(() => {
        this.configurations.import(null);
        this.alertService.showMessage('Defaults Reset', 'Account defaults reset completed successfully', MessageSeverity.success);
      });
  }
}
