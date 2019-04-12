// =============================
// Email: info@ebenmonney.com
// www.ebenmonney.com/templates
// =============================

import { Component, OnInit } from '@angular/core';
import { NguCarouselConfig } from '@ngu/carousel';

@Component({
  selector: 'banner-demo',
  templateUrl: './banner-demo.component.html',
  styleUrls: ['./banner-demo.component.scss']
})
export class BannerDemoComponent implements OnInit {
  carouselLoaded = false;
  carouselConfig: NguCarouselConfig;
  carouselItems = [
    {
      img: require('../../assets/images/demo/banner1.png'),
      alt: 'ASP.NET',
      caption: 'Learn how to build ASP.NET apps that can run anywhere',
      link: 'http://go.microsoft.com/fwlink/?LinkID=525028&clcid=0x409'
    },
    {
      img: require('../../assets/images/demo/banner2.png'),
      alt: 'Visual Studio',
      caption: 'One platform for building modern web, native mobile and native desktop applications',
      link: 'http://angular.io'
    },
    {
      img: require('../../assets/images/demo/banner3.png'),
      alt: 'Package Management',
      caption: 'Bring in libraries from NuGet and npm, and bundle with angular/cli',
      link: 'http://go.microsoft.com/fwlink/?LinkID=525029&clcid=0x409'
    },
    {
      img: require('../../assets/images/demo/banner4.png'),
      alt: 'Eben Monney',
      caption: 'Follow me on social media for updates and tips on using this startup project',
      link: 'https://www.ebenmonney.com/about'
    },
  ];

  ngOnInit() {
    this.carouselConfig = {
      grid: { xs: 1, sm: 1, md: 1, lg: 1, all: 0 },
      slide: 4,
      speed: 500,
      interval: { timing: 5000 },
      point: { visible: true },
      load: 2,
      custom: 'banner',
      touch: true,
      loop: true,
      easing: 'cubic-bezier(0, 0, 0.2, 1)'
    };
  }
}
