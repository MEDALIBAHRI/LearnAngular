import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  baseUrl = environment.apiUrl;
  constructor(private httpClient : HttpClient) { }

  getUsersWithRoles()
  {
   return this.httpClient.get<Partial<User>>(this.baseUrl+"admin/users-with-roles")
  }

  updateRoles(username, roles: string[])
  {
    return this.httpClient.post(this.baseUrl+'admin/edit-roles/'+ username+'?roles='+roles,{});
  }
}
