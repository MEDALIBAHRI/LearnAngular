import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS} from '@angular/common/http'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MembersDeailsComponent } from './members/members-details/members-details.component';
import { MembersListComponent } from './members/members-list/members-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { ErrorInterceptor } from './Interceptors/error-interceptor.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtInterceptor } from './Interceptors/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './Interceptors/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputComponent } from './_forms/date-input/date-input.component';
import { MemberMessageComponent } from './members/member-message/member-message.component';
import { HasRolesDirective } from './_directives/has-roles.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { PhotosManagementComponent } from './admin/photos-management/photos-management.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { ModalRolesComponent } from './modals/modal-roles/modal-roles.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { NewsComponent } from './news/news.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MembersDeailsComponent,
    MembersListComponent,
    ListsComponent,
    MessagesComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberCardComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    TextInputComponent,
    DateInputComponent,
    MemberMessageComponent,
    HasRolesDirective,
    UserManagementComponent,
    PhotosManagementComponent,
    AdminPanelComponent,
    ModalRolesComponent,
    ConfirmDialogComponent,
    NewsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    NgxSpinnerModule,
  ],
  providers: [
    {provide : HTTP_INTERCEPTORS, useClass : ErrorInterceptor, multi : true},
    {provide : HTTP_INTERCEPTORS, useClass : JwtInterceptor, multi : true},
    {provide : HTTP_INTERCEPTORS, useClass : LoadingInterceptor, multi : true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
