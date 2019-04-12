// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { TranslateModule } from '@ngx-translate/core';

import { QuickAppProMaterialModule } from '../modules/material.module';

import { PageHeaderComponent } from './page-header/page-header.component';
import { UserEditorComponent } from '../admin/user-editor/user-editor.component';
import { AppDialogComponent } from './app-dialog/app-dialog.component';

import { GroupByPipe } from '../pipes/group-by.pipe';

@NgModule({
  imports: [
    FlexLayoutModule,
    FormsModule, ReactiveFormsModule,
    BrowserModule, BrowserAnimationsModule,
    QuickAppProMaterialModule,
    TranslateModule
  ],
  exports: [
    FlexLayoutModule,
    FormsModule, ReactiveFormsModule,
    BrowserModule, BrowserAnimationsModule,
    QuickAppProMaterialModule,
    TranslateModule,
    PageHeaderComponent,
    GroupByPipe,
    UserEditorComponent,
    AppDialogComponent
  ],
  declarations: [
    PageHeaderComponent,
    GroupByPipe,
    UserEditorComponent,
    AppDialogComponent
  ],
  entryComponents: [
    AppDialogComponent
  ]
})
export class SharedModule {

}
