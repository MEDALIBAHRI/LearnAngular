import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode = false;
  constructor(public accountService :AccountService ) { }

  ngOnInit(): void {
  }
  
  registerToggle()
  {
    this.registerMode = !this.registerMode;
  }
 
   CancelRegister(event: boolean)
   {
     this.registerMode = event;
   }
}
