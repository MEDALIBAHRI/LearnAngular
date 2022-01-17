import { Component, OnInit } from '@angular/core';
import { Messages } from '../_models/message';
import { Pagination } from '../_modules/pagination';
import { MessageService } from '../_Services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  container : string ="Unread";
  pageNumber = 1;
  pageSize = 5;
  messages : Messages[];
  pagination : Pagination;
  loading : boolean = false;
  constructor(private messageService : MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }
  loadMessages()
  {
    
    this.loading = true;
      this.messageService.getMessages(this.pageNumber, this.pageSize, this.container)
          .subscribe(response => {
              this.messages = response.result;
              this.pagination = response.pagination;
              console.log(response.result);
              this.loading = false;
          });
  }

  pageChanged(event)
  {
    if(this.pageNumber !== event)
    {
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

  deleteMessage(id)
  {
    debugger;
    this.messageService.deleteMessage(id).subscribe(x=>{
      debugger;
      this.messages.splice(this.messages.findIndex(x=>x.id == id), 1);
    });
  }
}
