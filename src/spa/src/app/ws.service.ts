import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";

@Injectable({
    providedIn: 'root'
})
export class WsService {

    private weUrl: string = window.location.port === '4200' ? 
                            'https://localhost:7057' : 
                            window.location.port === '9001' ?
                            'http://localhost:9003' :
                            window.location.origin;

    private hubConnection!: signalR.HubConnection;

    public StartConnection = () => {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(this.weUrl + '/notifications')
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this.hubConnection
            .start()
            .then(() => {

                this.PingAsync();
            })
            .catch(err => console.log('Error while starting connection: ' + err))
    }
    //   public MoveRequestAsync(guid: string, command: string) {
    //     this.hubConnection.invoke('MoveRequestAsync', guid, command);
    //   }

    public PingAsync() {
        console.log('Invoke PingAsync');
        this.hubConnection.invoke('PingAsync');
    }
    public RegisterOnServerEvents() {

        this.hubConnection.on(
            'PongAsync',
            (data: any) => {
                console.log('Listening PongAsync - received', data);
                // this.simpleStateService.update(data);
            });
    }
}
