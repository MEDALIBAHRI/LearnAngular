import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Messages } from 'src/app/_models/message';
import { MessageService } from 'src/app/_Services/message.service';

@Component({
  selector: 'app-member-message',
  templateUrl: './member-message.component.html',
  styleUrls: ['./member-message.component.css']
})
export class MemberMessageComponent implements OnInit {
    @ViewChild('messageForm') messageForm : NgForm;
    @Input() messages : Messages[];
    @Input() username : string;
    content : string;
  constructor(private messageService : MessageService) { }

  ngOnInit(): void {
    
  }
 
  sendMessage()
  {
    this.messageService.sendMessage(this.username, this.content).subscribe(message=>{
        this.messages.push(message);
        this.messageForm.reset();
    });
  }

}
