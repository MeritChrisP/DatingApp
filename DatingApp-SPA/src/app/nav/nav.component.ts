import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { NgControlStatus } from '@angular/forms';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login(){
    this.authService.login(this.model).subscribe(next => {
        this.alertify.success('Login successful.');
      }, error => {
        this.alertify.error('Login failed.');
      }
    );
  }

  loggedIn(): boolean{
    return this.authService.loggedIn();
  }

  logout(): void {
    localStorage.removeItem('token');
    this.alertify.message('Logged out successfully.');
  }

}
