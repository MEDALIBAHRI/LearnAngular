import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_Services/account.service';
import { MessageService } from 'src/app/_Services/message.service';
import { PresenceService } from 'src/app/_Services/presence.service';

@Component({
  selector: 'app-members-details',
  templateUrl: './members-details.component.html',
  styleUrls: ['./members-details.component.css']
})
export class MembersDeailsComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  activeTab : TabDirective;
  member : Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages : Message[] = [];
 user : User;
  constructor(private route : ActivatedRoute,
    private messageService : MessageService, public presence : PresenceService,
     private accountService: AccountService, private router : Router) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(u=>{
        this.user =u;
        this.router.routeReuseStrategy.shouldReuseRoute = ()=>false;
      });
     }
 
setOnline()
{
  
}
  ngOnInit(): void {
   
    this.route.data.subscribe(data=>{
      this.member = data.member;
      this.setOnline();
    });
    this.route.queryParams.subscribe(params =>{
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    });
    
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent:100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }];
      this.galleryImages = this.getImages();
  }
  getImages():NgxGalleryImage[]
  {
    const photos =[];
    for(const photo  of this.member.photos)
    {
      photos.push({
        small: photo?.url,
        medium: photo?.url,
        big: photo?.url,
      });
    }
    return photos;
  }

  onTabActivated(data : TabDirective)
  {
      this.activeTab = data;
      if(this.activeTab.heading === 'Messages' && this.messages.length === 0)
      {
        this.messageService.createHubConnection(this.user, this.member.username);
        
      }
      else{
        this.messageService.stopHubConnection();
      }
  }

  loadMessages()
  {
    this.messageService.getMessagesThread(this.member.username).subscribe(response =>
      {
        this.messages = response;
      });
  }

  selectTab(tabId : number)
  {
    this.memberTabs.tabs[tabId].active = true;
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
}
