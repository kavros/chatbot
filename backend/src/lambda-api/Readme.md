# Database migration
`.\set-env.ps1`

` dotnet ef migrations add migrationName --context DbContextName`

`dotnet ef migrations script --context UserDbContext`

Copy and paste SQL code to `migrations.sql`. 

Add `if not exists` statements, if needed.

Run the script to apply the migration to the database.
`./run-migration.ps1`