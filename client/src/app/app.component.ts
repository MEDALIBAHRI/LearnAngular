import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_Services/account.service';
import { PresenceService } from './_Services/presence.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  
constructor(private accountService : AccountService, private signalRService : PresenceService) {}
  ngOnInit() {
   
    this.SetCurrentUser();
  }
  SetCurrentUser()
  {
      const user : User = JSON.parse(localStorage.getItem('user'));
      if(user)
      {
        this.signalRService.createHubConnection(user);
        this.accountService.SetCurrentUser(user);
      }
      
  }
  
}
