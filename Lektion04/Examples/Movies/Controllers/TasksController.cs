using Microsoft.AspNetCore.Mvc;
using Movies.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    // Midlertidig in-memory liste til at gemme opgaver (erstattes normalt af Entity Framework Core)
    private static readonly List<TaskItem> _tasks = new();

    // GET /api/tasks
    [HttpGet]
    public ActionResult<IEnumerable<TaskItem>> GetTasks()
    {
        return Ok(_tasks);
    }

    // GET /api/tasks/{id}
    [HttpGet("{id}")]
    public ActionResult<TaskItem> GetTaskById(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        
        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    // POST /api/tasks
    [HttpPost]
    public ActionResult<TaskItem> CreateTask(CreateTaskDto createTaskDto)
    {
        // Generer et simpelt unikt ID
        int newId = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;

        var task = new TaskItem
        {
            Id = newId,
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _tasks.Add(task);

        // Returnerer 201 Created med Location-header til det nye endepunkt
        return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        // Opdater egenskaber
        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.IsCompleted = updateTaskDto.IsCompleted;

        return NoContent(); // 204 No Content ved succes
    }

    // DELETE /api/tasks/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        _tasks.Remove(task);

        return NoContent(); // 204 No Content ved succes
    }
}