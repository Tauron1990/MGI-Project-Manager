// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, ViewChild, AfterViewInit, Inject } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { LoginControlComponent } from './login-control.component';

@Component({
    selector: 'app-login-dialog',
    templateUrl: 'login-dialog.component.html',
    styleUrls: ['login-dialog.component.scss']
})
export class LoginDialogComponent {
    @ViewChild(LoginControlComponent)
    loginControl: LoginControlComponent;

    constructor(
        public dialogRef: MatDialogRef<LoginDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any) { }

    ngAfterViewInit() {
        this.loginControl.modalClosedCallback = () => this.dialogRef.close(true);
    }

    onCancel(): void {
        this.dialogRef.close(false);
    }
}
