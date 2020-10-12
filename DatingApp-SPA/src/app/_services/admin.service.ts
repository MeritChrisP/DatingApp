import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Photo } from '../_models/photo';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

getUsersWithRoles() {
  return this.http.get(this.baseUrl + 'admin/usersWithRoles');
}

updateUserRoles(user: User, roles: {}) {
  return this.http.post(this.baseUrl + 'admin/editRoles/' + user.userName, roles);
}

getPhotosForModeration() {
  return this.http.get<Photo[]>(this.baseUrl + 'admin/photosForModeration');
}

approvePhoto(id: number) {
  return this.http.post(this.baseUrl + 'admin/photosForModeration/approve/' + id, {});
}

rejectPhoto(id: number) {
  return this.http.post(this.baseUrl + 'admin/photosForModeration/reject/' + id, {});
}
}
