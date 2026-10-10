// Matches EmployeeDto from .NET (what the API returns)
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  salary: number;
  dateOfJoining: string;
}

// Matches EmployeeRequestDto from .NET (what we send)
export type EmployeeRequest = Omit<Employee, 'id'>;

// Matches PagedResult<T> from .NET
export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Matches EmployeeQueryParameters from .NET
export interface EmployeeQuery {
  search?: string;
  department?: string;
  sortBy?: string;
  descending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

// One shared list, used by the list filter AND the add/edit form
export const DEPARTMENTS = ['IT', 'HR', 'Finance', 'Sales', 'Marketing', 'Operations'];