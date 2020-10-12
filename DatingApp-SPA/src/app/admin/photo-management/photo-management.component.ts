import { Component, Input, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.scss']
})
export class PhotoManagementComponent implements OnInit {
  @Input() photos: Photo[];
  
  constructor(private authService: AuthService, private userService: UserService,
    private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.initialisePhotosForModeration();
  }

  initialisePhotosForModeration () {
    this.adminService.getPhotosForModeration().subscribe(photos => {
      this.photos = photos;
      //console.log(photos);
    });
  }

  approvePhoto(id: number) {
    
    this.adminService.approvePhoto(id).subscribe(response => {
      this.alertify.success('Photo approved successfully.');
      this.removePhotoForView(id);
    }, error => {
      this.alertify.error(error);
    });
  }

  rejectPhoto(id: number) {
    this.adminService.rejectPhoto(id).subscribe(response => {
      this.alertify.success('Photo rejected successfully.');
      this.removePhotoForView(id);
    }, error => {
      this.alertify.error(error);
    });
  }

  removePhotoForView(id: number) {
    this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
  }

}
