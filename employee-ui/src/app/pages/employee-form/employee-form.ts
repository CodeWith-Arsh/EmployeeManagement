import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeService } from '../../services/employee.service';
import { EmployeeRequest } from '../../models/employee';

@Component({
  selector: 'app-employee-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './employee-form.html',
  styleUrl: './employee-form.css'
})
export class EmployeeForm implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private employeeService = inject(EmployeeService);

  employeeId = signal<number | null>(null);   // null = Add mode, number = Edit mode
  loading = signal(false);
  saving = signal(false);
  errorMessage = signal('');

  departments = ['IT', 'HR', 'Finance', 'Sales', 'Marketing', 'Operations'];

  // Reactive form – rules mirror the .NET EmployeeRequestDto
  form = this.fb.nonNullable.group({
    firstName:     ['', [Validators.required, Validators.maxLength(50)]],
    lastName:      ['', [Validators.required, Validators.maxLength(50)]],
    email:         ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    department:    ['', Validators.required],
    salary:        [0,  [Validators.required, Validators.min(0), Validators.max(10000000)]],
    dateOfJoining: ['', Validators.required]
  });

  get isEditMode(): boolean {
    return this.employeeId() !== null;
  }

  ngOnInit(): void {
    // Read :id from the URL → /employees/edit/5
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.employeeId.set(Number(id));
      this.loadEmployee(Number(id));
    }
  }

  private loadEmployee(id: number): void {
    this.loading.set(true);
    this.employeeService.getById(id).subscribe({
      next: emp => {
        this.form.patchValue({
          ...emp,
          dateOfJoining: emp.dateOfJoining.substring(0, 10)   // "2024-06-15T00:00:00" → "2024-06-15"
        });
        this.loading.set(false);
      },
      error: () => {
        this.errorMessage.set('Employee not found.');
        this.loading.set(false);
      }
    });
  }

  // Show an error only after the user touched the field (or pressed Save)
  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();   // show all error messages
      return;
    }

    this.saving.set(true);
    this.errorMessage.set('');

    const data: EmployeeRequest = this.form.getRawValue();

    const request$: Observable<unknown> = this.isEditMode
      ? this.employeeService.update(this.employeeId()!, data)
      : this.employeeService.create(data);

    request$.subscribe({
      next: () => this.router.navigate(['/employees']),
      error: (err: HttpErrorResponse) => {
        this.saving.set(false);
        if (err.status === 409) {
          this.errorMessage.set(err.error?.message ?? 'Email already exists.');
        } else if (err.status === 400) {
          this.errorMessage.set('Please check the entered values.');
        } else {
          this.errorMessage.set('Something went wrong. Please try again.');
        }
      }
    });
  }
}