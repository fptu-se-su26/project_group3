# Student Management System

Windows desktop application for academic administration, built with **C# 12**, **.NET 8**, **WPF (MVVM)**, and **Entity Framework Core** on **Microsoft SQL Server**.

The system centralizes student and academic data: student profiles, majors, classes, lecturers, subjects, course sections, registrations, grades, tuition, and basic reports.

## Tech stack

- C# 12 / .NET 8
- WPF + XAML, MVVM architecture
- Entity Framework Core (SQL Server)
- xUnit for testing
- Design patterns: MVVM, Repository, Singleton (session), Factory (export), Observer, Command

## Project structure

```
StudentManagementSystem/
├── StudentManagement.WPF/          # Views, ViewModels, Commands (UI layer)
├── StudentManagement.Business/     # Services, Interfaces (business logic)
├── StudentManagement.DataAccess/   # AppDbContext, Repositories, Configurations
├── StudentManagement.Domain/       # Entities, Enums
└── StudentManagement.Tests/        # Unit / integration tests
```

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full) — 2019 or later
- Visual Studio 2022+ (or VS Code with the C# Dev Kit)

### Setup

1. Clone the repository:
   ```bash
   git clone <repo-url>
   cd StudentManagementSystem
   ```
2. Update the connection string in `StudentManagement.DataAccess/AppDbContext.cs` if your SQL Server instance differs from the default (`Server=(local)`).
3. Apply migrations to create the database:
   ```bash
   dotnet ef database update --project StudentManagement.DataAccess --startup-project StudentManagement.WPF
   ```
4. Build and run:
   ```bash
   dotnet build
   dotnet run --project StudentManagement.WPF
   ```

### Running tests

```bash
dotnet test
```

## Modules

| Module | Functions |
|---|---|
| Authentication & System Management | Login, roles, accounts, dashboard |
| Student Management | CRUD, search/filter, import/export (JSON/XML) |
| Class, Major & Lecturer Management | CRUD, homeroom assignment |
| Subject & Course Registration | Sections, registration, timetable |
| Grade, Tuition & Reporting | Grade calculation, tuition billing, reports |

## Team

| Student ID | Full Name | GitHub | Role | Responsibility |
|---|---|---|---|---|
| DE190408 | Hồ Võ Minh Vỹ | [minhvy259](https://github.com/minhvy259) | Leader | Account & System Management |
| DE190147 | Phạm Nguyễn Tiến Đạt | [tiendat2277sg-wq](https://github.com/tiendat2277sg-wq) | Member | Student Management |
| DE190900 | Dương Khang Huy | [khuy17](https://github.com/khuy17) | Member | Class, Major & Lecturer Management |
| DE191003 | Hồ Tấn Phát | [Phat1425](https://github.com/Phat1425) | Member | Subject & Registration Management |
| DE190130 | Nguyễn Đăng Phúc | [ndangphuc-lgtm](https://github.com/ndangphuc-lgtm) | Member | Grade, Tuition & Reporting |
