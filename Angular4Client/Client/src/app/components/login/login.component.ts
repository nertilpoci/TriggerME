import { Component, OnInit } from "@angular/core";
import { Http } from "@angular/http";
import { Client } from "../../shared/models";
import { SignalR, SignalRConnection } from "ng2-signalr";
import { SignalRModule, BroadcastEventListener } from "ng2-signalr";
import { IConnectionOptions } from "ng2-signalr";
import { ClientService, HubService, NotificationService, AuthenticationService } from "../../shared/services";
import { OAuthService } from "angular-oauth2-oidc";

import { ButtonModule, PanelMenuModule, ConfirmationService, MenuItem, DataTable, DialogModule, Message } from "primeng/primeng";
import { ActivatedRoute } from "@angular/router";
@Component({
  selector: "login",
  template: ""
})
export class LoginComponent implements OnInit {
  constructor(public authService: AuthenticationService) {}
  ngOnInit() {
    this.authService.login();
  }
}
