import { Injectable, Inject } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { APP_CONFIG, AppConfig } from "../app.config";
import { BehaviorSubject } from "rxjs/BehaviorSubject";

@Injectable()
export class AuthenticationService {
  isAuthorized: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private oauthService: OAuthService, @Inject(APP_CONFIG) public config: AppConfig) {
    // URL of the SPA to redirect the user to after login
    this.oauthService.redirectUri = config.authReturnUrl;

    // The SPA's id. The SPA is registerd with this id at the auth-server
    this.oauthService.clientId = "js";
    this.oauthService.logoutUrl = config.authLogoutUrl;
    // set the scope for the permissions the client should request
    // The first three are defined by OIDC. The 4th is a usecase-specific one
    this.oauthService.scope = "openid profile api1";

    // set to true, to receive also an id_token via OpenId Connect (OIDC) in addition to the
    // OAuth2-based access_token
    this.oauthService.oidc = true;

    // Use setStorage to use sessionStorage or another implementation of the TS-type Storage
    // instead of localStorage
    this.oauthService.setStorage(sessionStorage);

    this.oauthService.issuer = this.config.serverUrl;

    // Set a dummy secret
    // Please note that the auth-server used here demand the client to transmit a client secret, although
    // the standard explicitly cites that the password flow can also be used without it. Using a client secret
    // does not make sense for a SPA that runs in the browser. That's why the property is called dummyClientSecret
    // Using such a dummy secreat is as safe as using no secret.
    this.oauthService.dummyClientSecret = "secret";

    // Load Discovery Document and then try to login the user
    this.oauthService.loadDiscoveryDocument().then(() => {
      // This method just tries to parse the token(s) within the url when
      // the auth-server redirects the user back to the web-app
      // It dosn't send the user the the login page
      this.oauthService.tryLogin({});
      this.isAuthorized.next(this.oauthService.hasValidAccessToken());
    });
  }
  login() {
    this.oauthService.clientId = "js";

    this.oauthService.initImplicitFlow();
  }
  logout() {
    this.oauthService.logOut();
  }
  givenName() {
    var claims = this.oauthService.getIdentityClaims();

    if (!claims) return null;

    return claims.name;
  }

  getAccessToken(): string {
    return this.oauthService.getAccessToken();
  }
  getIdToken(): string {
    return this.oauthService.getIdToken();
  }
}
