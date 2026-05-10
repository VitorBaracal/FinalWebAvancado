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

    /* Get  */
    [HttpGet]
    public async Task<IActionResult> ListTasksAsync() {
        var tasks = await _context.Tasks.ToListAsync();
        var dtos = tasks.OrderBy(u => u.Id).Select(t => new TaskDto {
            Id = t.Id,
            UserId = t.UserId,
            Name = t.Name,
            Description = t.Description,
            Level = t.Level,
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        });
        return Ok(dtos);
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

    /* Post */
    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync(PostTaskDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return BadRequest(new { message = EnumMessageReponse.InvalidUserReference });

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

        return CreatedAtRoute("GetTaskById",
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

            /* Put */
            [HttpPut("{id:int}")]
            public async Task<IActionResult> UpdateAsync(int id, [FromBody] TaskDto dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                    return NotFound();

                task.UserId = dto.UserId;
                task.Name = dto.Name;
                task.Description = dto.Description;
                task.Level = dto.Level;
                task.Status = dto.Status;
                task.UpdatedAt = DateTime.UtcNow;

                _context.Tasks.Update(task);

                await _context.SaveChangesAsync();

                return Ok(new TaskDto
                {
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

            /* Delete */
            [HttpDelete("{id:int}")]
            public async Task<IActionResult> DeleteAsync(int id)
            {
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                    return NotFound();

                _context.Tasks.Remove(task);

                await _context.SaveChangesAsync();

                return NoContent();
            }
}
