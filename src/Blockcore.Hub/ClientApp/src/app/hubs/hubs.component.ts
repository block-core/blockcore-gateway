import { Component, HostBinding, OnInit, OnDestroy, NgZone } from '@angular/core';
import { ApiService } from '../services/api.service';
import { SetupService } from '../services/setup.service';
import { Message, HubService } from '../services/hub.service';

@Component({
  selector: 'app-hubs-component',
  templateUrl: './hubs.component.html'
})
export class HubsComponent implements OnInit, OnDestroy {
  // @HostBinding('class.content-centered-top') hostClass = true;

  // info: any;
  // node: any;
  // blockchain: any;
  // network: any;
  // configuration: any;
  // consensus: any;
  // peers: any;
  // blocks: any;
  // timerInfo: any;
  // timerBlocks: any;
  // errorBlocks: string;
  // errorInfo: string;
  // subscription: any;

  // title = 'ClientApp';
  // txtMessage = '';
  // uniqueID: string = new Date().getTime().toString();
  messages = new Array<Message>();
  // message = new Message();

  constructor(
    private chatService: HubService,
    private _ngZone: NgZone,
    public setup: SetupService) {

    // this.subscription = this.setup.currentChain$.subscribe(async (chain) => {
    //   await this.updateInfo();
    //   await this.updateBlocks();
    // });

    this.subscribeToEvents();
  }

  async ngOnInit() {
    // this.messages.push({ 'message': 'test' });
    // console.log('Messages2:', this.messages2);
  }

  // sendMessage(): void {
  //   if (this.txtMessage) {
  //     this.message = new Message();
  //     this.message.clientuniqueid = this.uniqueID;
  //     this.message.type = 'sent';
  //     this.message.message = this.txtMessage;
  //     this.message.date = new Date();
  //     this.messages.push(this.message);
  //     this.chatService.sendMessage(this.message);
  //     this.txtMessage = '';
  //   }
  // }

  private subscribeToEvents(): void {

    this.chatService.eventReceived.subscribe((message: Message) => {
      this._ngZone.run(() => {

        console.log('MESSAGE FROM HUB: ', message);

        // if (message.clientuniqueid !== this.uniqueID) {
        //   message.type = 'received';
        this.messages.push(message);
        // }

        console.log(this.messages);

      });
    });

  }

  ngOnDestroy(): void {
    // clearTimeout(this.timerInfo);
    // clearTimeout(this.timerBlocks);
    // this.subscription.unsubscribe();
  }

  // async updateBlocks() {
  //   try {
  //     const list = await this.api.getBlocks(0, 5);

  //     // When the offset is not set (0), we should reverse the order of items.
  //     list.sort((b, a) => {
  //       if (a.blockIndex === b.blockIndex) {
  //         return 0;
  //       }
  //       if (a.blockIndex < b.blockIndex) {
  //         return -1;
  //       }
  //       if (a.blockIndex > b.blockIndex) {
  //         return 1;
  //       }
  //     });

  //     this.blocks = list;
  //     this.errorBlocks = null;
  //   } catch (error) {
  //     this.errorBlocks = error;
  //   }

  //   this.timerBlocks = setTimeout(async () => {
  //     await this.updateBlocks();
  //   }, 15000);
  // }

  // async updateInfo() {
  //   try {
  //     this.info = await this.api.getInfo();

  //     this.node = this.info.node;
  //     this.blockchain = this.node.blockchain;
  //     this.network = this.node.network;
  //     this.configuration = this.info.configuration;
  //     this.consensus = this.configuration?.consensus;
  //     this.errorInfo = null;
  //   } catch (error) {
  //     this.errorInfo = error;
  //   }

  //   this.timerInfo = setTimeout(async () => {
  //     await this.updateInfo();
  //   }, 15000);
  // }
}

