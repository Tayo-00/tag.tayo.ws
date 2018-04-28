import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  sidebarShowing = false;

  toggleSidebar() {
    const sidebar = document.getElementById('sidebar').classList;
    if (this.sidebarShowing) {
      sidebar.add('ng-leave');
      sidebar.remove('ng-enter');
      this.sidebarShowing = false;
    } else {
      sidebar.add('ng-enter');
      sidebar.remove('ng-leave');
      this.sidebarShowing = true;
    }
  }
}
