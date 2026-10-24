using Microsoft.AspNetCore.Mvc;using Microsoft.Data.SqlClient;using ITServiceDesk.API.Data;using ITServiceDesk.API.Models;
namespace ITServiceDesk.API.Controllers;
[ApiController][Route("api/[controller]")]
public class CategoriesController:ControllerBase{
 readonly SqlConnectionFactory _factory; public CategoriesController(SqlConnectionFactory factory)=>_factory=factory;
 [HttpGet] public async Task<ActionResult<IEnumerable<Category>>> GetAll(){
  var list=new List<Category>(); await using var c=_factory.Create(); await c.OpenAsync();
  await using var cmd=new SqlCommand("SELECT Id,Name FROM Categories ORDER BY Name",c);
  await using var r=await cmd.ExecuteReaderAsync(); while(await r.ReadAsync()) list.Add(new Category{Id=r.GetInt32(0),Name=r.GetString(1)});
  return Ok(list);
 }}