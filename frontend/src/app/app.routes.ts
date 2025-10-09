import { Routes } from '@angular/router';
import { HomeComponent } from '../app/pages/home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },

  // LISTA
  {
    path: 'reservas',
    loadComponent: () =>
      import('./pages/reservas/reservas-list/reservas-list.component')
        .then(m => m.ReservasListComponent)
  },

  // DETALLE
  {
    path: 'reservas/:id',
    loadComponent: () =>
      import('./pages/reservas/reserva-detail/reserva-detail.component')
        .then(m => m.default) // si exportaste "default" en el componente de detalle
        // .then(m => m.ReservaDetailComponent) // usa esta línea si NO es default export
  },

  {
    path: 'habitaciones',
    loadComponent: () =>
      import('./pages/habitaciones/habitaciones-list/habitaciones-list.component')
        .then(m => m.HabitacionesListComponent)
  },
  {
    path: 'huespedes',
    loadComponent: () =>
      import('./pages/huespedes/huespedes-list/huespedes-list.component')
        .then(m => m.HuespedesListComponent)
  },
  {
    path: 'inicio',
    loadComponent: () =>
      import('./pages/home/home.component')
        .then(m => m.HomeComponent)
  },

  { path: '**', redirectTo: '' }
];
