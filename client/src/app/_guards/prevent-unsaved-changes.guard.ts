import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanDeactivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_Services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {
  constructor(private confirmService: ConfirmService) { }
  canDeactivate(component: MemberEditComponent): boolean | Observable<boolean>  {
    console.log(component.editForm);
    if(component.editForm.dirty)
    {
      return this.confirmService.confirm();
    }
    return true;
  }
  
}
