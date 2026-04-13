import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Employee, EmployeeCreateRequest } from '../models/employee';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly api = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  login(payload: EmployeeCreateRequest) {
    return this.http.post(`${this.api}/auth/login`, payload);
  }

  createEmployee(payload: EmployeeCreateRequest) {
    return this.http.post<Employee>(`${this.api}/employees`, payload);
  }

  listEmployees(search?: string) {
    return this.http.get<Employee[]>(`${this.api}/employees`, {
      params: search ? { search } : {}
    });
  }

  exportPdf(id: number) {
    window.open(`${this.api}/employees/${id}/certificate/pdf`, '_blank');
  }

  exportExcel(id: number) {
    window.open(`${this.api}/employees/${id}/certificate/excel`, '_blank');
  }
}
