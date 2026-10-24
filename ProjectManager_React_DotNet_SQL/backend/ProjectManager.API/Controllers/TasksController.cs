using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjectManager.API.Data;
using ProjectManager.API.Models;

namespace ProjectManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly SqlConnectionFactory _factory;

    public TasksController(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        var tasks = new List<TaskItem>();

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            SELECT Id, ProjectId, Title, Description, Status, CreatedAt
            FROM Tasks
            ORDER BY Id DESC;
            """;

        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new TaskItem
            {
                Id = reader.GetInt32(0),
                ProjectId = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Status = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5)
            });
        }

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create(TaskItem task)
    {
        if (task.ProjectId <= 0)
            return BadRequest("Project is required.");

        if (string.IsNullOrWhiteSpace(task.Title))
            return BadRequest("Task title is required.");

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            INSERT INTO Tasks (ProjectId, Title, Description, Status)
            OUTPUT INSERTED.Id, INSERTED.CreatedAt
            VALUES (@ProjectId, @Title, @Description, @Status);
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@ProjectId", task.ProjectId);
        command.Parameters.AddWithValue("@Title", task.Title.Trim());
        command.Parameters.AddWithValue("@Description", task.Description ?? "");
        command.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(task.Status) ? "Todo" : task.Status);

        try
        {
            await using var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();

            task.Id = reader.GetInt32(0);
            task.CreatedAt = reader.GetDateTime(1);
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return BadRequest("The selected project does not exist.");
        }

        return Ok(task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskItem task)
    {
        if (task.ProjectId <= 0 || string.IsNullOrWhiteSpace(task.Title))
            return BadRequest("Project and task title are required.");

        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = """
            UPDATE Tasks
            SET ProjectId = @ProjectId,
                Title = @Title,
                Description = @Description,
                Status = @Status
            WHERE Id = @Id;
            """;

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@ProjectId", task.ProjectId);
        command.Parameters.AddWithValue("@Title", task.Title.Trim());
        command.Parameters.AddWithValue("@Description", task.Description ?? "");
        command.Parameters.AddWithValue("@Status", task.Status ?? "Todo");

        try
        {
            var affected = await command.ExecuteNonQueryAsync();
            return affected == 0 ? NotFound() : NoContent();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return BadRequest("The selected project does not exist.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await using var connection = _factory.Create();
        await connection.OpenAsync();

        const string sql = "DELETE FROM Tasks WHERE Id = @Id;";

        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", id);

        var affected = await command.ExecuteNonQueryAsync();

        return affected == 0 ? NotFound() : NoContent();
    }
}