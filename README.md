# Data API Builder + .NET Aspire + Razor Pages Sample

A minimal sample showing how .NET Aspire composes SQL Server + Data API Builder (DAB) + a Razor Pages UI. All connection details (including the SQL connection string) are generated and injected by AppHost; you do not need to manually edit them.

## What is here?

Project | Purpose
------- | -------
`AppHost` | Orchestrates SQL Server, Data API Builder container, and Web project with Aspire.
`Database` | SQL project (`.sqlproj`) defining schema and seed script.
`Web` | Razor Pages frontend consuming DAB REST endpoints.
`api/dab-config.json` | Mounted into the DAB container (provided in repo). No manual connection string changes required.

## Quick start

Prerequisites:
- .NET 8 SDK
- Docker Desktop running

Steps:
1. Run: Hit `F5` or `dotnet run --project AppHost`.
2. Aspire Workbench opens automatically: inspect resources and use the provided links.
3. Useful endpoints:
   - DAB Swagger: `/swagger`
   - GraphQL (if enabled): `/graphql`
   - Health: `/health`
   - Web UI: root of the web project

## Data access pattern
The Web project uses repositories (`TodoRepository`, `CategoryRepository`) that talk to DAB via `TableRepository<T>` instances produced by `DabRepositoryFactory`. Environment variables (e.g. `services__dab__http__0`) supplied by Aspire give the DAB base URL at runtime.

## UI
`Pages/Index.cshtml` shows pending vs. completed items. A checkbox toggles completion; icon links handle edit/delete with minimal chrome. `/health` is exposed for quick container readiness checks.

## License
Add a license of your choice (e.g. MIT) at the root.
