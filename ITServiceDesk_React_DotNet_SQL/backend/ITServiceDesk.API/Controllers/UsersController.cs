using Microsoft.AspNetCore.Mvc;using Microsoft.Data.SqlClient;using ITServiceDesk.API.Data;using ITServiceDesk.API.Models;
namespace ITServiceDesk.API.Controllers;
[ApiController][Route("api/[controller]")]
public class UsersController:ControllerBase{
 readonly SqlConnectionFactory _factory; public UsersController(SqlConnectionFactory factory)=>_factory=factory;
 [HttpGet] public async Task<ActionResult<IEnumerable<User>>> GetAll(){
  var list=new List<User>(); await using var c=_factory.Create(); await c.OpenAsync();
  await using var cmd=new SqlCommand("SELECT Id,Name,Email,Department FROM Users ORDER BY Name",c);
  await using var r=await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(new User{Id=r.GetInt32(0),Name=r.GetString(1),Email=r.GetString(2),Department=r.GetString(3)});
  return Ok(list);
 }}