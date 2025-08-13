import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CountryService, Country } from '../services/country';

@Component({
  selector: 'app-countries-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './countries-form.component.html',
  styleUrls: ['./countries-form.component.css']
})
export class CountriesFormComponent implements OnInit {

  countryForm!: FormGroup;
  isEditMode = false;
  countryId?: number;

  constructor(
    private fb: FormBuilder,
    private countryService: CountryService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.countryForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      code: ['', [Validators.required, Validators.maxLength(5)]]
    });

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.countryId = +params['id'];
        this.loadCountry(this.countryId);
      }
    });
  }

  loadCountry(id: number) {
    this.countryService.getCountryById(id).subscribe(country => {
      this.countryForm.patchValue({
        name: country.name,
        code: country.code
      });
    });
  }

  errorMessage: string = '';
  successMessage: string = '';

  onSubmit(): void {
    if (this.countryForm.invalid) return;

    this.errorMessage = '';
    this.successMessage = '';

    this.countryService.createCountry(this.countryForm.value).subscribe(
      () => {
        this.successMessage = 'Country created successfully!';

        // Hide toast and navigate after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
          this.router.navigate(['/']);
        }, 3000);

      },
      (error) => {
        console.error('Error creating country', error);

        if (error.status === 400 && error.error?.message) {
          this.errorMessage = error.error.message;

          if (this.errorMessage.toLowerCase().includes('name')) {
            this.countryForm.get('name')?.setErrors({ duplicate: true });
          }
          if (this.errorMessage.toLowerCase().includes('code')) {
            this.countryForm.get('code')?.setErrors({ duplicate: true });
          }

        } else {
          this.errorMessage = 'Failed to create country due to server error.';
        }
      }
    );
  }



  onCancel() {
    this.router.navigate(['/countries']);
  }
}
