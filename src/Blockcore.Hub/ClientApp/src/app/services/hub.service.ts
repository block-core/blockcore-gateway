import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { EventEmitter, Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

export class HttpError extends Error {
   code: number;
   url: string;

   constructor(code: number, url: string, message?: string) {
      super(message);
      Object.setPrototypeOf(this, new.target.prototype);
      this.name = HttpError.name;
      this.url = url;
      this.code = code;
   }
}

export class Message {

   clientuniqueid: string;
   type: string;
   message: string;
   date: Date;

   constructor() {

   }
}

@Injectable({
   providedIn: 'root'
})
export class HubService {
   eventReceived = new EventEmitter<Message>();
   connectionEstablished = new EventEmitter<Boolean>();

   private connectionIsEstablished = false;
   private _hubConnection: HubConnection;

   constructor() {
      this.createConnection();
      this.registerOnServerEvents();
      this.startConnection();
   }

   sendMessage(message: Message) {
      this._hubConnection.invoke('Broadcast', message);
   }

   private createConnection() {
      this._hubConnection = new HubConnectionBuilder()
         // .withUrl(window.location.href + 'ws')
         .withUrl('http://localhost:9912/ws')
         .build();
   }

   private startConnection(): void {
      this._hubConnection
         .start()
         .then(() => {
            this.connectionIsEstablished = true;
            console.log('Hub connection started');
            this.connectionEstablished.emit(true);
         })
         .catch(err => {
            console.log('Error while establishing connection, retrying...');
            setTimeout(function () { this.startConnection(); }, 5000);
         });
   }

   private registerOnServerEvents(): void {
      this._hubConnection.on('Event', (data: any) => {
         this.eventReceived.emit(data);
      });
   }
}
