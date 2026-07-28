# Changelog

## 0.2.0

- Added PostgreSQL persistence.
- Added ASP.NET Core Identity and JWT authentication.
- Added refresh-token rotation foundation.
- Added role seeding and development administrator.
- Added Companies and Customers CRUD endpoints.
- Added audit fields, optimistic version field and soft delete.
- Added Swagger bearer authentication, Serilog, health checks and Docker Compose.

## 0.2.1 - Work Orders foundation
- Added work-order aggregate with scheduling, technician assignment, GPS check-in/check-out, tasks, parts and customer signature.
- Added EF Core configurations and indexes.
- Added lifecycle validation for completion and cancellation.

## 0.2.3 - Build fix
- Added the missing audit properties to `BaseEntity`.
- Made audit setters accessible to `AtlasDbContext`.
- Added the optimistic concurrency `Version` property used by EF mappings.
- Enabled automatic loading of WorkOrder EF Core configurations.
