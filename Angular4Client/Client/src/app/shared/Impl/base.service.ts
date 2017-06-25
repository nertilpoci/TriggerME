import { Inject } from "@angular/core";
import { Headers, RequestOptions, Response } from "@angular/http";
import { AuthenticationService } from "../services";
import { APP_CONFIG, AppConfig } from "../app.config";
import { Observable } from "rxjs/Observable";
export class ServiceBase {
  public headers = new Headers();
  public options = new RequestOptions({ headers: this.headers });
  public apiUrl: string = this.config.apiEndpoint;
  constructor(protected authService: AuthenticationService, protected config: AppConfig) {
    this.headers.append("Authorization", "Bearer " + authService.getAccessToken());
    this.options.headers = this.headers;
  }
  handleError(error: Response | any) {
    if (error.status == 401) this.authService.login();
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || "";
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.log(errMsg);
    return Observable.throw(errMsg);
  }
}
