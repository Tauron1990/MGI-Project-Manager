// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Injectable } from '@angular/core';

import { AppTheme } from '../../models/AppTheme';

@Injectable()
export class ThemeManager {
  themes: Array<AppTheme> = [
    {
      id: 1,
      name: 'Indigo/Pink',
      primary: '#3F51B5',
      accent: '#E91E63',
      href: 'indigo-pink.css',
      isDark: false,
      isDefault: true,
    },
    {
      id: 2,
      name: 'Purple/Amber',
      primary: '#673AB7',
      accent: '#FFC107',
      href: 'deeppurple-amber.css',
      isDark: false,
    },
    {
      id: 3,
      name: 'Pink/Blue',
      primary: '#E91E63',
      accent: '#607D8B',
      href: 'pink-bluegrey.css',
      isDark: true,
    },
    {
      id: 4,
      name: 'Purple/Green',
      primary: '#9C27B0',
      accent: '#4CAF50',
      href: 'purple-green.css',
      isDark: true,
    },
  ];

  public installTheme(theme: AppTheme) {
    if (theme == null || theme.isDefault) {
      this.removeStyle('theme');
    } else {
      this.setStyle('theme', `assets/themes/${theme.href}`);
    }
  }

  public getThemeByID(id: number): AppTheme {
    return this.themes.find(theme => theme.id === id);
  }

  private setStyle(key: string, href: string) {
    this.getLinkElementForKey(key).setAttribute('href', href);
  }

  private removeStyle(key: string) {
    const existingLinkElement = this.getExistingLinkElementByKey(key);
    if (existingLinkElement) {
      document.head.removeChild(existingLinkElement);
    }
  }

  private getLinkElementForKey(key: string) {
    return this.getExistingLinkElementByKey(key) || this.createLinkElementWithKey(key);
  }

  private getExistingLinkElementByKey(key: string) {
    return document.head.querySelector(`link[rel="stylesheet"].${this.getClassNameForKey(key)}`);
  }

  private createLinkElementWithKey(key: string) {
    const linkEl = document.createElement('link');
    linkEl.setAttribute('rel', 'stylesheet');
    linkEl.classList.add(this.getClassNameForKey(key));
    document.head.appendChild(linkEl);
    return linkEl;
  }

  private getClassNameForKey(key: string) {
    return `style-manager-${key}`;
  }
}
