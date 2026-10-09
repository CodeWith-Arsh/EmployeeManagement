import { Component, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Employee } from '../../models/employee';
import { EmployeeService } from '../../services/employee.service';

@Component({
  selector: 'app-employee-list',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css'
})
export class EmployeeList implements OnInit {
  private employeeService = inject(EmployeeService);

  // Signals = reactive state; the UI updates automatically when they change
  employees = signal<Employee[]>([]);
  loading = signal(true);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.loading.set(true);
    this.employeeService.getAll().subscribe({
      next: data => {
        this.employees.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Could not load employees. Is the API running?');
        this.loading.set(false);
      }
    });
  }

  deleteEmployee(employee: Employee): void {
    if (!confirm(`Delete ${employee.firstName} ${employee.lastName}?`)) return;

    this.employeeService.delete(employee.id).subscribe({
      next: () => {
        // Remove from the list without reloading everything
        this.employees.update(list => list.filter(e => e.id !== employee.id));
      },
      error: () => alert('Delete failed. Please try again.')
    });
  }
}