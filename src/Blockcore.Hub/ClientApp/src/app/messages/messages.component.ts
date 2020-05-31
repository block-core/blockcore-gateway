import { Component, ViewChild } from '@angular/core';
import { map } from 'rxjs/operators';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { ApplicationState } from '../services/applicationstate.service';
import { HubService } from '../services/hub.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent {

  message: string;
  messages = [];

  sendMessage() {
    const msg = { self: true, clientuniqueid: '', date: new Date(), type: 'Broadcast', content: this.message};

    this.hub.sendMessage(msg);
    this.hub.sendMessageToHubs(msg);
    this.messages.push(msg);

    this.message = '';
  }

  constructor(private breakpointObserver: BreakpointObserver, private appState: ApplicationState, private hub: HubService) {
    appState.title = 'Messages';
  }
}
