# ATLAS v0.2

Working backend foundation for the ATLAS Field Service Management platform.

## Included

- ASP.NET Core 8 Web API
- PostgreSQL + Entity Framework Core
- ASP.NET Core Identity
- JWT access tokens + refresh tokens
- Roles: SystemAdministrator, CompanyAdministrator, ServiceManager, Dispatcher, Technician, Warehouse, Customer
- Companies CRUD
- Customers CRUD
- Audit fields and soft delete
- Swagger with Bearer authentication
- Serilog console/file logging
- Health endpoint
- Docker Compose
- Automatic database creation and role/admin seeding

## Fastest start with Docker

```bash
docker compose up --build
```

Swagger: http://localhost:8080/swagger
Health: http://localhost:8080/health

Default development administrator:

- Email: admin@atlas.local
- Password: Atlas.Admin.2026!

Change this password before any non-local deployment.

## Start from Visual Studio

1. Install .NET 8 SDK and Docker Desktop/PostgreSQL.
2. Open `Atlas.sln`.
3. Set `Atlas.Api` as Startup Project.
4. Start PostgreSQL: `docker compose up postgres -d`.
5. Press F5.

## First API test

1. POST `/api/auth/login`
2. Use the default admin credentials.
3. Copy `accessToken`.
4. Click **Authorize** in Swagger and enter `Bearer <token>`.
5. Test `/api/companies` and `/api/customers`.

## Security note

Development secrets are deliberately easy to replace. Use environment variables or a secrets manager in production.
