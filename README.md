# HealthTrack API

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Build](https://img.shields.io/github/actions/workflow/status/Carlings/healthtrack-system/ci.yml)
![License](https://img.shields.io/badge/license-educational-green)

Backend for the HealthTrack diploma project: auth, profile, records, goals, activities, notifications, dashboard aggregation, realtime notification stream (SSE), and avatar uploads.

## Tech Stack

- ASP.NET Core Web API (.NET 8)
- EF Core + SQL Server
- MediatR (CQRS style)
- FluentValidation
- JWT + Refresh tokens
- NUnit + Moq + FluentAssertions

## Solution Structure

- `HealthTrack.Api` — controllers, middleware, DI, Swagger, HTTP pipeline
- `HealthTrack.Application` — commands, queries, DTOs, validators, interfaces
- `HealthTrack.Domain` — entities/enums
- `HealthTrack.Infrastructure` — DbContext, repositories, token/avatar/realtime services
- `HealthTrack.Tests` — unit tests

## Main Features

### Authentication

- Register / Login / Refresh / Logout
- Token version invalidation (forced re-login support)

### Users

- `GET /api/users/me`
- `PUT /api/users/me`
- `PUT /api/users/change-password`
- `POST /api/users/avatar`
- `DELETE /api/users/avatar`

### Health Records

- CRUD + filtering by date range
- Paged list:
  - `GET /api/records/paged?page=1&pageSize=10&from=&to=`
- Validation includes prevention of future `recordedAt`

### Goals

- CRUD goals
- Dashboard progress metrics

### Activities

- CRUD activities
- Activity types lookup: `GET /api/activities/types`
- Paged list: `GET /api/activities/paged?page=1&pageSize=10`

### Notifications

- Unread/all/by-id, mark as read, create/delete
- SSE stream:
  - `GET /api/notifications/stream` (`text/event-stream`)
- Auto-generated alerts from health records:
  - high blood pressure
  - high heart rate
- Duplicate cooldown: 1 minute per message/type, with future-date guard fix

### Dashboard

- `GET /api/dashboard/overview`
- Returns:
  - latest stats
  - weight trend
  - latest records/activities/notifications
  - unread notifications count
  - goal progress snapshot

## Realtime Flow

1. User creates record (`POST /api/records`)
2. Health rules evaluate pulse/pressure
3. Notification saved in DB
4. Realtime publisher emits `notification.created`
5. SSE endpoint pushes event to connected frontend clients

## CI

Workflow: `.github/workflows/ci.yml`

- restore
- build (Release)
- test + trx artifact upload

## Configuration (`appsettings.json`)

- `ConnectionStrings:DefaultConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`
- `Jwt:AccessTokenMinutes`
- `Jwt:RefreshTokenDays`
- `AvatarStorage:RootPath`
- `AvatarStorage:RequestPath`

## Run Locally

```bash
dotnet restore
dotnet build HealthTrack.sln -c Release
dotnet run --project HealthTrack.Api
```

Swagger: `https://localhost:<port>/swagger`

## Tests

```bash
dotnet test HealthTrack.sln -c Release
```

## Notes

- Avatar storage is local filesystem-based (`wwwroot/avatars`) at the moment.
- Realtime notifications are delivered through SSE and consumed by frontend stream subscription.

## Next Improvements (Optional)

- Move avatar storage to Azure Blob Storage
- Add backend-calculated dashboard status fields
- Add Azure deployment workflows for App Service + SQL
