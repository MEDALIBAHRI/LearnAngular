import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { take } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { AccountService } from 'src/app/_Services/account.service';
import { MessageService } from 'src/app/_Services/message.service';
@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css']
})
export class MemberMessageComponent implements OnInit {
    @ViewChild('messageForm') messageForm : NgForm;
    @Input() messages : Message[];
    @Input() username : string;
    content : string;
  constructor(public messageService : MessageService, public accountService : AccountService) {
    
   }

  ngOnInit(): void {
    this.messageService.messageThread$.pipe(take(1)).subscribe(messages =>{
      messages = messages;
    });
  }
 
  sendMessage()
  {
    this.messageService.sendMessage(this.username, this.content).then(message=>{
        this.messageForm.reset();
    });
  }

}
