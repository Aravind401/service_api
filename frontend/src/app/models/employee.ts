export interface Employee {
  id: number;
  userType: 'admin' | 'user';
  employeeName: string;
  age: number;
  dob: string;
  email: string;
  salary?: number;
  certificateCode: string;
}

export interface EmployeeCreateRequest {
  userType: 'admin' | 'user';
  employeeName: string;
  age: number;
  dob: string;
  email: string;
  salary?: number | null;
}
