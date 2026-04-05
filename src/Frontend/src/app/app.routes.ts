import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/layout/layout').then((m) => m.Layout),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/dashboard/components/dashboard/dashboard').then(
            (m) => m.Dashboard,
          ),
      },
      {
        path: 'transactions/create',
        loadComponent: () =>
          import(
            './features/transactions/components/create-transaction/create-transaction'
          ).then((m) => m.CreateTransaction),
      },
    ],
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/components/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () =>
      import('./features/auth/components/register/register').then(
        (m) => m.Register,
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
