// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit, OnDestroy } from '@angular/core';
import { MediaObserver, MediaChange } from '@angular/flex-layout';
import { AlertService, DialogType, AlertMessage, MessageSeverity } from '../../services/alert.service';
import { Subscription } from 'rxjs';

require('chart.js');

@Component({
  selector: 'statistics-demo',
  templateUrl: './statistics-demo.component.html',
  styleUrls: ['./statistics-demo.component.scss']
})
export class StatisticsDemoComponent implements OnInit, OnDestroy {

  chartData = [
    { data: [65, 59, 80, 81, 56, 55], label: 'Series A' },
    { data: [28, 48, 40, 19, 86, 27], label: 'Series B' },
    { data: [18, 48, 77, 9, 100, 27], label: 'Series C' }
  ];
  chartLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
  chartOptions = {
    responsive: true,
    title: {
      display: false,
      fontSize: 16,
      text: 'Important Stuff'
    }
  };
  chartColors = [
    { // grey
      backgroundColor: 'rgba(148,159,177,0.2)',
      borderColor: 'rgba(148,159,177,1)',
      pointBackgroundColor: 'rgba(148,159,177,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    },
    { // dark grey
      backgroundColor: 'rgba(77,83,96,0.2)',
      borderColor: 'rgba(77,83,96,1)',
      pointBackgroundColor: 'rgba(77,83,96,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(77,83,96,1)'
    },
    { // something else
      backgroundColor: 'rgba(128,128,128,0.2)',
      borderColor: 'rgba(128,128,128,1)',
      pointBackgroundColor: 'rgba(128,128,128,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(128,128,128,0.8)'
    }
  ];
  chartLegend = true;
  chartType = 'line';

  timerReference: any;
  mediaQuerySub: Subscription;


  constructor(private alertService: AlertService, private mediaQuery: MediaObserver) {
  }

  ngOnInit() {
    this.timerReference = setInterval(() => this.randomize(), 5000);

    this.mediaQuerySub = this.mediaQuery.media$.subscribe(change => this.chartLegend = change.mqAlias == 'xs' ? false : true);
  }

  ngOnDestroy() {
    clearInterval(this.timerReference);
    this.mediaQuerySub.unsubscribe();
  }

  randomize(): void {
    const _chartData = new Array(this.chartData.length);
    for (let i = 0; i < this.chartData.length; i++) {
      _chartData[i] = { data: new Array(this.chartData[i].data.length), label: this.chartData[i].label };

      for (let j = 0; j < this.chartData[i].data.length; j++) {
        _chartData[i].data[j] = Math.floor((Math.random() * 100) + 1);
      }
    }

    this.chartData = _chartData;
  }

  changeChartType(type: string) {
    this.chartType = type;
  }

  showMessage(msg: string): void {
    this.alertService.showMessage('Demo', msg, MessageSeverity.info);
  }

  showDialog(msg: string): void {
    this.alertService.showDialog('Configure Chart', msg, DialogType.prompt, (val) => this.configure(true, val), () => this.configure(false), 'Ok', 'Cancel', 'Default');
  }

  configure(response: boolean, value?: string) {

    if (response) {

      this.alertService.showStickyMessage('Simulating...', '', MessageSeverity.wait);

      setTimeout(() => {

        this.alertService.resetStickyMessage();
        this.alertService.showMessage('Demo', `Your settings was successfully configured to \"${value}\"`, MessageSeverity.success);
      }, 2000);
    } else {
      this.alertService.showMessage('Demo', 'Operation cancelled by user', MessageSeverity.default);
    }
  }

  chartClicked(e): void {
    console.log(e);
  }

  chartHovered(e): void {
    console.log(e);
  }
}
