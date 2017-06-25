import { Injectable } from "@angular/core";
import { CanActivate } from "@angular/router";
import { OAuthService } from "angular-oauth2-oidc";
import { AuthenticationService } from "./authentication.service";
@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthenticationService) {}
  canActivate() {
    if (!this.authService.isAuthorized) {
      this.authService.login();
    } else {
      return true;
    }
    return false;
  }
}
