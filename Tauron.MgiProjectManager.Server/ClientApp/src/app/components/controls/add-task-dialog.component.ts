// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, ViewChild, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, FormControl } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { AlertService, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { ToDoTask } from './todo-demo.component';

@Component({
    selector: 'app-add-task-dialog',
    templateUrl: 'add-task-dialog.component.html',
    styleUrls: ['add-task-dialog.component.scss']
})
export class AddTaskDialogComponent {
    taskForm: FormGroup;

    get taskName() {
        return this.taskForm.get('taskName');
    }

    constructor(
        public dialogRef: MatDialogRef<AddTaskDialogComponent>,
        private alertService: AlertService,
        private translationService: AppTranslationService,
        private formBuilder: FormBuilder
    ) {
        this.buildForm();
    }

    save() {
        if (this.taskForm.valid) {
            const formModel = this.taskForm.value;

            const newtask: ToDoTask = {
                name: formModel.taskName,
                description: formModel.description,
                isImportant: formModel.isImportant,
                isComplete: false
            };

            this.dialogRef.close(newtask);
        } else {
            this.alertService.showStickyMessage(this.translationService.getTranslation('form.ErrorCaption'), this.translationService.getTranslation('form.ErrorMessage'), MessageSeverity.error);
        }
    }

    cancel(): void {
        this.dialogRef.close(null);
    }

    private buildForm() {
        this.taskForm = this.formBuilder.group({
            taskName: ['', Validators.required],
            description: '',
            isImportant: ''
        });
    }
}
