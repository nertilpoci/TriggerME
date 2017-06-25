import { Component, OnInit } from "@angular/core";
import { Http } from "@angular/http";
import { Client } from "../../shared/models";
import { SignalR, SignalRConnection } from "ng2-signalr";
import { SignalRModule, BroadcastEventListener } from "ng2-signalr";
import { IConnectionOptions } from "ng2-signalr";
import { ClientService, HubService, NotificationService, LoadingService } from "../../shared/services";
import { ButtonModule, PanelMenuModule, ConfirmationService, MenuItem, DataTable, DialogModule, Message } from "primeng/primeng";
import { fadeInAnimation } from "../../shared/animation/slideIn.animation";
import { ActivatedRoute } from "@angular/router";
@Component({
  selector: "clients",
  templateUrl: "./client.component.html",
  styleUrls:['./client.component.css'],
  animations: [fadeInAnimation],
  host: { "[@fadeInAnimation]": "" }
})
export class ClientsComponent implements OnInit {
  displayDialog: boolean;
  client: Client = new Client();
  selectedClient: Client;
  newClient: boolean;
  private channel = "clients";
  clients: Client[] = [];
  connection: any;

  constructor(private loadingService: LoadingService, private clientService: ClientService, private notificationService: NotificationService, private route: ActivatedRoute, private confirmationService: ConfirmationService, private channelService: HubService) {}

  ngOnInit() {
    let action = this.clientService.getClients().subscribe(
      clients => {
        this.clients = clients;
      },
      error => {
        this.notificationService.showError("", "Failed to Load clients" + error);
      }
    );
    this.loadingService.loader(action);
    this.connection = this.route.snapshot.data["connection"];

    let action2 = this.channelService.sub("clientUpdated").then(subscriber => {
      subscriber.subscribe((client: Client) => {
        var cl = this.clients.find(x => x.identifier == client.identifier);
        cl.isOnline = client.isOnline;
      });
    });
    this.loadingService.loader(action);
  }

  showDialogToAdd() {
    this.newClient = true;
    this.client = new Client();
    this.displayDialog = true;
  }

  save() {
    let action = this.clientService.create(this.client).subscribe(
      client => {
        this.clients.push(client);
        this.clients = [...this.clients];
        this.notificationService.showSuccess(client.name, "Added Successfully");
      },
      error => this.notificationService.showError("", "Failed to create client")
    );
    this.client = null;
    this.displayDialog = false;
  }

  delete(client: Client) {
    let action = this.clientService.delete(client.id).subscribe(
      client => {
        var cl = this.clients.find(x => x.identifier == client.identifier);
        var index = this.clients.indexOf(cl);
        this.clients.splice(index, 1);
        this.clients = [...this.clients];
        this.notificationService.showSuccess(client.name, "Removed Successfully");
      },
      error => this.notificationService.showError("", "Failed to delete client")
    );
    this.loadingService.loader(action, "Deleting client. Please wait..");
  }
  onEditInit(event) {}
  onEditComplete(event) {
    let updatedClient: Client = <Client>event.data;
    let action = this.clientService.update(updatedClient.id, updatedClient).subscribe(
      client => {
        this.notificationService.showSuccess("Update", "Changes saved succesfully");
      },
      error => this.notificationService.showError("Update", "Error occured during update. Please try again")
    );
    this.loadingService.loader(action, "Saving client. Please wait..");
  }

  cancel() {
    this.client = null;
    this.displayDialog = false;
  }

  onRowSelect(event) {
    this.newClient = false;
    this.client = this.cloneClient(event.data);
    this.displayDialog = true;
  }

  cloneClient(c: Client): Client {
    let client = new Client();
    for (let prop in c) {
      client[prop] = c[prop];
    }
    return client;
  }

  findSelectedClientIndex(): number {
    return this.clients.indexOf(this.selectedClient);
  }
  confirmDelete(message: string, client: Client) {
    this.confirmationService.confirm({
      message: message,
      header: "Delete Confirmation",
      icon: "fa fa-trash",
      accept: () => {
        this.delete(client);
      }
    });
  }
}
