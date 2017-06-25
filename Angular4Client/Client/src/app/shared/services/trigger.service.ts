import { Injectable, Inject } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { AuthenticationService } from "./authentication.service";
import { TriggerMessage } from "../models/TriggerMessage";
import { Headers, RequestOptions } from "@angular/http";
import { ServiceBase } from "../Impl/base.service";
import { APP_CONFIG, AppConfig } from "../app.config";
@Injectable()
export class TriggerService extends ServiceBase {
  private triggerUrl = "";

  constructor(private http: Http, public authService: AuthenticationService, @Inject(APP_CONFIG) public config: AppConfig) {
    super(authService, config);
    this.triggerUrl = this.apiUrl + "/triggers/";
  }
  create(client: TriggerMessage): Observable<TriggerMessage> {
    return this.http.post(this.triggerUrl, client, this.options).map(this.extractData).catch(this.handleError);
  }
  update(id: number, trigger: TriggerMessage): Observable<TriggerMessage> {
    return this.http.put(this.triggerUrl + id, trigger, this.options).map(this.extractData).catch(this.handleError);
  }
  delete(id: number): Observable<TriggerMessage> {
    return this.http.delete(this.triggerUrl + id, this.options).map(this.extractData).catch(this.handleError);
  }
  getTriggers(id: number): Observable<TriggerMessage[]> {
    console.log(this.options);
    console.log(id);
    return this.http.get(this.triggerUrl + id, this.options).map(this.extractData).catch(this.handleError);
  }
  private extractData(res: Response) {
    let body = res.json();
    return body;
  }
}
