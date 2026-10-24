using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjectManager.API.Data;
using ProjectManager.API.Models;

namespace ProjectManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly SqlConnectionFactory _factory;

    public ProjectsController(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetAll()
    {
        var projects = new List<Project>();

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            SELECT Id, Name, Description, Status, CreatedAt
            FROM Projects
            ORDER BY Id DESC;
            """;

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            projects.Add(new Project
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Status = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }

        return Ok(projects);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Project>> Get(int id)
    {
        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            SELECT Id, Name, Description, Status, CreatedAt
            FROM Projects
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            return NotFound();

        return Ok(new Project
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
            Status = reader.GetString(3),
            CreatedAt = reader.GetDateTime(4)
        });
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
            return BadRequest("Project name is required.");

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Projects (Name, Description, Status)
            OUTPUT INSERTED.Id, INSERTED.CreatedAt
            VALUES (@Name, @Description, @Status);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Name", project.Name.Trim());
        command.Parameters.AddWithValue("@Description", project.Description ?? "");
        command.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(project.Status) ? "Active" : project.Status);

        await using var reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        project.Id = reader.GetInt32(0);
        project.CreatedAt = reader.GetDateTime(1);

        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, Project project)
    {
        if (string.IsNullOrWhiteSpace(project.Name))
            return BadRequest("Project name is required.");

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            UPDATE Projects
            SET Name = @Name,
                Description = @Description,
                Status = @Status
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Name", project.Name.Trim());
        command.Parameters.AddWithValue("@Description", project.Description ?? "");
        command.Parameters.AddWithValue("@Status", project.Status ?? "Active");

        var affected = await command.ExecuteNonQueryAsync();

        return affected == 0 ? NotFound() : NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = "DELETE FROM Projects WHERE Id = @Id;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var affected = await command.ExecuteNonQueryAsync();

        return affected == 0 ? NotFound() : NoContent();
    }
}