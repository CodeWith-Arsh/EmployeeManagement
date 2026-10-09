// Matches EmployeeDto from .NET (what the API returns)
export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  salary: number;
  dateOfJoining: string;   // JSON sends dates as strings
}

// Matches EmployeeRequestDto from .NET (what we send)
export type EmployeeRequest = Omit<Employee, 'id'>;