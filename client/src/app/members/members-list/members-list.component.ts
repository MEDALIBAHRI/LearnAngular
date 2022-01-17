import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { UserParams } from 'src/app/_models/userParams';
import { Pagination } from 'src/app/_modules/pagination';
import { AccountService } from 'src/app/_Services/account.service';
import { MembersService } from 'src/app/_Services/members.service';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {
 
 
  members : Member[];
  user : any;
  pagination : Pagination;
  userParams : UserParams;
  genderList =[{value :'male' ,display : 'male'},{ value :'female', display : 'female'}]
  sortList =[{value :'created' ,display : 'Last created'},
             { value :'lastActive', display : 'Last activated'}]
  constructor(private memberServices : MembersService) {
    this.userParams = this.memberServices.userParams;
   }

  ngOnInit(): void {
    this.loadMembers();
    
  }

  loadMembers()
  {
    this.memberServices.setUserParams(this.userParams);
    console.log(this.userParams);
    this.memberServices.getMembers(this.userParams).subscribe(response=>{
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }
  
  pageChanged(event : any)
  {
    
    this.userParams.pageNumber = event.page;
    this.memberServices.setUserParams(this.userParams);
    this.loadMembers();
  }
  resetFilter()
  {
    this.userParams = this.memberServices.resetUserParams();
    this.loadMembers();
  }
}
