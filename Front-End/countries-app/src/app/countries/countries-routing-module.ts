import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CountriesListComponent } from './countries-list.component/countries-list.component';
import { CountriesFormComponent } from './countries-form.component/countries-form.component';
import { CountriesEditComponent } from './countries-edit.component/countries-edit.component';

const routes: Routes = [
  { path: '', component: CountriesListComponent, pathMatch: 'full' },
  { path: 'create', component: CountriesFormComponent },
  { path: 'edit/:id', component: CountriesEditComponent },

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CountriesRoutingModule { }