import { Routes } from '@angular/router';

const loadEditComponent = () =>
  import('./components/edit/edit.component').then(m => m.EditComponent);

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'games' },
  {
    path: 'games',
    title: 'Video Games',
    loadComponent: () => import('./components/browse/browse.component').then(m => m.BrowseComponent)
  },
  { path: 'games/new', title: 'Add Game', loadComponent: loadEditComponent },
  { path: 'games/:id/edit', title: 'Edit Game', loadComponent: loadEditComponent },
  { path: '**', redirectTo: 'games' }
];
