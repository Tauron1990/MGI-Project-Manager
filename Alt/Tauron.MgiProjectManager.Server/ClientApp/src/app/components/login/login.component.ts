// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, ViewChild } from '@angular/core';

import { LoginControlComponent } from './login-control.component';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent {
    @ViewChild(LoginControlComponent)
    loginControl: LoginControlComponent;
}
