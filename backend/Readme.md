# Database Migration Guide

## Creating a New Migration

Generate a new Entity Framework migration:

```powershell
set environment variables
cd ./backend/src/lambda-api
dotnet ef migrations add <migrationName> --context UserDbContext
```

## Generating Migration Script

Create a SQL script from the migration:

```powershell
dotnet ef migrations script --context UserDbContext
```

## Applying the Migration

1. Copy the generated SQL code to `migrations.sql`
2. Add `if not exists` statements where necessary to prevent conflicts
