import { Component, OnInit, Inject } from "@angular/core";
import { AuthenticationService } from "../../services/authentication.service";
import { APP_CONFIG, AppConfig } from "../../app.config";
@Component({
  selector: "app-header",
  templateUrl: "./header.component.html",
  styleUrls: ["./header.component.scss"]
})
export class HeaderComponent implements OnInit {
  username: string;
  desktopUrl: string;
  constructor(private authService: AuthenticationService, @Inject(APP_CONFIG) public config: AppConfig) {
    this.username = this.authService.givenName();
    this.desktopUrl = config.desktopClientUrl;
  }

  ngOnInit() {}

  toggleSidebar() {
    const dom: any = document.querySelector("body");
    dom.classList.toggle("push-right");
  }
  logout() {
    this.authService.logout();
  }
}
