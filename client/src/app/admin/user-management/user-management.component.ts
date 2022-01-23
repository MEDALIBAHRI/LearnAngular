import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ModalRolesComponent } from 'src/app/modals/modal-roles/modal-roles.component';
import { User } from 'src/app/_models/user';
import { MembersService } from 'src/app/_Services/members.service';
import { UsersService } from 'src/app/_Services/users.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users : Partial<User>;
  bsModalRef : BsModalRef;
  constructor(private usersService : UsersService, private modalService : BsModalService) { }

  ngOnInit(): void {
   this.usersService.getUsersWithRoles().subscribe(response => {
     this.users = response;
   });
  }
 openRolesModal(user : User)
 {
   const config ={
    class : 'modal-dialog-centered',
    initialState: {
      user,
      roles : this.getRolesArray(user)
    }
   };
  
  this.bsModalRef = this.modalService.show(ModalRolesComponent, config);
  this.bsModalRef.content.updateSelectedRoles.subscribe(values =>{
    const rolesToUpdate ={
      roles : [...values.filter(el =>el.checked === true).map(el => el.name)]
    };
    if(rolesToUpdate)
    {
      this.usersService.updateRoles(user.username, rolesToUpdate.roles).subscribe(()=>{
        user.roles = [...rolesToUpdate.roles];
      });
    }
  });
  
 }

 getRolesArray(user)
 {
   const roles =[];
   const userRoles = user.roles;
   const availableRoles: any[]=[
     {name : 'Admin', value :'admin'},
     {name : 'Moderator', value :'moderator'},
     {name : 'Member', value :'member'}
   ];
   availableRoles.forEach(role=>{
    role.checked = userRoles.includes(role.name);
     roles.push(role);
   });

   return roles;
 }
}
