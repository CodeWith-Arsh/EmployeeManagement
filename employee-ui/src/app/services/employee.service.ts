import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Employee, EmployeeRequest } from '../models/employee';

@Injectable({ providedIn: 'root' })   // one shared instance for the whole app (singleton)
export class EmployeeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/employees`;

  // GET api/employees
  getAll(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }

  // GET api/employees/5
  getById(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  // POST api/employees
  create(employee: EmployeeRequest): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee);
  }

  // PUT api/employees/5
  update(id: number, employee: EmployeeRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, employee);
  }

  // DELETE api/employees/5
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}