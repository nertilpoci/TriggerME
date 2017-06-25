import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import { ButtonModule, PanelMenuModule, ConfirmationService, MenuItem, DataTable, DialogModule, Message } from "primeng/primeng";

@Injectable()
export class NotificationService {
  constructor() {}
  private message: BehaviorSubject<Message> = new BehaviorSubject<Message>(null);

  register(): BehaviorSubject<Message> {
    return this.message;
  }
  showError(summary: string, detail: string) {
    this.message.next({ severity: "error", summary: summary, detail: detail });
  }
  showInfo(summary: string, detail: string) {
    this.message.next({ severity: "info", summary: summary, detail: detail });
  }
  showSuccess(summary: string, detail: string) {
    this.message.next({ severity: "success", summary: summary, detail: detail });
  }
  addMessage(message: Message) {
    this.message.next(message);
  }
}
