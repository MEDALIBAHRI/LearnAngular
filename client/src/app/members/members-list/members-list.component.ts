import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_Services/members.service';

@Component({
  selector: 'app-members-list',
  templateUrl: './members-list.component.html',
  styleUrls: ['./members-list.component.css']
})
export class MembersListComponent implements OnInit {

  members : Member[];
  user : any;
  constructor(private memberServices : MembersService) { }

  ngOnInit(): void {
    this.LoadMembers();
    
  }
  LoadMembers()
  {
    this.memberServices.getMembers().subscribe(members=>
     { this.members= members;
    });
  }

}
