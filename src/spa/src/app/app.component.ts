import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { AppService } from './app.service';
import { WsService } from './ws.service';

@Component({
  selector: 'app-root',
  template: `
  <ul>
  <li *ngFor="let item of items$ | async">
    {{item | json}}
  </li>
  </ul>
  `,
  styles: []
})
export class AppComponent {
  
  items$: Observable<any[]>;
  
  constructor(
    appService:AppService, 
    wsService: WsService) {
    this.items$ = appService.WeatherForecast();
    wsService.StartConnection();
    wsService.RegisterOnServerEvents();

  }
}
