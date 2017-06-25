import { InjectionToken } from "@angular/core";

export let APP_CONFIG = new InjectionToken<AppConfig>("app.config");

export class AppConfig {
  apiEndpoint: string;
  hubEndpoint: string;
  serverUrl: string;
  userHubName: string;
  authReturnUrl: string;
  authLogoutUrl: string;
  desktopClientUrl: string;
  githubUrl: string;
}

export const APP_DI_CONFIG: AppConfig = {
  apiEndpoint: "http://triggerme.net/api",
  hubEndpoint: "http://triggerme.net",
  serverUrl: "http://triggerme.net",
  userHubName: "WebUserHub",

  authReturnUrl: "http://triggerme.net/welcome",
  authLogoutUrl: "http://triggerme.net/welcome",
  desktopClientUrl: "http://triggerme.net/desktop/index.html",
  githubUrl: "https://github.com/nertilpoci/TriggerME"
};
