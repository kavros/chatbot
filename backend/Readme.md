# Backend API

A .NET 8 AWS Lambda API with authentication, AI agents, and PostgreSQL database.

## Configuration

Environment variables required:

- `Jwt` - JWT settings (JSON)
- `Google` - Google OAuth settings (JSON)
- `GitHubModelsToken` - OpenAI API token
- Database connection string

## Development

### Prerequisites

- .NET 8 SDK
- PostgreSQL database
- AWS credentials configured

### Running Locally

```bash
cd src/lambda-api
dotnet run
```

### Database Migrations

Create a new migration:

```powershell
cd src/lambda-api
# make sure that the environemnt variables are set
dotnet ef migrations add <migrationName> --context UserDbContext
```

Generate SQL script:

```powershell
# make sure that the environemnt variables are set
dotnet ef migrations script --context UserDbContext
```

Execute:

1. Copy the generated SQL code to `migrations.sql`
2. Add `if not exists` statements where necessary to prevent conflicts
