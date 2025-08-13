import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CountryService, Country } from '../services/country';

@Component({
  selector: 'app-countries-edit',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './countries-edit.component.html',
  styleUrls: ['./countries-edit.component.css']
})
export class CountriesEditComponent implements OnInit {

  countryForm!: FormGroup;
  countryId?: number;

  constructor(
    private fb: FormBuilder,
    private countryService: CountryService,
    public router: Router,
    public route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.countryId = Number(this.route.snapshot.paramMap.get('id'));

    this.countryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(5)]]
    });

    if (this.countryId) {
      this.loadCountry();
    }
  }

  loadCountry(): void {
    this.countryService.getCountryById(this.countryId!).subscribe(
      (country: Country) => {
        this.countryForm.patchValue({
          name: country.name,
          code: country.code
        });
      },
      error => {
        console.error('Error loading country', error);
      }
    );
  }

backendError: string = '';
successMessage: string = '';

saveCountry(): void {
  if (this.countryForm.invalid || !this.countryId) return;

  const payload = this.countryForm.value;

  // Reset errors/messages
  this.backendError = '';
  this.successMessage = '';

  this.countryService.updateCountry(this.countryId, payload).subscribe(
    () => {
      this.successMessage = 'Country updated successfully!';

      // Hide success message after 3 seconds and navigate back
      setTimeout(() => {
        this.successMessage = '';
        this.goHome();
      }, 3000);
    },
    (error) => {
      console.error('Error updating country', error);

      if (error.status === 400 && error.error?.message) {
        this.backendError = error.error.message;

        if (this.backendError.toLowerCase().includes('name')) {
          this.countryForm.get('name')?.setErrors({ duplicate: true });
        } 
        if (this.backendError.toLowerCase().includes('code')) {
          this.countryForm.get('code')?.setErrors({ duplicate: true });
        }

      } else {
        this.backendError = 'Failed to update country due to server error.';
      }
    }
  );
}





  goHome(): void {
    this.router.navigate(['/']);
  }
}
