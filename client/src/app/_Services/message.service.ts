import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Messages } from '../_models/message';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  constructor(private http : HttpClient) { }

  getMessages(pageNumber, pageSize, container)
  {
   let params = getPaginationHeader(pageNumber, pageSize);
   params = params.append('Container', container);
   return getPaginatedResult<Messages[]>(this.baseUrl + 'messages', params, this.http)
  }

  getMessagesThread(username)
  {
    return this.http.get<Messages[]>(this.baseUrl+"messages/thread/"+username);
  }
  sendMessage(username, content)
  {
    return this.http.post<Messages>(this.baseUrl+"messages/",{RecipientUsername : username, content});
  }
  deleteMessage(id)
  {
   return this.http.delete(this.baseUrl +"messages/"+ id);
  }
}
