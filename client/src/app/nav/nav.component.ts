import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
model :any={}
user : User;
  constructor(public accountService : AccountService, private router : Router, 
    private toastr : ToastrService) { 
      this.accountService.currentUser$.pipe(take(1)).subscribe(user=>this.user = user);
    }

  ngOnInit(): void {
    
  }
 Login(){
   this.accountService.Login(this.model).subscribe(response =>
    {
     this.router.navigateByUrl('/members');
     
    }, error=>{
      console.log(error);
      //this.toastr.error(error.error);
    });
    
 }
 Logout()
 {
   this.accountService.logout();
   this.router.navigateByUrl('/');
 }

}
