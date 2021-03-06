import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_modules/pagination';
import { MembersService } from '../_Services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

members : Partial<Member[]>;
predicate = "Liked";
pageNumber = 1;
pageSize = 2;
pagination : Pagination;
  constructor(private memberService : MembersService) { 
    
    this.pagination;
  }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes()
  {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe(response=>{
      this.members = response.result;
      this.pagination = response.pagination;
    });
  }
  pageChanged(event : any)
  {
    
    this.pageNumber = event.page;
    this.loadLikes();
  }

}
