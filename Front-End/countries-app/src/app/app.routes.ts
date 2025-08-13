import { Routes } from '@angular/router';

export const routes: Routes = [{ path: 'countries', loadChildren: () => import('./countries/countries-module').then(m => m.CountriesModule) },
  { path: '', redirectTo: 'countries', pathMatch: 'full' }];
