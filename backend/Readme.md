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

#### Step 1: Create a New Migration

```powershell
cd src/lambda-api
# Ensure environment variables are set
dotnet ef migrations add <migrationName> --context UserDbContext
```

#### Step 2: Generate SQL Script

```powershell
# Generate SQL from all migrations
dotnet ef migrations script --context UserDbContext

# Or generate SQL from a specific migration
dotnet ef migrations script <fromMigration> <toMigration> --context UserDbContext
```

#### Step 3: Review and Prepare SQL

1. Copy the generated SQL code to `Script/migration.sql`
2. Add `IF NOT EXISTS` statements where necessary to prevent conflicts
3. Review the SQL for any Aurora DSQL compatibility issues
4. Commit and push your updates to trigger the script execution and apply the migration via GitHub Actions.
