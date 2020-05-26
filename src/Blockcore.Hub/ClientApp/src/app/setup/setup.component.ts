import { Component, HostBinding } from '@angular/core';
import { SetupService } from '../services/setup.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-setup',
  templateUrl: './setup.component.html',
})
export class SetupComponent {
  @HostBinding('class.content-centered') hostClass = true;

  recoveryPhrase = '';
  gateway = 'https://gateway.blockcore.net/';

  constructor(public setup: SetupService, private router: Router) {
    // When we are not in multichain mode, redirect to chain-home.
    // if (!setup.multiChain) {
    //   router.navigate(['/' + setup.current.toLowerCase()]);
    // }
  }
}
