// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class Footer {
  getYear() {
    return new Date().getUTCFullYear();
  }
}

@NgModule({
  exports: [Footer],
  declarations: [Footer],
})
export class FooterModule { }
