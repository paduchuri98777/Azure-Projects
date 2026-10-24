using Microsoft.Data.SqlClient;
namespace ITServiceDesk.API.Data;
public class SqlConnectionFactory {
 private readonly IConfiguration _configuration;
 public SqlConnectionFactory(IConfiguration configuration)=>_configuration=configuration;
 public SqlConnection Create()=>new SqlConnection(_configuration.GetConnectionString("DefaultConnection")??throw new InvalidOperationException("Database connection string is missing."));
}