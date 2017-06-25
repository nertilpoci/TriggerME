import { Component, OnInit } from "@angular/core";
import { Http, Headers, RequestOptions } from "@angular/http";
import { Client, TriggerMessage } from "../../shared/models";
import { ButtonModule, PanelMenuModule, ConfirmationService, MenuItem, DataTable, DialogModule, Message } from "primeng/primeng";
import { ActivatedRoute } from "@angular/router";
import { TriggerService, LoadingService, ClientService, HubService, NotificationService } from "../../shared/services";
import { Location } from "@angular/common";
import { Inject } from "@angular/core";
import { OAuthService } from "angular-oauth2-oidc";
import { APP_CONFIG, AppConfig } from "../../shared/app.config";
import { fadeInAnimation } from "../../shared/animation/slideIn.animation";
@Component({
  selector: "triggers",
  templateUrl: "./trigger.component.html",
  animations: [fadeInAnimation],
  host: { "[@fadeInAnimation]": "" }
})
export class TriggersComponent implements OnInit {
  displayDialog: boolean;
  triggerMessage: TriggerMessage = new TriggerMessage();
  selectedTriggerMessage: TriggerMessage;
  newTriggerMessage: boolean;
  triggerMessages: TriggerMessage[];
  clients: Client[];
  currentClient: Client;
  clientId: number;

  constructor(@Inject(APP_CONFIG) public config: AppConfig, private oauthService: OAuthService, private http: Http, private notificationService: NotificationService, private triggerService: TriggerService, private hubService: HubService, private clientService: ClientService, activatedRoute: ActivatedRoute, private confirmationService: ConfirmationService, private location: Location, private loadingService: LoadingService) {
    this.clientId = activatedRoute.snapshot.params["id"];
  }
  ngOnInit() {
    if (this.clientId) {
      this.clientService.getClients().subscribe(
        clients => {
          this.clients = clients;
          this.currentClient = this.clients.find(x => x.id == this.clientId);
        },
        error => {
          this.notificationService.showError("", "Failed to load clients");
        },
        () => {
          if (this.currentClient) {
            let action = this.triggerService.getTriggers(this.currentClient.id).subscribe(
              triggers => {
                this.triggerMessages = triggers;
              },
              error => {
                this.notificationService.showError("", "Failed to load triggers");
              }
            );
            this.loadingService.loader(action);
          } else {
            this.notificationService.showError("", "This client does not exist");
          }
        }
      );
    } else {
      this.clientService.getClients().subscribe(
        clients => {
          this.clients = clients;
          this.currentClient = this.clients[0];
          this.clientId = this.currentClient.id;
        },
        error => {
          this.notificationService.showError("", "Failed to get clients");
        },
        () => {
          if (this.currentClient) {
            let action = this.triggerService.getTriggers(this.currentClient.id).subscribe(
              triggers => {
                this.triggerMessages = triggers;
              },
              error => {
                this.notificationService.showError("", "Failed to load triggers");
              }
            );
            this.loadingService.loader(action);
          }
        }
      );
    }

    let action = this.hubService.sub("clientUpdated").then(subscriber => {
      subscriber.subscribe((client: Client) => {
        var cl = this.clients.find(x => x.identifier == client.identifier);
        cl.isOnline = client.isOnline;
      });
    });
  }
  
  showDialogToAdd() {
    this.newTriggerMessage = true;
    this.triggerMessage = new TriggerMessage();
    this.displayDialog = true;
  }

  save() {
    this.triggerMessage.clientId = this.currentClient.id;

    let action = this.triggerService.create(this.triggerMessage).subscribe(
      trigger => {
        this.triggerMessages = [...this.triggerMessages, trigger];
        this.notificationService.showSuccess(trigger.name, "Added Successfully");
      },
      error => this.notificationService.showError("", "Error occured during save. Please try again")
    );
    this.loadingService.loader(action, "Working on creating trigger. Please wait..");
    this.triggerMessage = null;
    this.displayDialog = false;
  }
  viewTriggerUrl(trigger: TriggerMessage) {
    this.confirmationService.confirm({
      message: `${this.config.serverUrl}/api/sendtrigger/${this.currentClient.identifier}/${trigger.secret}`,
      header: "Trigger Url",
      acceptVisible: false,
      rejectVisible: false
    });
  }
  trigger(message: TriggerMessage) {
    let action = this.http.post(`${this.config.serverUrl}/api/sendtrigger/${this.currentClient.identifier}/${message.secret}`, message).subscribe(result => {});
    this.loadingService.loader(action, "Triggering");
  }

  delete(trigger: TriggerMessage) {
    let action = this.triggerService.delete(trigger.id).subscribe(
      trigger => {
        var cl = this.triggerMessages.find(x => x.id == trigger.id);
        var index = this.triggerMessages.indexOf(cl);
        this.triggerMessages.splice(index, 1);
        this.triggerMessages = [...this.triggerMessages];
        this.notificationService.showSuccess(trigger.name, "Removed Successfully");
      },
      error => this.notificationService.showError("", "Failed to delete trigger")
    );
    this.loadingService.loader(action, "Deleting trigger. Please wait..");
  }
  onEditInit(event) {}
  onEditComplete(event) {
    let updatedTrigger: TriggerMessage = <TriggerMessage>event.data;
    let action = this.triggerService.update(updatedTrigger.id, updatedTrigger).subscribe(
      trigger => {
        this.notificationService.showSuccess("Update", "Changes saved succesfully");
      },
      error => this.notificationService.showError("Update", "Error occured during update. Please try again" + error)
    );
    this.loadingService.loader(action, "Saving trigger. Please wait..");
  }

  cancel() {
    this.triggerMessage = null;
    this.displayDialog = false;
  }

  onClientChange(client: Client) {
    this.location.replaceState("/trigger/" + client.id);
    let action = this.triggerService.getTriggers(client.id).subscribe(
      triggers => {
        this.triggerMessages = triggers;
      },
      error => {
        this.notificationService.showError("", "Failed to load triggers");
      }
    );
    this.loadingService.loader(action);
  }

  findSelectedClientIndex(): number {
    return this.triggerMessages.indexOf(this.selectedTriggerMessage);
  }
  confirmDelete(message: string, trigger: TriggerMessage, acceptFunction) {
    this.confirmationService.confirm({
      message: message,
      header: "Delete Confirmation",
      icon: "fa fa-trash",
      accept: () => {
        this.delete(trigger);
      }
    });
  }
}
