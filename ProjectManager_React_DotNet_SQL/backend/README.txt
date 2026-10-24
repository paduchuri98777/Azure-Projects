BACKEND

Requirements:
- .NET 8 SDK
- SQL Server

Before starting:
1. Run the SQL scripts in ../database in this order:
   01_CreateDatabase.sql
   02_CreateTables.sql
   03_SeedData.sql

If your SQL Server instance is not localhost, edit:
ProjectManager.API/appsettings.json

Run:
1. Open terminal in backend/ProjectManager.API
2. Run: dotnet restore
3. Run: dotnet run

API:
http://localhost:5000/api/projects
http://localhost:5000/api/tasks

The frontend is configured for:
http://localhost:5000
