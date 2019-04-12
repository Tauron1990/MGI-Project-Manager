// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { AppPage } from './app.po';

describe('QuickApp-PRO App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display application title: QuickApp Pro', () => {
    page.navigateTo();
    expect(page.getAppTitle()).toEqual('QuickApp Pro');
  });
});
