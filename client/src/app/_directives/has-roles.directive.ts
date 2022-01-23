import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_Services/account.service';

@Directive({
  selector: '[appHasRoles]'
})
export class HasRolesDirective implements OnInit{
  @Input() appHasRoles : string[];
  user : User;
  constructor(private accountServices : AccountService,
    private viewContainerRef : ViewContainerRef,
     private templateRef : TemplateRef<any>) { 
     accountServices.currentUser$.pipe(take(1)).subscribe(u=>{
      this.user =u;
    });
  }
  ngOnInit(): void {
    if(this.user === null || !this.user.roles)
    {
      this.viewContainerRef.clear();
      return;
    }

    if(this.user.roles.some(r=> this.appHasRoles.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    }
    else{
      this.viewContainerRef.clear();
    }
  }

}
