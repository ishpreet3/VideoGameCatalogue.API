import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { ToastContainerComponent } from './shared/toast-container.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, ToastContainerComponent],
  template: `
    <nav class="navbar navbar-dark bg-dark">
      <div class="container">
        <a class="navbar-brand" routerLink="/games">
          <i class="bi bi-controller" aria-hidden="true"></i> Video Game Catalogue
        </a>
      </div>
    </nav>
    <main>
      <router-outlet />
    </main>
    <app-toast-container />
  `
})
export class AppComponent {}
