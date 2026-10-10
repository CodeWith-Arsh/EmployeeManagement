import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Subject, catchError, debounceTime, filter, map, of, switchMap, tap } from 'rxjs';
import { DEPARTMENTS, Employee, EmployeeQuery, PagedResult } from '../../models/employee';
import { EmployeeService } from '../../services/employee.service';

@Component({
  selector: 'app-employee-list',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './employee-list.html',
  styleUrl: './employee-list.css'
})
export class EmployeeList implements OnInit {
  private employeeService = inject(EmployeeService);

  departments = DEPARTMENTS;
  pageSizeOptions = [5, 10, 20, 50];

  // ---------- Query state (what we ask the API for) ----------
  search = signal('');
  department = signal('');
  sortBy = signal('');
  descending = signal(false);
  pageNumber = signal(1);
  pageSize = signal(10);

  // ---------- Response state ----------
  result = signal<PagedResult<Employee> | null>(null);
  loading = signal(false);
  errorMessage = signal('');

  // ---------- Derived values (recalculate automatically) ----------
  employees = computed(() => this.result()?.items ?? []);

  pages = computed(() =>
    Array.from({ length: this.result()?.totalPages ?? 0 }, (_, i) => i + 1)
  );

  rangeText = computed(() => {
    const r = this.result();
    if (!r || r.totalCount === 0) return '';
    const from = (r.pageNumber - 1) * r.pageSize + 1;
    const to = Math.min(r.pageNumber * r.pageSize, r.totalCount);
    return `Showing ${from}–${to} of ${r.totalCount}`;
  });

  // ---------- RxJS streams ----------
  private loadTrigger = new Subject<void>();
  private searchTerms = new Subject<string>();

  constructor() {
    // Every load request goes through here.
    // switchMap CANCELS the previous HTTP call if a new one starts (no stale results).
    this.loadTrigger.pipe(
      tap(() => this.loading.set(true)),
      switchMap(() =>
        this.employeeService.getAll(this.buildQuery()).pipe(
          catchError(() => {
            this.errorMessage.set('Could not load employees. Is the API running?');
            return of(null);
          })
        )
      ),
      takeUntilDestroyed()          // auto-unsubscribe when the component is destroyed
    ).subscribe(result => {
      if (result) {
        this.result.set(result);
        this.errorMessage.set('');
      }
      this.loading.set(false);
    });

    // Search box: wait until the user stops typing for 400 ms
    this.searchTerms.pipe(
      map(term => term.trim()),
      debounceTime(400),
      filter(term => term !== this.search()),   // skip if nothing actually changed
      takeUntilDestroyed()
    ).subscribe(term => {
      this.search.set(term);
      this.pageNumber.set(1);                   // new search → back to page 1
      this.load();
    });
  }

  ngOnInit(): void {
    this.load();
  }

  private buildQuery(): EmployeeQuery {
    return {
      search: this.search(),
      department: this.department(),
      sortBy: this.sortBy(),
      descending: this.descending(),
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize()
    };
  }

  private load(): void {
    this.loadTrigger.next();
  }

  // ---------- UI event handlers ----------
  onSearch(term: string): void {
    this.searchTerms.next(term);
  }

  onDepartmentChange(value: string): void {
    this.department.set(value);
    this.pageNumber.set(1);
    this.load();
  }

  onPageSizeChange(value: string): void {
    this.pageSize.set(Number(value));
    this.pageNumber.set(1);
    this.load();
  }

  onSort(column: string): void {
    if (this.sortBy() === column) {
      this.descending.update(d => !d);       // same column → flip direction
    } else {
      this.sortBy.set(column);               // new column → ascending
      this.descending.set(false);
    }
    this.pageNumber.set(1);
    this.load();
  }

  sortIcon(column: string): string {
    if (this.sortBy() !== column) return '↕';
    return this.descending() ? '▼' : '▲';
  }

  goToPage(page: number): void {
    const totalPages = this.result()?.totalPages ?? 1;
    if (page < 1 || page > totalPages || page === this.pageNumber()) return;
    this.pageNumber.set(page);
    this.load();
  }

  clearFilters(): void {
    this.search.set('');
    this.department.set('');
    this.sortBy.set('');
    this.descending.set(false);
    this.pageNumber.set(1);
    this.load();
  }

  deleteEmployee(employee: Employee): void {
    if (!confirm(`Delete ${employee.firstName} ${employee.lastName}?`)) return;

    this.employeeService.delete(employee.id).subscribe({
      next: () => {
        // If we deleted the last row on this page, go back one page
        if (this.employees().length === 1 && this.pageNumber() > 1) {
          this.pageNumber.update(p => p - 1);
        }
        this.load();   // reload: with server-side paging, rows shift between pages
      },
      error: () => alert('Delete failed. Please try again.')
    });
  }
}