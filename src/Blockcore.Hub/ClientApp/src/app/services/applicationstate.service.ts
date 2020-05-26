import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Injectable({
   providedIn: 'root'
})
export class ApplicationState {

   title: string;

   constructor() {

   }

}
