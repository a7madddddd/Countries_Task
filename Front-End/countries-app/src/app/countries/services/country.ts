import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface Country {
  id: number;
  name: string;
  code: string;
  createdDate: string;
}

export interface PagedResult<T> {
  data: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  private apiUrl = 'https://localhost:44366/api/Countries';

  constructor(private http: HttpClient) { }

  getCountries(pageNumber: number, pageSize: number, search?: string): Observable<PagedResult<Country>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search) {
      params = params.set('search', search);
    }

    return this.http.get<ApiResponse<PagedResult<Country>>>(this.apiUrl, { params }).pipe(
      map(response => response.data)
    );
  }

  getCountryById(id: number): Observable<Country> {
    return this.http.get<ApiResponse<Country>>(`${this.apiUrl}/${id}`).pipe(
      map(response => response.data)
    );
  }

  createCountry(country: Country): Observable<Country> {
    return this.http.post<ApiResponse<Country>>(this.apiUrl, country).pipe(
      map(response => {
        if (!response.success && response.errors?.length) {
          throw new Error(response.errors.join(', '));
        }
        return response.data;
      })
    );
  }

  updateCountry(id: number, country: Country): Observable<Country> {
    return this.http.put<ApiResponse<Country>>(`${this.apiUrl}/${id}`, country).pipe(
      map(response => response.data)
    );
  }



  deleteCountry(id: number): Observable<void> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`).pipe(
      map(response => response.data)
    );
  }
}