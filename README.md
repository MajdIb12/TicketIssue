# TicketIssue

Short description
-----------------
`TicketIssue` is a small ticketing/fare-calculation sample implemented in .NET 9. It demonstrates a clean separation between API, domain logic and infrastructure (EF Core), uses strategy patterns for base fare calculation and supports applying fare modifications (fixed amounts and percentage-based).

Solution layout
---------------
- `TicketIssue.Api/` - ASP.NET Core Web API project exposing endpoints for tickets and fare modifications. Contains DTOs and controllers.
- `TicketIssue.Domain/` - Domain models, business services and strategy implementations (fare calculation engine, strategies for product types).
- `TicketIssue.Infrastructure/` - EF Core `ApplicationDbContext`, entity configurations, migrations and a database seeder.

Key technologies
----------------
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core (SQL Server provider)
- Swagger / OpenAPI for interactive API documentation (enabled in Development)

How it works (high level)
-------------------------
- The API accepts requests to create sold products (tickets) of different product types (Point-to-Point and Daily Pass).
- `FareCalculationEngine` chooses a base-fare strategy based on the product type and computes the base fare.
- Fare modifications (fixed amount or percentage) can be applied; percentage modifications are applied relative to the base fare.
- Calculated tickets and applied modification transactions are persisted via EF Core. The application runs any pending migrations and seeds sample modifications on startup.

Domain summary
--------------
- Entities:
  - `SoldProduct` - represents an issued ticket (passenger, product type, distance/duration, base/final fare, issued date, applied modifications).
  - `FareModification` - defines a named modification: fixed amount or percentage, and whether it is active.
  - `SoldProductModification` - join entity linking a sold product with the modification that was applied and the applied amount.
- Strategies:
  - `PointToPointFareStrategy` - base fare = 10.00 + 2.50 * distance (km). Validates that `DistanceInKm` is provided and > 0.
  - `DailyPassFareStrategy` - base fare = 50.00 * durationInDays. Validates `DurationInDays` > 0.
- Business service:
  - `FareCalculationEngine` - selects appropriate strategy, computes base fare and applies requested modifications (percentage is applied on base fare, fixed amount added/subtracted). It stores the applied amounts into `SoldProduct.AppliedModifications` and sets `FinalFare`.

API endpoints
-------------
The API exposes the following controllers and endpoints (base path `/api`):

- `GET /api/tickets` - list tickets with pagination. Query params: `pageNumber` (default 1), `pageSize` (default 10).
- `GET /api/tickets/{id}` - get a ticket by GUID (includes applied modifications and related data).
- `POST /api/tickets/point-to-point` - create a point-to-point ticket
  - Request DTO: `CreatePointToPointTicketDto` { `PassengerName` (string), `DistanceInKm` (double), `ModificationIds` (List<int>?) }
- `POST /api/tickets/daily-pass` - create a daily pass ticket
  - Request DTO: `CreateDailyPassTicketDto` { `PassengerName` (string), `DurationInDays` (int), `ModificationIds` (List<int>?) }
- `GET /api/tickets/transactions-report` - paginated transaction report of applied modifications. Query params: `pageNumber`, `pageSize`.

- `GET /api/faremodifications` - list all fare modifications.
- `GET /api/faremodifications/{id}` - get modification by id.
- `POST /api/faremodifications` - create modification
  - Request DTO: `CreateModificationDto` { `Name`, `ValueType` (enum: `FixedAmount` | `Percentage`), `Value` (decimal), `IsActive` (bool) }
- `PUT /api/faremodifications/{id}` - update modification (uses `UpdateModificationDto`).
- `DELETE /api/faremodifications/{id}` - delete modification (will fail with DB constraint if already linked to sold products; prefer toggling `IsActive`).

Swagger / OpenAPI
-----------------
Swagger is configured and available when the app environment is Development. Run the API and open `/swagger` to interact with the API.

Database and seeding
--------------------
- `ApplicationDbContext` is configured to use SQL Server. The connection string key is `DefaultConnection` and must be supplied in `appsettings.json` or environment variables.
- On startup the app runs `context.Database.MigrateAsync()` and then `DbInitializer.SeedAsync(context)`. The seeder adds several sample `FareModification` records if none exist (e.g., First Class Tier 20% etc.).

Configuration
-------------
1. Add a connection string called `DefaultConnection` to `TicketIssue.Api/appsettings.Development.json` or to user secrets / environment variables. Example (local SQL Server):

```
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TicketIssueDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

2. To apply migrations from the repo root (requires `dotnet-ef` tools):

```
dotnet tool install --global dotnet-ef
dotnet ef database update --project TicketIssue.Infrastructure --startup-project TicketIssue.Api
```

Note: The application will automatically call `MigrateAsync()` at startup and seed the DB, so running the API after setting up the connection string is usually sufficient.

Running the API
---------------
From the repository root (PowerShell):

1. Ensure `DefaultConnection` is configured (see above).
2. Run:

```
dotnet run --project TicketIssue.Api
```

3. When running in Development you can open `https://localhost:{port}/swagger` to explore endpoints.

Sample JSON payloads
--------------------
- Create point-to-point ticket:

```
{ "passengerName": "Alice", "distanceInKm": 12.5, "modificationIds": [1,3] }
```

- Create daily-pass ticket:

```
{ "passengerName": "Bob", "durationInDays": 3, "modificationIds": [2] }
```

- Create modification:

```
{ "name": "Weekend Discount", "valueType": "FixedAmount", "value": -5.00, "isActive": true }
```

Important notes & tips
----------------------
- Percentage modifications are calculated from the product's base fare (not the running total).
- The domain enforces validation for required inputs (e.g., distance/duration) and will throw `ArgumentException` if required fields are missing or invalid; controllers return `400 Bad Request` in such cases.
- Deleting a `FareModification` that is already referenced by sold product modifications will fail due to the DB foreign key; prefer setting `IsActive = false` to disable it.

Development & Contribution
--------------------------
- Create a feature branch, add tests for new behavior, and open PRs against `main`.
- Keep domain logic covered by unit tests (not included in this template yet).

Quick copy-and-run (Docker)
---------------------------
If you want to give someone a one-line set of instructions to clone and run the project with Docker, add and share these commands:

```
git clone <repository-url>
cd <repository-root>
docker compose up --build
```

Replace `<repository-url>` with the repo URL and `<repository-root>` with the folder created by the clone. Docker and Docker Compose must be installed on the machine.


