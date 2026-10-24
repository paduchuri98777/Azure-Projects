using Microsoft.AspNetCore.Mvc;using Microsoft.Data.SqlClient;using ITServiceDesk.API.Data;using ITServiceDesk.API.Models;
namespace ITServiceDesk.API.Controllers;
[ApiController][Route("api/[controller]")]
public class TicketsController:ControllerBase{
 readonly SqlConnectionFactory _factory; public TicketsController(SqlConnectionFactory factory)=>_factory=factory;
 [HttpGet] public async Task<ActionResult<IEnumerable<Ticket>>> GetAll(){
  var list=new List<Ticket>();await using var c=_factory.Create();await c.OpenAsync();
  await using var cmd=new SqlCommand("SELECT Id,Title,Description,CategoryId,Priority,Status,CreatedById,AssignedToId,CreatedAt,UpdatedAt FROM Tickets ORDER BY Id DESC",c);
  await using var r=await cmd.ExecuteReaderAsync();while(await r.ReadAsync())list.Add(Read(r));return Ok(list);
 }
 [HttpGet("{id:int}")] public async Task<ActionResult<Ticket>> Get(int id){
  await using var c=_factory.Create();await c.OpenAsync();await using var cmd=new SqlCommand("SELECT Id,Title,Description,CategoryId,Priority,Status,CreatedById,AssignedToId,CreatedAt,UpdatedAt FROM Tickets WHERE Id=@Id",c);cmd.Parameters.AddWithValue("@Id",id);
  await using var r=await cmd.ExecuteReaderAsync();if(!await r.ReadAsync())return NotFound();return Ok(Read(r));
 }
 [HttpPost] public async Task<ActionResult<Ticket>> Create(Ticket t){
  if(string.IsNullOrWhiteSpace(t.Title)||t.CategoryId<=0||t.CreatedById<=0)return BadRequest("Title, Category and Created By are required.");
  await using var c=_factory.Create();await c.OpenAsync();
  var sql="INSERT INTO Tickets(Title,Description,CategoryId,Priority,Status,CreatedById,AssignedToId) OUTPUT INSERTED.Id,INSERTED.CreatedAt,INSERTED.UpdatedAt VALUES(@Title,@Description,@CategoryId,@Priority,@Status,@CreatedById,@AssignedToId)";
  await using var cmd=new SqlCommand(sql,c);Add(cmd,t);
  try{await using var r=await cmd.ExecuteReaderAsync();await r.ReadAsync();t.Id=r.GetInt32(0);t.CreatedAt=r.GetDateTime(1);t.UpdatedAt=r.GetDateTime(2);}catch(SqlException ex)when(ex.Number==547){return BadRequest("Selected user or category does not exist.");}
  return CreatedAtAction(nameof(Get),new{id=t.Id},t);
 }
 [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id,Ticket t){
  if(string.IsNullOrWhiteSpace(t.Title)||t.CategoryId<=0||t.CreatedById<=0)return BadRequest("Title, Category and Created By are required.");
  await using var c=_factory.Create();await c.OpenAsync();
  var sql="UPDATE Tickets SET Title=@Title,Description=@Description,CategoryId=@CategoryId,Priority=@Priority,Status=@Status,CreatedById=@CreatedById,AssignedToId=@AssignedToId,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id";
  await using var cmd=new SqlCommand(sql,c);cmd.Parameters.AddWithValue("@Id",id);Add(cmd,t);
  try{var n=await cmd.ExecuteNonQueryAsync();return n==0?NotFound():NoContent();}catch(SqlException ex)when(ex.Number==547){return BadRequest("Selected user or category does not exist.");}
 }
 [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id){
  await using var c=_factory.Create();await c.OpenAsync();await using var cmd=new SqlCommand("DELETE FROM Tickets WHERE Id=@Id",c);cmd.Parameters.AddWithValue("@Id",id);return await cmd.ExecuteNonQueryAsync()==0?NotFound():NoContent();
 }
 static Ticket Read(SqlDataReader r)=>new(){Id=r.GetInt32(0),Title=r.GetString(1),Description=r.IsDBNull(2)?"":r.GetString(2),CategoryId=r.GetInt32(3),Priority=r.GetString(4),Status=r.GetString(5),CreatedById=r.GetInt32(6),AssignedToId=r.IsDBNull(7)?null:r.GetInt32(7),CreatedAt=r.GetDateTime(8),UpdatedAt=r.GetDateTime(9)};
 static void Add(SqlCommand c,Ticket t){c.Parameters.AddWithValue("@Title",t.Title.Trim());c.Parameters.AddWithValue("@Description",t.Description??"");c.Parameters.AddWithValue("@CategoryId",t.CategoryId);c.Parameters.AddWithValue("@Priority",string.IsNullOrWhiteSpace(t.Priority)?"Medium":t.Priority);c.Parameters.AddWithValue("@Status",string.IsNullOrWhiteSpace(t.Status)?"Open":t.Status);c.Parameters.AddWithValue("@CreatedById",t.CreatedById);c.Parameters.AddWithValue("@AssignedToId",(object?)t.AssignedToId??DBNull.Value);}
}