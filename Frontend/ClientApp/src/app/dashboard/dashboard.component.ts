import { Component, OnInit } from '@angular/core'
import { HubConnectionBuilder } from '@microsoft/signalr'
import { HubConnection } from '@microsoft/signalr/dist/esm/HubConnection'
import { LogLevel } from '@microsoft/signalr/dist/esm/ILogger'
import { HttpTransportType } from '@microsoft/signalr/dist/esm/ITransport'
import * as _ from 'lodash'
import { IOrder } from './IOrder'

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: [
    "./dashboard.component.scss"
  ]
})
export class DashboardComponent implements OnInit {
  connection?: HubConnection
  receivedOrders: IOrder[] = []
  columnsToDisplay: string[] = ["orderNumber", "customer", "product", "description", "price"]

  ngOnInit(): void {
    this.connection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Debug)
      .withUrl("https://localhost:7180/hubs/notification", {
        transport: HttpTransportType.WebSockets,
        skipNegotiation: true
      })
      .build()

    this.connection.on("order_recieved_event", (payload) => {
      console.log(payload)
      this.receivedOrders.push(payload)
      console.log(this.receivedOrders)
    })

    this.connection.on("order_status_changed_event", (payload) => {
      const found = _.find(this.receivedOrders, order => order.orderNumber === payload.orderNumber)
      if (found) {
        found.status = payload.status
      }
    })

    this.connection.start()
      .then(() => {
        console.log('Connect SignalR success!')
      })
      .catch(err => {
        console.log(err)
      })
  }

}
