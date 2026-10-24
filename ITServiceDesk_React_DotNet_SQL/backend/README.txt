IT SERVICE DESK BACKEND
Requirements: .NET 8 SDK and SQL Server.
Run database scripts first.
Then from backend/ITServiceDesk.API:
dotnet restore
dotnet run
API: http://localhost:5000/api
Azure: configure ConnectionStrings__DefaultConnection in App Service and add the deployed React origin to CORS in Program.cs.
