import { Routes } from '@angular/router';
import { EmployeeList } from './pages/employee-list/employee-list';
import { EmployeeForm } from './pages/employee-form/employee-form';

export const routes: Routes = [
  { path: '', redirectTo: 'employees', pathMatch: 'full' },
  { path: 'employees', component: EmployeeList },
  { path: 'employees/add', component: EmployeeForm },        // Create
  { path: 'employees/edit/:id', component: EmployeeForm },   // Update (:id = route parameter)
  { path: '**', redirectTo: 'employees' }                    // must stay LAST
];