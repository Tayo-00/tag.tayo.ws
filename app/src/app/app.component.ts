import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  sidebarHidden = true;

  toggleSidebar() {
    if (this.sidebarHidden) {
      this.sidebarHidden = false;
    } else {
      this.sidebarHidden = true;
    }
  }
}
