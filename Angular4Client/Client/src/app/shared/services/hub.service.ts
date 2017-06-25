import { Injectable, Inject } from "@angular/core";
import { Subject } from "rxjs/Subject";
import { Observable } from "rxjs/Observable";
import { AuthenticationService } from "./authentication.service";
import { SignalR, ConnectionStatus, SignalRConfiguration, SignalRConnection, BroadcastEventListener, ISignalRConnection } from "ng2-signalr";
import { APP_CONFIG, AppConfig } from "../app.config";

@Injectable()
export class HubService {
  createConfig(): SignalRConfiguration {
    const c = new SignalRConfiguration();
    c.hubName = this.config.userHubName;
    (c.qs = { authorization: `${this._authService.getAccessToken()}` }), (c.url = this.config.hubEndpoint);
    c.logging = true;
    return c;
  }
  ConnectionStatus: Observable<ConnectionStatus>;

  hubConnection: SignalRConnection;

  private eventListeners = new Array<BroadcastEventListener<any>>();

  constructor(private _signalR: SignalR, private _authService: AuthenticationService, @Inject(APP_CONFIG) private config: AppConfig) {}
  private initialize(): Promise<any> {
    return this._signalR.connect(this.createConfig()).then(con => {
      this.hubConnection = <SignalRConnection>con;
      this.ConnectionStatus = con.status;
    });
  }

  sub<TReturn>(eventName: string): Promise<BroadcastEventListener<TReturn>> {
    if (!this.hubConnection) {
      return new Promise(resolve => {
        this.initialize().then(con => {
          return resolve(this.sub<TReturn>(eventName));
        });
      });
    } else {
      let onMessageSent$ = this.eventListeners.find(x => x.event == eventName);
      if (onMessageSent$) {
        this.hubConnection.listen(onMessageSent$);
        return new Promise(resolve => {
          resolve(onMessageSent$);
        });
      }
      onMessageSent$ = new BroadcastEventListener<TReturn>(eventName);

      console.log(this.hubConnection);

      this.hubConnection.listen(onMessageSent$);
      this.eventListeners.push(onMessageSent$);

      return new Promise(resolve => {
        resolve(onMessageSent$);
      });
    }
  }
}
