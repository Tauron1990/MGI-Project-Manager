// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit, OnDestroy, AfterViewInit, Input, TemplateRef, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar, MatDialog } from '@angular/material';
import { SelectionModel } from '@angular/cdk/collections';

import { AuthService } from '../../services/auth.service';
import { AlertService, MessageSeverity, DialogType } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { LocalStoreManager } from '../../services/local-store-manager.service';
import { Utilities } from '../../services/utilities';

import { AddTaskDialogComponent } from './add-task-dialog.component';

export interface ToDoTask {
    name: string;
    description: string;
    isImportant: boolean;
    isComplete: boolean;
}

@Component({
    selector: 'todo-demo',
    templateUrl: './todo-demo.component.html',
    styleUrls: ['./todo-demo.component.scss']
})
export class TodoDemoComponent implements OnInit, AfterViewInit, OnDestroy {
    public static readonly DBKeyTodoDemo = 'todo-demo.todo_list';

    displayedColumns = ['select', 'name', 'description', 'actions'];
    dataSource: MatTableDataSource<ToDoTask>;
    completedTasks: SelectionModel<ToDoTask>;

    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;

    taskEdit: ToDoTask;
    isDataLoaded = false;
    loadingIndicator = true;
    formResetToggle = true;
    _currentUserId: string;
    _hideCompletedTasks = false;

    get currentUserId() {
        if (this.authService.currentUser) {
            this._currentUserId = this.authService.currentUser.id;
        }

        return this._currentUserId;
    }

    get areAllTasksComplete(): boolean {
        return this.completedTasks.selected.length == this.dataSource.data.length;
    }

    get hideCompletedTasks() {
        return this._hideCompletedTasks;
    }
    set hideCompletedTasks(value: boolean) {
        this._hideCompletedTasks = value;
    }

    constructor(
        private alertService: AlertService,
        private translationService: AppTranslationService,
        private localStorage: LocalStoreManager,
        private authService: AuthService,
        private snackBar: MatSnackBar,
        public dialog: MatDialog
    ) {
        this.dataSource = new MatTableDataSource<ToDoTask>();
        this.completedTasks = new SelectionModel<ToDoTask>(true, []);
    }

    ngOnInit() {
        this.loadingIndicator = true;

        this.fetch((data) => {
            this.dataSource.data = data;
            this.completedTasks = new SelectionModel<ToDoTask>(true, data.filter(x => x.isComplete));
            this.isDataLoaded = true;

            setTimeout(() => { this.loadingIndicator = false; }, 1500);
        });
    }

    ngAfterViewInit() {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.dataSource.filterPredicate = (data, filter) => this.filterData(data, filter);
    }

    ngOnDestroy() {
        this.saveToDisk();
    }

    toggleCompletedTasks() {
        this.hideCompletedTasks = !this.hideCompletedTasks;
        this.refresh();
    }

    refresh() {
        // Causes the filter to refresh there by updating with recently added data.
        this.applyFilter(this.dataSource.filter);
    }

    toggleAllTasksComplete() {
        if (this.areAllTasksComplete) {
            this.completedTasks.clear();
            this.dataSource.data.forEach(task => {
                task.isComplete = false;
            });
        } else {
            this.dataSource.data.forEach(task => {
                task.isComplete = true;
                this.completedTasks.select(task);
            });
        }
    }

    toggleTaskComplete(task: ToDoTask) {
        task.isComplete = !task.isComplete;
        this.completedTasks.select(task);
    }

    applyFilter(filterValue: string) {
        if (filterValue.length < 1) {
            filterValue = ' ';
        }

        this.dataSource.filter = filterValue;
    }

    fetch(cb) {
        let data = this.getFromDisk();

        if (data == null) {
            setTimeout(() => {
                data = this.getFromDisk();

                if (data == null) {
                    data = [
                        {
                            isComplete: true,
                            isImportant: true,
                            name: 'Create visual studio extension',
                            description: 'Create a visual studio VSIX extension package that will add this project as an aspnet-core project template'
                        },
                        {
                            isComplete: false,
                            isImportant: true,
                            name: 'Do a quick how-to writeup',
                            description: ''
                        },
                        {
                            isComplete: false,
                            isImportant: false,
                            name: 'Create aspnet-core/angular7 tutorials based on this project',
                            description: 'Create tutorials (blog/video/youtube) on how to build applications (full stack)' +
                            ' using aspnet-core/angular7. The tutorial will focus on getting productive with the technology right away rather than the details on how and why they work so audience can get onboard quickly.'
                        },
                    ];
                }

                cb(data);
            }, 1000);
        } else {
            cb(data);
        }
    }

    showErrorAlert(caption: string, message: string) {
        this.alertService.showMessage(caption, message, MessageSeverity.error);
    }

    addTask() {
        const dialogRef = this.dialog.open(AddTaskDialogComponent,
            {
                panelClass: 'mat-dialog-sm',
            });
        dialogRef.afterClosed().subscribe(newTask => {
            if (newTask) {
                this.dataSource.data.push(newTask);
                this.refresh();
                this.saveToDisk();
            }
        });
    }

    confirmDelete(task) {
        this.snackBar.open('Delete the task?', 'DELETE', { duration: 5000 })
            .onAction().subscribe(() => {
                this.dataSource.data = this.dataSource.data.filter(item => item !== task);
                this.saveToDisk();
            });
    }

    getFromDisk() {
        return this.localStorage.getDataObject<ToDoTask[]>(`${TodoDemoComponent.DBKeyTodoDemo}:${this.currentUserId}`);
    }

    saveToDisk() {
        if (this.isDataLoaded) {
            this.localStorage.saveSyncedSessionData(this.dataSource.data, `${TodoDemoComponent.DBKeyTodoDemo}:${this.currentUserId}`);
        }
    }

    private filterData(task: ToDoTask, filter: string): boolean {
        return !(task.isComplete && this.hideCompletedTasks) && Utilities.searchArray(filter, false, task.name, task.description);
    }
}
