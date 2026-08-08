# Coding Standards: R_ERP Backend

This document defines the development standards and patterns to be followed for the R_ERP Backend.

## General Principles

1. **Explicit Over Implicit**: Prefer clearly defined interfaces and strongly typed DTOs over `dynamic` or `object`.
2. **Repository Consistency**: Every new entity should have a corresponding `IRepo` and `Repo` implementation if it involves complex logic.
3. **Async Empowerment**: All I/O-bound operations (DB, Files, Network) MUST use `async/await`.

## Naming Conventions (C# / .NET)

| Element | Format | Example |
|---------|--------|---------|
| **Controllers** | PascalCase + Controller | `EmployeePayrollController` |
| **Methods** | PascalCase (Verb) | `GetEmployeeDetailsAsync` |
| **Variables/Fields** | camelCase | `_repo`, `employeeId` |
| **Interfaces** | I + PascalCase | `IEmployeeRepo` |
| **Entities** | PascalCase | `EmployeePayroll` |

## Repository Pattern Implementation

When adding a new repository:
1. Define the interface in `RERPAPI.IRepo`.
2. Implement the class in `RERPAPI.Repo`, inheriting from `GenericRepo<TEntity>`.
3. Register the service in `Program.cs` as `Scoped`.

```csharp
// Example Registration
builder.Services.AddScoped<IEmployeeRepo, EmployeeRepo>();
```

## Controller Standards

- **Return Types**: Always use `Task<IActionResult>`.
- **Response Wrapper**: Use a consistent response DTO (Status, Data, Message).
- **Dependency Injection**: Use Constructor Injection for Repositories and Services.

## Data Access Standards

- **LINQ**: Prefer Method Syntax for readability in complex queries.
- **Transactions**: For multi-entity writes, use `IDbContextTransaction`.
- **Validation**: Perform server-side validation even if the frontend does it.

## Security Standards

- **Authorization**: Use the `[DynamicAuthorize]` attribute (if applicable) or built-in `[Authorize]`.
- **Claims**: Do not store passwords or sensitive data in JWT claims; stick to IDs and Roles.
- **SQL Injection**: Always use Parameterized queries via EF Core (default).
