import { Component, OnInit } from "@angular/core";
import { Message } from "primeng/primeng";
import { NotificationService, LoadingService } from "../../shared/services";
import { Subscription } from "rxjs";
@Component({
  selector: "app-layout",
  templateUrl: "./layout.component.html",
  styleUrls: ["./layout.component.scss"]
})
export class LayoutComponent implements OnInit {
  busy: Subscription;
  message: string;
  constructor(private notificationService: NotificationService, private loadingService: LoadingService) {}
  msgs: Message[] = [];
  sticky: boolean = false;
  ngOnInit(): void {
    this.loadingService.busy.subscribe(state => {
      if (state) this.busy = state;
    });
    this.loadingService.message.subscribe(message => {
      if (message) this.message = message;
    });
    this.notificationService.register().subscribe(message => {
      if (message) this.msgs = [...this.msgs, message];
    });
  }
}
