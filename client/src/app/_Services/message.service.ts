import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  hubConnection : HubConnection;

  private messageThreadSource :BehaviorSubject<Message[]> = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http : HttpClient) { }

  getMessages(pageNumber, pageSize, container)
  {
   let params = getPaginationHeader(pageNumber, pageSize);
   params = params.append('Container', container);
   return getPaginatedResult<Message[]>(this.baseUrl + 'messages', params, this.http)
  }

  getMessagesThread(username)
  {
    return this.http.get<Message[]>(this.baseUrl+"messages/thread/"+username);
  }
  async sendMessage(username, content)
  {
    return this.hubConnection.invoke("SendMessage",{RecipientUsername : username, content})
          .catch(error => console.log(error));
  }
  deleteMessage(id)
  {
   return this.http.delete(this.baseUrl +"messages/"+ id);
  }

  createHubConnection(user : User, otherUserName : string)
  {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'messages?user=' + otherUserName, {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()
    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on("ReceiveMessageThread", messages=>{
      this.messageThreadSource.next(messages);
    });
    this.hubConnection.on("NewMessage", message=>{
      this.messageThread$.pipe(take(1)).subscribe(messages=>{
        this.messageThreadSource.next([...messages, message]);
      });
    });
   
  }
  stopHubConnection()
  {
    if(this.hubConnection)
    {
      this.hubConnection.stop();
    }
  }
}
