import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { AccountService } from 'src/app/_Services/account.service';
import { MembersService } from 'src/app/_Services/members.service';

@Component({
  selector: 'app-members-details',
  templateUrl: './members-details.component.html',
  styleUrls: ['./members-details.component.css']
})
export class MembersDeailsComponent implements OnInit {
  member : Member;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];

  
  constructor(private memberService : MembersService, private route : ActivatedRoute) { }

  ngOnInit(): void {
    this.loadMember();
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imagePercent:100,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }];
     
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
  loadMember()
  {
    this.memberService.getMember(this.route.snapshot.paramMap.get('username')).subscribe(member=>
      {
        this.member =member;
        this.galleryImages = this.getImages();
      });
  }
}
