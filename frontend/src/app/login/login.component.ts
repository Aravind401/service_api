import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  error = '';

  form = this.fb.group({
    userType: ['user', Validators.required],
    employeeName: ['', Validators.required],
    age: [null as number | null, [Validators.required, Validators.min(18)]],
    dob: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    salary: [null as number | null]
  });

  constructor(private fb: FormBuilder, private api: ApiService, private router: Router) {}

  onUserTypeChange() {
    const role = this.form.value.userType;
    if (role === 'admin') {
      this.form.controls.salary.addValidators([Validators.required, Validators.min(0)]);
    } else {
      this.form.controls.salary.clearValidators();
      this.form.controls.salary.setValue(null);
    }
    this.form.controls.salary.updateValueAndValidity();
  }

  submit() {
    this.error = '';
    this.onUserTypeChange();
    if (this.form.invalid) {
      this.error = 'Please fill all required fields correctly.';
      return;
    }

    const payload = this.form.getRawValue();
    this.api.login(payload as never).subscribe({
      next: () => {
        this.api.createEmployee(payload as never).subscribe({
          next: () => this.router.navigateByUrl('/dashboard'),
          error: err => (this.error = err.error ?? 'Failed to save employee details')
        });
      },
      error: err => (this.error = err.error ?? 'Login validation failed')
    });
  }
}
