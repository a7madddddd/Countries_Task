import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';  // Add this import
import { CountriesRoutingModule } from './countries-routing-module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    HttpClientModule,        
    CountriesRoutingModule,
    ReactiveFormsModule 
  ]
})
export class CountriesModule { }