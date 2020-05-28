import { Component, ViewChild } from '@angular/core';
import { map } from 'rxjs/operators';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { ApplicationState } from '../services/applicationstate.service';
import {MatAccordion} from '@angular/material/expansion';

export interface Section {
  name: string;
  icon: string;
  description: string;
}

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent {

  @ViewChild(MatAccordion) accordion: MatAccordion;

  /** Based on the screen size, switch from standard to one column per row */
  cards = this.breakpointObserver.observe(Breakpoints.Handset).pipe(
    map(({ matches }) => {
      if (matches) {
        return [
          { title: 'Card 1', cols: 1, rows: 1 },
          { title: 'Card 2', cols: 1, rows: 1 },
          { title: 'Card 3', cols: 1, rows: 1 },
          { title: 'Card 4', cols: 1, rows: 1 }
        ];
      }

      return [
        { title: 'Card 1', cols: 2, rows: 1 },
        { title: 'Card 2', cols: 1, rows: 1 },
        { title: 'Card 3', cols: 1, rows: 2 },
        { title: 'Card 4', cols: 1, rows: 1 }
      ];
    })
  );

  features: Section[] = [
    {
      name: 'DocumentStorage',
      icon: 'note',
      description: 'Provides storage of data entities'
    },
    {
      name: 'FileSharing',
      icon: 'note',
      description: 'Provides query and download of files'
    },
    {
      name: 'Node',
      icon: 'note',
      description: 'Provides blockchain node capabilities'
    }
  ];

  apps: Section[] = [
    {
      name: 'Casino',
      icon: 'casino',
      description: 'Basic casino running on your Hub'
    },
    {
      name: 'Grid Map',
      icon: 'map',
      description: 'Property Registry for Grid Maps'
    },
    {
      name: 'Crypto Pets',
      icon: 'pets',
      description: 'Cute cryptographic pets'
    },
    {
      name: 'Polls',
      icon: 'poll',
      description: 'Host polls and results'
    },
    {
      name: 'Casino',
      icon: 'casino',
      description: 'Basic casino running on your Hub'
    },
    {
      name: 'Grid Map',
      icon: 'map',
      description: 'Property Registry for Grid Maps'
    }
  ];

  constructor(private breakpointObserver: BreakpointObserver, private appState: ApplicationState) {
    appState.title = 'Messages';
  }
}
