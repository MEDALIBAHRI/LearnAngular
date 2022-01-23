import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-modal-roles',
  templateUrl: './modal-roles.component.html',
  styleUrls: ['./modal-roles.component.css']
})
export class ModalRolesComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user :User;
  roles : any[];
  constructor(public bsModalRef : BsModalRef) { }

  ngOnInit(): void {
    
  }

  updateRoles()
  {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
