import { Component, OnInit, Inject } from "@angular/core";
import { AuthenticationService } from "../../shared/services";
import { APP_CONFIG, AppConfig } from "../../shared/app.config";
import { Subscription } from "rxjs";
import { Http, Response } from "@angular/http";
import { fadeInAnimation } from "../../shared/animation/slideIn.animation";
declare var particlesJS: any;
@Component({
  selector: "welcome",
  templateUrl: "./welcome.component.html",
  styleUrls: ["./welcome.component.css"],
  // make fade in animation available to this component
  animations: [fadeInAnimation],

  // attach the fade in animation to the host (root) element of this component
  host: { "[@fadeInAnimation]": "" }
})
export class WelcomeComponent implements OnInit {
  isLoggedIn: boolean;
  desktopUrl: string;
  githubUrl: string;
  constructor(private authService: AuthenticationService, private http: Http, @Inject(APP_CONFIG) public config: AppConfig) {
    this.desktopUrl = config.desktopClientUrl;
    this.githubUrl = config.githubUrl;
  }

  ngOnInit() {
    particlesJS.load("particles-js", "assets/particles.json", null);
    this.authService.isAuthorized.subscribe(authorized => {
      this.isLoggedIn = authorized;
    });
  }
  logout() {
    this.authService.logout();
  }
}
