# DevHouse API

DevHouse is a RESTful ASP.NET Core Web API designed to manage software development resources such as developers, teams, projects, roles, and project types. It includes secure JWT authentication, clean separation using DTOs, and full CRUD functionality.

---

## Features

- JWT Authentication
- Developer/Project/Team/Role Management
- Entity Framework Core with MySQL
- Swagger UI for testing endpoints
- DTO-based response optimization

---

## Technologies & Packages

This project is built using the following NuGet packages:

| Package                                         | Version | Description                                     |
| ----------------------------------------------- | ------- | ----------------------------------------------- |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.0   | JWT token-based authentication                  |
| `Microsoft.EntityFrameworkCore.Design`          | 8.0.10  | EF Core design-time tools (used for migrations) |
| `MySQL.EntityFrameworkCore`                     | 8.0.10  | EF Core provider for MySQL                      |
| `Swashbuckle.AspNetCore`                        | 6.4.0   | Swagger/OpenAPI integration for .NET APIs       |
| `System.IdentityModel.Tokens.Jwt`               | 8.0.0   | Token handling for JWT                          |

Install these using:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.10
dotnet add package MySQL.EntityFrameworkCore --version 8.0.10
dotnet add package Swashbuckle.AspNetCore --version 6.4.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.0
```

---

## Setup Instructions

1. **Clone the repository**

```bash
git clone https://github.com/your-user/devhouse-api.git
cd devhouse-api
```

2. **Update `appsettings.json`**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=devhouse1;user=root;password=YOUR_PASSWORD"
},
"JWTSettings": {
  "SecretKey": "<your-generated-secret>",
  "Issuer": "MyIssuer",
  "Audience": "MyAudience",
  "ExpiryMinutes": 60
}
```

> Generate secret key:

```bash
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
```

3. **Create Database Migrations**

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Run the Application**

```bash
dotnet run
```

Navigate to Swagger UI:

```
https://localhost:<port>/swagger
```

---

## API Endpoints Overview

- `/api/Auth/token` — Generate JWT token
- `/api/Developer` — Manage developers
- `/api/Project` — Manage projects
- `/api/ProjectType` — Manage project types
- `/api/Team` — Manage teams
- `/api/Role` — Manage roles

All endpoints are documented in Swagger.

---

## Why DTOs?

DTOs (Data Transfer Objects) are used to:

- Simplify response structure
- Avoid circular references
- Hide unnecessary fields
- Return related names instead of just IDs (e.g., RoleName, TeamName)

---

## Authentication

JWT Authentication is used to secure all sensitive endpoints (Create, Update, Delete). Only authorized requests with valid tokens are allowed to modify data.

You must pass the token in the header:

```
Authorization: Bearer <your_token>
```

---
