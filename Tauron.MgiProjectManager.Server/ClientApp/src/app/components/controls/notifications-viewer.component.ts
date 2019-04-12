// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit, OnDestroy, TemplateRef, ViewChild, Input } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar, MatDialog } from '@angular/material';

import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { AppTranslationService } from '../../services/app-translation.service';
import { NotificationService } from '../../services/notification.service';
import { AccountService } from '../../services/account.service';
import { Permission } from '../../models/permission.model';
import { Utilities } from '../../services/utilities';
import { Notification } from '../../models/notification.model';

@Component({
  selector: 'notifications-viewer',
  templateUrl: './notifications-viewer.component.html',
  styleUrls: ['./notifications-viewer.component.scss']
})
export class NotificationsViewerComponent implements OnInit, OnDestroy {
  displayedColumns = ['date', 'header', 'actions'];
  dataSource: MatTableDataSource<Notification>;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  loadingIndicator: boolean;

  dataLoadingConsecutiveFailures = 0;
  dataLoadingSubscription: any;

  constructor(
    private alertService: AlertService,
    private translationService: AppTranslationService,
    private accountService: AccountService,
    private notificationService: NotificationService,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Notification>();
  }

  ngOnInit() {
    this.initDataLoading();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  ngOnDestroy() {
    if (this.dataLoadingSubscription) {
      this.dataLoadingSubscription.unsubscribe();
    }
  }

  initDataLoading() {
    this.loadingIndicator = true;

    const dataLoadTask = this.notificationService.getNewNotificationsPeriodically();

    this.dataLoadingSubscription = dataLoadTask
      .subscribe(notifications => {
        this.loadingIndicator = false;
        this.dataLoadingConsecutiveFailures = 0;

        this.dataSource.data = notifications;
      },
        error => {
          this.loadingIndicator = false;

          this.alertService.showMessage('Load Error', 'Loading new notifications from the server failed!', MessageSeverity.warn);
          this.alertService.logError(error);

          if (this.dataLoadingConsecutiveFailures++ < 5) {
            setTimeout(() => this.initDataLoading(), 5000);
          } else {
            this.alertService.showStickyMessage('Load Error', 'Loading new notifications from the server failed!', MessageSeverity.error);
          }
        });
  }

  getPrintedDate(value: Date) {
    if (value) {
      return Utilities.printTimeOnly(value) + ' on ' + Utilities.printDateOnly(value);
    }
  }

  confirmDelete(notification: Notification) {
    this.snackBar.open(`Delete ${notification.header}?`, 'DELETE', { duration: 5000 })
      .onAction().subscribe(() => {
        this.alertService.startLoadingMessage('Deleting...');
        this.loadingIndicator = true;

        this.notificationService.deleteNotification(notification)
          .subscribe(results => {
            this.alertService.stopLoadingMessage();
            this.loadingIndicator = false;

            this.dataSource.data = this.dataSource.data.filter(item => item.id != notification.id);
          },
            error => {
              this.alertService.stopLoadingMessage();
              this.loadingIndicator = false;

              this.alertService.showStickyMessage('Delete Error', `An error occured whilst deleting the notification.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
                MessageSeverity.error, error);
            });
      });
  }

  togglePin(row: Notification) {
    const pin = !row.isPinned;
    const opText = pin ? 'Pinning' : 'Unpinning';

    this.alertService.startLoadingMessage(opText + '...');
    this.loadingIndicator = true;

    this.notificationService.pinUnpinNotification(row, pin)
      .subscribe(results => {
        this.alertService.stopLoadingMessage();
        this.loadingIndicator = false;

        row.isPinned = pin;
      },
        error => {
          this.alertService.stopLoadingMessage();
          this.loadingIndicator = false;

          this.alertService.showStickyMessage(opText + ' Error', `An error occured whilst ${opText} the notification.\r\nError: "${Utilities.getHttpResponseMessages(error)}"`,
            MessageSeverity.error, error);
        });
  }

  get canManageNotifications() {
    return this.accountService.userHasPermission(Permission.manageRolesPermission);
  }
}
