# HealthTrack API

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Build](https://img.shields.io/github/actions/workflow/status/Carlings/healthtrack-system/ci.yml)
![License](https://img.shields.io/badge/license-educational-green)

HealthTrack API is the backend for a diploma web application designed to monitor a person’s physical state and daily progress.  
It provides authentication, user profile management, health records tracking, goals, activities, notifications, dashboard aggregation, avatar uploads, refresh token flow, and JWT-based authorization.

## Tech Stack

- **ASP.NET Core Web API** (.NET 8)
- **Entity Framework Core**
- **MS SQL Server**
- **MediatR** (CQRS)
- **FluentValidation**
- **JWT authentication**
- **Refresh token flow**
- **Clean Architecture / vertical-slice inspired structure**
- **NUnit + Moq + FluentAssertions** for unit testing

---

## Features

### Authentication
- Register
- Login
- Refresh access token
- Logout
- JWT access token + refresh token flow
- Token version invalidation for instant logout / password change invalidation

### User Profile
- Get current user profile
- Update current user profile
- Change password
- Upload avatar
- Delete avatar

### Health Tracking
- Create, read, update, delete health records
- Filter health records by date range
- User-owned access only

### Goals
- Create, read, update, delete goals
- Dashboard progress calculations

### Activities
- Create, read, update, delete activities
- Activity types lookup
- Calories burned calculation based on activity type and duration

### Notifications
- Get unread notifications
- Get all notifications
- Get notification by id
- Mark notification as read
- Create and delete notifications

### Dashboard
- Aggregated overview endpoint for the main UI
- Latest health stats
- Weight trend
- Latest records
- Latest activities
- Latest notifications
- Unread notifications count
- Goal progress metrics

---

## Architecture

The solution follows a clean and pragmatic structure:

- **HealthTrack.Domain**: Core entities and enums.
- **HealthTrack.Application**: Commands, queries, DTOs, validators, interfaces, business logic.
- **HealthTrack.Infrastructure**: EF Core, repositories, JWT/token services, password hashing, file storage, persistence.
- **HealthTrack.Api**: Controllers, middleware, Swagger, DI registration, HTTP pipeline.

---

## Solution Structure

```text
HealthTrack.sln
- HealthTrack.Api
- HealthTrack.Application
- HealthTrack.Domain
- HealthTrack.Infrastructure
- HealthTrack.Tests
```

---

## Main API Endpoints

### Auth
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/refresh
- POST /api/auth/logout

### Users
- GET /api/users/me
- PUT /api/users/me
- PUT /api/users/change-password
- POST /api/users/avatar
- DELETE /api/users/avatar

### Health Records
- GET /api/records
- GET /api/records/{id}
- POST /api/records
- PUT /api/records/{id}
- DELETE /api/records/{id}

### Goals
- GET /api/goals
- POST /api/goals
- PUT /api/goals/{id}
- DELETE /api/goals/{id}

### Activities
- GET /api/activities
- GET /api/activities/{id}
- POST /api/activities
- PUT /api/activities/{id}
- DELETE /api/activities/{id}
- GET /api/activities/types

### Notifications
- GET /api/notifications
- GET /api/notifications/all
- GET /api/notifications/{id}
- POST /api/notifications
- PUT /api/notifications/{id}/read
- DELETE /api/notifications/{id}

### Dashboard
- GET /api/dashboard/overview

---

## Requirements

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 / Rider / VS Code
- Git

## Getting Started

1. Clone repository
2. Configure appsettings.json
3. Apply EF Core migrations
4. Run the API
5. Open Swagger UI

---

## Configuration

### appsettings.json
Set your database connection and JWT settings.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HealthTrackDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "Jwt": {
    "Issuer": "HealthTrack",
    "Audience": "HealthTrackUsers",
    "Key": "YOUR_SUPER_SECRET_KEY",
    "AccessTokenMinutes": 10,
    "RefreshTokenDays": 7
  },
  "AvatarStorage": {
    "RootPath": "wwwroot/avatars",
    "RequestPath": "/avatars"
  }
}
```

---

## Running the Project

Restore: `dotnet restore`  
Build: `dotnet build`  
Run: `dotnet run --project HealthTrack.Api`

Swagger UI:
https://localhost:xxxx/swagger

---

## Database

The app uses EF Core migrations.
Database schema includes: Users, HealthRecords, Goals, ActivityTypes, UserActivities, Notifications, RefreshTokens.

---

## Tests

Run tests: `dotnet test`  
Run with coverage: `dotnet test --collect:"XPlat Code Coverage"`

---

## Security Notes

- JWT access tokens are short-lived
- Refresh tokens are stored in database
- Token rotation is implemented
- TokenVersion invalidation is used for forced logout/password reset
- Passwords are hashed using ASP.NET Core PasswordHasher

---

## Future Improvements

- Azure Blob Storage integration
- Email-based password recovery
- Role-based authorization
- Docker support
- Redis caching
- Rate limiting
- API versioning
- Integration tests
- Frontend SPA client

---

## File Uploads
Supported formats: .jpg, .jpeg, .png, .webp. Max size: 5 MB.
