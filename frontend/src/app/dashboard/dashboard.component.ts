import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../services/api.service';
import { Employee } from '../models/employee';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  employees: Employee[] = [];
  search = '';

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    this.fetch();
  }

  fetch() {
    this.api.listEmployees(this.search).subscribe(data => (this.employees = data));
  }

  exportPdf(id: number) {
    this.api.exportPdf(id);
  }

  exportExcel(id: number) {
    this.api.exportExcel(id);
  }
}
