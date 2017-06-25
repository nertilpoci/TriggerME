import { Injectable, Inject } from "@angular/core";
import { Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/map";
import { Client } from "../models";
import { Headers, RequestOptions } from "@angular/http";
import { AuthenticationService } from "./authentication.service";
import { ServiceBase } from "../Impl/base.service";
import { APP_CONFIG, AppConfig } from "../app.config";
@Injectable()
export class ClientService extends ServiceBase {
  private clientsUrl = "";

  constructor(private http: Http, protected authService: AuthenticationService, @Inject(APP_CONFIG) protected config: AppConfig) {
    super(authService, config);
    this.clientsUrl = this.apiUrl + "/Clients/";
  }

  create(client: Client): Observable<Client> {
    return this.http.post(this.clientsUrl, client, this.options).map(this.extractData).catch((err: any) => {
      return this.handleError(err);
    });
  }
  single(id: number): Observable<Client> {
    return this.http.get(this.clientsUrl + id, this.options).map(this.extractData).catch((err: any) => {
      return this.handleError(err);
    });
  }
  update(id: number, client: Client): Observable<Client> {
    return this.http.put(this.clientsUrl + id, client, this.options).map(this.extractData).catch((err: any) => {
      return this.handleError(err);
    });
  }
  delete(id: number): Observable<Client> {
    return this.http.delete(this.clientsUrl + id, this.options).map(this.extractData).catch((err: any) => {
      return this.handleError(err);
    });
  }

  getClients(): Observable<Client[]> {
    return this.http.get(this.clientsUrl, this.options).map(this.extractData).catch((err: any) => {
      return this.handleError(err);
    });
  }
  private extractData(res: Response) {
    let body = res.json();

    return body;
  }
}
