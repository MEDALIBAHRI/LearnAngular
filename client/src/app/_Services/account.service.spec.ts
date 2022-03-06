import { HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

import { AccountService } from './account.service';
import { PresenceService } from './presence.service';

describe('AccountService', () => {
  let toastrService : ToastrService;
   let rooter : Router;
   let accountService : AccountService;
  beforeEach(() => {TestBed.configureTestingModule({
    
    imports: [
      
      HttpClientModule
    ],
    providers: [PresenceService,
                { provide: ToastrService, useValue: toastrService },
                { provide: Router, useValue: rooter }]
  })
 accountService = TestBed.get(AccountService);;
});

  it('should be created', () => {
    const service: AccountService = TestBed.get(AccountService);
    expect(service).toBeTruthy();
  });
 /*  it('be able to retrieve posts from the API bia GET', () => {
    const dummyPosts: Post[] = [{
        userId: '1',
        id: 1,
        body: 'Http Client',
        title: 'Testing Angular Service'
        }, {
        userId: '2',
        id: 2,
        body: 'Hello World2',
        title: 'Testing Angular Services'
    }];
    service.getPost().subscribe(posts => {
        expect(posts.length).toBe(2);
        expect(posts).toEqual(dummyPosts);
    }); */
});
