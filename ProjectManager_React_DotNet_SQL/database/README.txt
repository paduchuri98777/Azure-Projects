DATABASE

SQL Server database:
ProjectManagerDb

Run the scripts in this exact order:
1. 01_CreateDatabase.sql
2. 02_CreateTables.sql
3. 03_SeedData.sql

Default backend connection:
Server=localhost;Database=ProjectManagerDb;Trusted_Connection=True;TrustServerCertificate=True;

If you use SQL Server Express, change the backend connection string to something such as:
Server=.\\SQLEXPRESS;Database=ProjectManagerDb;Trusted_Connection=True;TrustServerCertificate=True;
