import { Component, OnInit } from '@angular/core';
import { CountryService, Country, PagedResult } from '../services/country';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-countries-list',
  standalone: true,
  templateUrl: './countries-list.component.html',
  styleUrls: ['./countries-list.component.css'],
  imports: [CommonModule]
})
export class CountriesListComponent implements OnInit {

  countries: Country[] = [];
  totalRecords = 0;
  pageNumber = 1;
  pageSize = 10;
  searchTerm = '';
  totalPages = 0;
  Math = Math;
  sortColumn: keyof Country = 'name';
  sortAscending = true;



  constructor(private countryService: CountryService, private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.loadCountries();
  }



  loadCountries(): void {
    this.countryService.getCountries(this.pageNumber, this.pageSize, this.searchTerm).subscribe(
      (result: PagedResult<Country>) => {
        this.countries = result.data;
        this.totalRecords = result.totalRecords;
        this.pageNumber = result.pageNumber;
        this.pageSize = result.pageSize;
        this.totalPages = result.totalPages;

      },
      error => {
        console.error('Error fetching countries', error);
      }
    );
  }

  onSearchChange(newSearch: string): void {
    this.searchTerm = newSearch;
    this.pageNumber = 1;
    this.loadCountries();
  }

  onPageChange(newPage: number): void {
    if (newPage < 1 || newPage > this.totalPages) return;
    this.pageNumber = newPage;
    this.loadCountries();
  }

  sortCountries(column: keyof Country) {
    if (this.sortColumn === column) {
      // If clicking the same column, toggle the order
      this.sortAscending = !this.sortAscending;
    } else {
      // Otherwise, change the column and reset to ascending
      this.sortColumn = column;
      this.sortAscending = true;
    }

    this.countries.sort((a, b) => {
      let valueA = a[column];
      let valueB = b[column];

      // Handle date sorting
      if (column === 'createdDate') {
        valueA = Date.parse(valueA as any);
        valueB = Date.parse(valueB as any);
      } else {
        // Handle string sorting (name, code, etc.)
        valueA = valueA?.toString().toLowerCase();
        valueB = valueB?.toString().toLowerCase();
      }

      if (valueA < valueB) return this.sortAscending ? -1 : 1;
      if (valueA > valueB) return this.sortAscending ? 1 : -1;
      return 0;
    });
  }

  goToAddCountry(): void {
    this.router.navigate(['./create'], { relativeTo: this.route });
  }

  goToEditCountry(id: number) {
    this.router.navigate(['/countries/edit', id]);
  }

successMessage: string = '';
errorMessage: string = '';
showDeleteConfirm = false;
countryToDeleteId: number | null = null;

promptDeleteCountry(id: number) {
  this.countryToDeleteId = id;
  this.showDeleteConfirm = true;
}

confirmDelete(confirm: boolean) {
  if (confirm && this.countryToDeleteId != null) {
    this.countryService.deleteCountry(this.countryToDeleteId).subscribe(() => {
      this.successMessage = 'Country deleted successfully!';
      this.loadCountries();
      setTimeout(() => this.successMessage = '', 3000);
    }, error => {
      this.errorMessage = 'Failed to delete country.';
      setTimeout(() => this.errorMessage = '', 3000);
    });
  }
  this.showDeleteConfirm = false;
  this.countryToDeleteId = null;
}

}

