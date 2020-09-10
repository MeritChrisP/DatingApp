import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  // @Output allows values to be sent back to the calling/parent using 'EventEmitter'.
  @Output() cancelRegister = new EventEmitter();

  model: any = {};

  constructor(private authService: AuthService) {

  }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('Registration successful.');
    }, error => {
      console.log(error);
    });
  }
  cancel() {
    this.cancelRegister.emit(false);
    console.log('Cancelled.');
  }
}
