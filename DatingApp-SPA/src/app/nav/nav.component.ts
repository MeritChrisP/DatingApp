import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { NgControlStatus } from '@angular/forms';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(private authservice: AuthService) { }

  ngOnInit() {
  }

  login(){
    this.authservice.login(this.model).subscribe(next => {
        console.log('Login successful.');
      }, error => {
        console.log('Login failed.');
      }
    );
  }

  loggedIn(): boolean{

    // The '!!' is shorthand and implies a boolean value to be returned based on the presence of a value in the variable 'token'.
    // Return 'true' if 'token' contains a value and 'false' if not.
    const token = localStorage.getItem('token');
    return !!token;
  }

  logout(): void {
    localStorage.removeItem('token');
    console.log('Logged out successfully.');
  }

}
