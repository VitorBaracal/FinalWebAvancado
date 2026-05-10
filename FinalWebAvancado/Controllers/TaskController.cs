using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Dto;
using Api.Enums;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ListTasksAsync() {

        var tasks = await _context.Tasks.ToListAsync();

        var response = tasks.OrderBy(u => u.Id).Select(t => new TaskDto {
            Id = t.Id,
            UserId = t.UserId,
            Name = t.Name,
            Description = t.Description,
            Level = t.Level,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });

        return Ok(response);
    }

    [HttpGet("{id:int}", Name = "GetTaskById")]
    public async Task<IActionResult> GetByIdAsync(int id) {

        var task = await _context.Tasks.FindAsync(id);

        if (task == null) return NotFound();

        return Ok(new TaskDto {
            Id = task.Id,
            UserId = task.UserId,
            Name = task.Name,
            Description = task.Description,
            Level = task.Level,
            Status = task.Status,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync(PostTaskDto dto) {

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);

        if (!userExists) return BadRequest(new { message = EnumMessageReponse.InvalidUserReference });

        var utcNow = DateTime.UtcNow;

        var entity = new Api.Models.Task {
            UserId = dto.UserId,
            Name = dto.Name,
            Description = dto.Description,
            Level = dto.Level,
            Status = dto.Status,
            CreatedAt = utcNow,
            UpdatedAt = utcNow
        };

        _context.Tasks.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtRoute(
            "GetTaskById",
            new { id = entity.Id },
            new TaskDto {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Description = entity.Description,
                Level = entity.Level,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, PutTaskDto dto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) return NotFound();

        if (dto.Name is not null)
            task.Name = dto.Name;
        if (dto.Description is not null)
            task.Description = dto.Description;
        if (dto.Level is not null)
            task.Level = dto.Level.Value;
        if (dto.Status is not null)
            task.Status = dto.Status.Value;

        var utcNow = DateTime.UtcNow;

        task.UpdatedAt = utcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return NotFound();

        _context.Tasks.Remove(task);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
