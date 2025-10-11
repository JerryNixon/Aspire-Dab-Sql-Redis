# SQL Commander

A minimalist, single-page ASP.NET Core web application for exploring SQL Server databases, generating scripts, and executing queries. Built with Razor Pages and modern server-side rendering.

## Features

### Database Explorer
- **Hierarchical tree view** of Tables, Views, and Stored Procedures grouped by schema
- **Filterable** object list for quick navigation
- **Expandable nodes** showing:
  - Columns with data types for tables and views
  - Parameters and output columns for stored procedures
- **Object actions**:
  - **Top N**: Generate `SELECT TOP N` statements
  - **Create**: Generate CREATE scripts from metadata
  - **Drop**: Generate DROP statements
- **Schema download**: Export complete database metadata as JSON

### SQL Editor
- Single-document SQL editor with syntax-aware textarea
- Execute queries with **Ctrl+E** or the play button
- **Cancel running queries** with the stop button
- **Multiple result sets** displayed in tabular format
- Results automatically limited to configurable row count (default: 100)

### Status Bar
- Real-time connection state and query execution status
- Live elapsed time counter during query execution
- Row count and truncation warnings
- Current database and user information

### Settings Management
- Connection string builder with fields for:
  - Server
  - Database
  - User ID
  - Password (SQL authentication only)
  - Default result limit
- **Test Connection** button to verify credentials
- **Settings persistence** to `sqlcommander.settings.json`
- **Delete settings file** option for reset
- Environment variable support via `ConnectionStrings__db`

### Priority Order
1. Settings file (`sqlcommander.settings.json`) - **highest priority**
2. Environment variable (`ConnectionStrings__db`)
3. Default/empty settings

## Architecture

- **ASP.NET Core 8.0** with Razor Pages
- **Microsoft.Data.SqlClient** for database connectivity
- **Serilog** for structured logging (console + file sinks)
- **Dependency Injection** throughout
- **Modern C# 12** syntax with records, pattern matching, and latest features
- **Separation of concerns**: Models, Services, and Pages

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (any edition)
- A SQL Server user account with appropriate permissions

### Configuration

#### Option 1: Environment Variable
Set the `ConnectionStrings__db` environment variable:

```bash
# Windows (PowerShell)
$env:ConnectionStrings__db = "Server=localhost;Database=master;User Id=sa;Password=YourPassword;TrustServerCertificate=true"

# Windows (Command Prompt)
set ConnectionStrings__db=Server=localhost;Database=master;User Id=sa;Password=YourPassword;TrustServerCertificate=true

# Linux/macOS
export ConnectionStrings__db="Server=localhost;Database=master;User Id=sa;Password=YourPassword;TrustServerCertificate=true"
```

#### Option 2: Settings File (Preferred)
Use the Settings dialog in the web interface to create `sqlcommander.settings.json`. This file will be created automatically when you save settings.

Example `sqlcommander.settings.json`:
```json
{
  "Server": "localhost",
  "Database": "master",
  "UserId": "sa",
  "Password": "YourPassword123",
  "DefaultResultLimit": 100,
  "TrustServerCertificate": true,
  "ConnectionTimeout": 30
}
```

### Running the Application

```bash
cd SqlCommander
dotnet run
```

Then navigate to `https://localhost:5001` (or the URL shown in the console).

### Logging

Logs are written to:
- **Console**: All log output
- **File**: `logs/sqlcommander-<date>.txt` (retained for 7 days)

Configure logging levels in `appsettings.json`.

## Usage

### Exploring the Database
1. Expand categories (Tables, Views, Stored Procedures)
2. Expand schemas to see objects
3. Click objects to expand and view columns/parameters
4. Use the filter box to search for specific objects

### Executing Queries
1. Type or generate SQL in the editor
2. Press **Ctrl+E** or click **Execute**
3. View results in the table below
4. Click **Cancel** to stop long-running queries

### Generating Scripts
- **Top N**: Generates `SELECT TOP N * FROM [schema].[object]`
- **Create**: Reconstructs CREATE statement from metadata
- **Drop**: Generates DROP statement

### Exporting Metadata
Click **Download Schema JSON** to export complete database schema including:
- All tables with columns and data types
- All views with columns
- All stored procedures with parameters and output columns

## Security Notes

⚠️ **Important**: 
- This application stores passwords in plain text in the settings file
- Do not use this in production environments without additional security measures
- Consider using integrated security or Azure AD authentication for production
- The settings file is saved in the application's content root directory

## Development

### Project Structure
```
SqlCommander/
├── Models/              # Data models and DTOs
│   ├── AppSettings.cs
│   ├── DatabaseMetadata.cs
│   └── QueryModels.cs
├── Services/            # Business logic layer
│   ├── ISettingsService.cs
│   ├── SettingsService.cs
│   ├── IMetadataService.cs
│   ├── MetadataService.cs
│   ├── IQueryExecutionService.cs
│   └── QueryExecutionService.cs
├── Pages/               # Razor Pages
│   ├── Index.cshtml
│   └── Index.cshtml.cs
└── Program.cs           # Application startup
```

### Key Technologies
- **Razor Pages**: Server-side page model
- **Dependency Injection**: All services registered as singletons
- **SqlConnectionStringBuilder**: Type-safe connection string management
- **Serilog**: Structured logging with multiple sinks
- **Modern C#**: Records, pattern matching, top-level statements

### Extending the Application

To add new features:
1. Define models in `Models/`
2. Create services in `Services/` with interfaces
3. Register services in `Program.cs`
4. Add page handlers in `Index.cshtml.cs`
5. Update UI in `Index.cshtml`

## Browser Title

The browser title dynamically updates to show:
```
sql commander: server-name/database-name
```

## Keyboard Shortcuts

- **Ctrl+E**: Execute query

## Design Philosophy

SQL Commander follows a minimalist design philosophy:
- **Flat layout** with neutral palette
- **Clean typography**: Monospaced font in editor, sans-serif elsewhere
- **No framework dependencies**: Vanilla JavaScript
- **Fast and direct**: Server-rendered HTML with minimal client-side logic
- **Professional appearance**: Reliable enough for technical users

## Troubleshooting

### Cannot connect to SQL Server
- Verify SQL Server is running
- Check firewall rules
- Ensure SQL authentication is enabled
- Verify credentials in settings

### Metadata not loading
- Check user has appropriate permissions
- Verify database exists and is accessible
- Check logs in `logs/` directory

### Settings not persisting
- Ensure application has write permissions to content root directory
- Check for file system errors in logs

## License

This is a demonstration/educational project. Use at your own risk.

## Related Tools

SQL Commander provides functionality similar to:
- SQL Server Management Studio (SSMS) - query execution and object exploration
- Azure Data Studio - lightweight SQL editor
- DBeaver - universal database tool

But with a focus on minimalism, web-based access, and MCP-style metadata operations.
