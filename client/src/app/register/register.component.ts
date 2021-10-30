import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_Services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  
  @Output() cancelRegister = new EventEmitter();
  
  validationErrors : string[] =[];
  registerForms : FormGroup;
  maxDate : Date;
  constructor(private accountService : AccountService, 
    private fb : FormBuilder, private router : Router) { 
      this.maxDate = new Date();
      this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
    }

  ngOnInit(): void {
    this.initializeRegisterForms();
  }

  initializeRegisterForms()
  {
    this.registerForms = this.fb.group({
      gender : ['male'],
      username : ['', Validators.required],
      knownAs : ['', Validators.required],
      dateOfBirth : ['', Validators.required],
      city : ['', Validators.required],
      country : ['', Validators.required],
      password : ['',[Validators.required, Validators.minLength(4),
         Validators.maxLength(8)]],
      confirmPassword : ['', [Validators.required, 
        this.matchValues("password")]]
    });

    this.registerForms.controls.password.valueChanges.subscribe(()=>{
      this.registerForms.controls.confirmPassword.updateValueAndValidity();
    });
  }

  matchValues(matchTo :string): ValidatorFn
  {
    return (control :AbstractControl)=>{
      return control?.value === control?.parent?.controls[matchTo].value ? null:
      {isMatching : true}
    }
  }
  register()
  {
    this.accountService.Register(this.registerForms.value).subscribe(response =>
      {
        this.router.navigateByUrl('/members');
      }, error=>
      {
        this.validationErrors =(error);
      });
  }
  cancel()
  {
    this.cancelRegister.emit(false);
  }
}
