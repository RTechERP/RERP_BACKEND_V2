# Knowledge Overview: R_ERP Backend

This document provides a high-level overview of the R_ERP (RTC Enterprise Resource Planning) Backend system.

## Project Scope
The R_ERP Backend is a centralized service provider built using **ASP.NET Core**. It manages the persistent state, business logic, and security enforcement for the entire RERP ecosystem, including HRM (Human Resource Management), Project Management, and Logistics (Warehouse/AGV).

## Core Responsibilities
- **Data Persistence**: Managing thousands of records across hundreds of entities (Employees, Payroll, Projects, Inventory) via SQL Server.
- **Business Logic Enforcement**: Server-side validation, recursive calculations (Org Charts), and complex transactional integrity.
- **Security & Authorization**: Enforcing Role-Based Access Control (RBAC) and dynamic authorization via JWT middleware.
- **Integration**: Communicating with external systems (Modula storage, TCP clients) and providing real-time updates (SSE/Email).

## Target Audience
- **Backend Developers**: For understanding the service orchestration and repository patterns.
- **Frontend Developers**: For understanding API signatures and data structures.
- **DevOps/Architects**: For system deployment and scaling considerations.

---

## High-Level Capabilities
| Area | Description |
|------|-------------|
| **HRM** | Complex Employee lifecycle, Payroll processing, and Attendance tracking. |
| **Project** | Multi-level project task tracking, part list management, and procurement requests. |
| **Warehousing** | AGV integration, inventory audits, and location management (Modula). |
| **System** | Menu dynamic distribution, role management, and versioning control. |
