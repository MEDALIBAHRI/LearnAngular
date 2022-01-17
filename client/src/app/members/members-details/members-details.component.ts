import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Messages } from 'src/app/_models/message';
import { MembersService } from 'src/app/_Services/members.service';
import { MessageService } from 'src/app/_Services/message.service';

@Component({
  selector: 'app-members-details',
  templateUrl: './members-details.component.html',
  styleUrls: ['./members-details.component.css']
})
export class MembersDeailsComponent implements OnInit {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  activeTab : TabDirective;
  member : Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  messages : Messages[] = [];
  
  constructor(private memberService : MembersService, private route : ActivatedRoute,
    private messageService : MessageService) { }

  ngOnInit(): void {
   
    this.route.data.subscribe(data=>{
      this.member = data.member;
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
        this.loadMessages();
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
}
