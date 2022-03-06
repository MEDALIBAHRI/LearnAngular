import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import {HttpClientModule} from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from './_Services/presence.service';
import { Router } from '@angular/router';
describe('AppComponent', () => {
  let presenceService : PresenceService;
  let toastrService : ToastrService;
  let rooter : Router;
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        HttpClientModule,
        
      ],
      declarations: [
        AppComponent
      ],
      providers: [PresenceService,{ provide: ToastrService, useValue: toastrService }, { provide: Router, useValue: rooter }]
    }).compileComponents();
    
  });

   it('should create the app', () => {
     const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy(); 
   
  });
 
 it(`should have as title 'The Dating App'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('The Dating App');
  });

  /*  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('.content span')?.textContent).toContain('client app is running!');
  });   */
});
