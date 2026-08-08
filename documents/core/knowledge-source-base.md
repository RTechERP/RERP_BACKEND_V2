# Source Base: Backend Project Structure

This document maps the logical directories of the R_ERP Backend to their functional roles.

## Project Root: `R_ERPWeb/Backend/RERP_BACKEND_V2/RERP_BACKEND_V2`

### 1. `RERPAPI/` (The Core API)
The main executable project containing the web server logic.
- **`Controllers/`**: Grouped by business module:
  - `HRM/`: Employee, Payroll, Attendance.
  - `Systems/`: Menus, Roles, Versioning.
  - `Project/`: Project tracking.
  - `Logistics/`: Warehouse and AGV.
- **`Middleware/`**: Custom HTTP pipeline components like `DynamicAuthorizationMiddleware`.
- **`Program.cs`**: System configuration and Dependency Injection.

### 2. `RERPAPI.IRepo/` (The Abstractions)
Pure interfaces defining the contracts for service implementation.
- `IGenericRepo.cs`: The foundation for all CRUDS.
- Modules specific interfaces (e.g., `IEmployeeRepo.cs`).

### 3. `RERPAPI.Repo/` (The Implementation)
The "Heavy Lifters" - where business logic and data queries live.
- **`GenericEntity/`**: Sub-folders corresponding to business modules containing specific repository implementations.
- `GenericRepo.cs`: The core implementation of generic CRUD operations.

### 4. `RERPAPI.Model/` (The Data)
Shared data structures used by all layers.
- **`Entities/`**: C# classes representing database tables.
- **`DTO/`**: Data Transfer Objects for API inputs and outputs.
- **`Context/`**: `RTCContext.cs` (Entity Framework context).
- **`Common/`**: Shared settings like `JwtSettings`, `SmtpSettings`.

---

## File Search Tips
- **Business Logic**: Look in `RERPAPI.Repo/GenericEntity/[Module]`.
- **API Endpoints**: Look in `RERPAPI/Controllers/[Module]`.
- **Database Schema**: Look in `RERPAPI.Model/Entities`.
- **System Config**: Check `RERPAPI/Program.cs` or `appsettings.json`.
