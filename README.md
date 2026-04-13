# Employee Portal (Angular + .NET + MySQL)

## Features implemented
- Login page with role-based fields:
  - `admin`: employee name, age, dob, email, salary
  - `user`: employee name, age, dob, email
- Dashboard page with grid view.
- Search functionality in dashboard grid.
- Certificate export:
  - Excel export
  - PDF export in A4 format
  - PDF includes top logo placeholder and barcode
- Barcode scan endpoint logs certificate scan activity.
- Basic CSS styling for login/dashboard.

## Project structure
- `frontend/` Angular app
- `backend/` ASP.NET Core Web API with MySQL (EF Core)

## Backend run
```bash
cd backend
dotnet restore
dotnet run
```

## Frontend run
```bash
cd frontend
npm install
npm start
```

## API endpoints
- `POST /api/auth/login`
- `POST /api/employees`
- `GET /api/employees?search=...`
- `GET /api/employees/{id}/certificate/excel`
- `GET /api/employees/{id}/certificate/pdf`
- `GET /api/employees/certificate/scan/{certificateCode}`
