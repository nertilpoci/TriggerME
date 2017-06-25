import { BrowserModule, Title } from "@angular/platform-browser";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpModule } from "@angular/http";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { RouterModule } from "@angular/router";
import { AppComponent } from "./app.component";
import { LayoutComponent, DashboardComponent, TriggersComponent, ClientsComponent, LoginComponent, WelcomeComponent } from "./components";
import { HeaderComponent, SidebarComponent } from "./shared/components";
import { NgbDropdownModule, NgbCarouselModule, NgbTooltipModule } from "@ng-bootstrap/ng-bootstrap";
import { ClientService, LoadingService, AuthenticationService, TriggerService, HubService, AuthGuard, NotificationService } from "./shared/services";
import { ScrollToModule } from "ng2-scroll-to";
import { BusyModule, BusyConfig } from "angular2-busy";
import { DataTableModule, DialogModule, ConfirmDialogModule, InplaceModule, GrowlModule, ConfirmationService } from "primeng/primeng";
import { SignalRModule, SignalRConfiguration } from "ng2-signalr";
import { OAuthModule, OAuthService } from "angular-oauth2-oidc";
import { APP_CONFIG, APP_DI_CONFIG } from "./shared/app.config";
export function createConfig(): SignalRConfiguration {
  const c = new SignalRConfiguration();
  return c;
}
export function busyConfig(): BusyConfig{
  const c= new BusyConfig({
  message: "Loading Data. Please wait",
  backdrop: false,
  template: '<div class="alert alert-info" role="alert"><strong> {{message}}</strong></div>',
  delay: 200,
  minDuration: 600
});
return c;
}

@NgModule({
  declarations: [AppComponent, LayoutComponent, HeaderComponent, SidebarComponent, ClientsComponent, TriggersComponent, LoginComponent, WelcomeComponent, DashboardComponent],
  imports: [
    BrowserModule,
    FormsModule,
    HttpModule,
    DataTableModule,
    ConfirmDialogModule,
    DialogModule,
    InplaceModule,
    GrowlModule,
    BrowserAnimationsModule,
    ScrollToModule.forRoot(),
    NgbTooltipModule.forRoot(),
    RouterModule.forRoot([
      { path: "", redirectTo: "welcome", pathMatch: "full", data: { title: "TriggerME-Home" } },
      { path: "login", component: LoginComponent },
      { path: "welcome", component: WelcomeComponent, data: { title: "TriggerME-Home" } },
      {
        path: "dashboard",
        component: DashboardComponent,
        canActivate: [AuthGuard],
        data: { title: "TriggerME-Dashboard" },
        children: [{ path: "", redirectTo: "clients", pathMatch: "full", data: { title: "TriggerME-Clients" } }, { path: "clients", component: ClientsComponent, canActivate: [AuthGuard], data: { title: "TriggerME-Clients" } }, { path: "triggers", component: TriggersComponent, canActivate: [AuthGuard], data: { title: "TriggerME-Triggers" } }, { path: "triggers/:id", component: TriggersComponent, canActivate: [AuthGuard], data: { title: "TriggerME-Trigger" } }]
      },
      { path: "**", redirectTo: "welcome", data: { title: "TriggerME-Home" } }
    ]),
    NgbDropdownModule.forRoot(),
    NgbCarouselModule.forRoot(),
    SignalRModule.forRoot(createConfig),
    OAuthModule.forRoot(),
    BusyModule
  ],
  providers: [{ provide: APP_CONFIG, useValue: APP_DI_CONFIG }, Title, LoadingService, ClientService, OAuthService, AuthenticationService, AuthGuard, TriggerService, ConfirmationService, HubService, NotificationService],
  bootstrap: [AppComponent]
})
export class AppModule {}
