# Domain Model: R_ERP Business Entities

The R_ERP system covers a vast business domain. This document categorizes the core entities and their relationships.

## Core Domain Modules

### 1. Human Resource Management (HRM)
- **Employee**: The central entity, containing ~60+ fields (Code, Name, BirthDate, Address, PII).
- **Payroll**: `EmployeePayroll`, `EmployeePayrollDetail`, and `EmployeePayrollBonusDeduction`.
- **Attendance**: `EmployeeAttendance`, `EmployeeChamCongMaster`, `EmployeeOvertime`.
- **Org Structure**: `Department`, `EmployeeTeam`, `OrganizationalChart`.

### 2. Project Management
- **Project**: The root container for engineering work.
- **ProjectItem**: Tasks or components within a project.
- **ProjectPartList**: The "Bill of Materials" (BOM) for equipment production.
- **ProjectTask**: Individual assigned work items.

### 3. Logistics & Warehousing
- **Inventory**: `Inventory`, `InventoryStock`, `InventoryProject`.
- **Warehouse**: `Warehouse`, `Stock`, `Location`.
- **AGV (Automated Guided Vehicle)**: `AGVProduct`, `AGVBillImport`, `AGVBillExport`.
- **Modula**: Specialized entities for automated vertical storage integration (`ModulaLocation`).

### 4. Sales & Procurement
- **PONCC / POKH**: Purchase Orders (Vendor/Customer).
- **Quotation**: `QuotationNCC` (Supplier) and `QuotationKH` (Client).
- **RequestBuy**: Purchasing requests initiated by departments or projects.

### 5. System & Metadata
- **User / Role**: Identity management.
- **Menu / FormAndFunction**: Dynamic UI permission mapping.
- **GeneralCategory**: Standards like `Province`, `Currency`, `UnitCount`.

## Entity Relationship Overview
Most entities follow a **Master-Detail** pattern:
- `EmployeePayroll` (Master) -> `EmployeePayrollDetail` (Individual Rows).
- `ProjectPartList` (Master) -> `ProjectPartListVersion` -> Individual Part items.

## Sensitive Domains
- **Payroll/Financial**: Protected by high-level permission guards and audit logs.
- **PII (Personally Identifiable Information)**: Restricted to HR and Management roles.
