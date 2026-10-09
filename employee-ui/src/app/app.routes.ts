import { Routes } from '@angular/router';
import { EmployeeList } from './pages/employee-list/employee-list';

export const routes: Routes = [
  { path: '', redirectTo: 'employees', pathMatch: 'full' },
  { path: 'employees', component: EmployeeList },
  { path: '**', redirectTo: 'employees' }   // any unknown URL → list
];