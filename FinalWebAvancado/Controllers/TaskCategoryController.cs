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
public class TaskCategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskCategoryController(AppDbContext context) {
        _context = context;
    }

    [HttpGet("{id:int}", Name = "GetTaskCategoryById")]
    public async Task<IActionResult> GetByIdAsync(int id) {
        var link = await _context.TaskCategories.FindAsync(id);

        if (link == null) return NotFound();

        return Ok(new TaskCategoryDto {
            Id = link.Id,
            UserId = link.UserId,
            TaskId = link.TaskId,
            CategoryId = link.CategoryId
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskCategoryAsync(PostTaskCategoryDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return BadRequest(new { message = EnumMessageReponse.InvalidUserReference });

        var task = await _context.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == dto.TaskId);
        if (task == null || task.UserId != dto.UserId)
            return BadRequest(new { message = EnumMessageReponse.InvalidTaskReference });

        var category = await _context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);
        if (category == null || category.UserId != dto.UserId)
            return BadRequest(new { message = EnumMessageReponse.InvalidCategoryReference });

        var duplicate = await _context.TaskCategories.AnyAsync(tc =>
            tc.TaskId == dto.TaskId && tc.CategoryId == dto.CategoryId);
        if (duplicate)
            return BadRequest(new { message = EnumMessageReponse.TaskCategoryAlreadyLinked });

        var entity = new Api.Models.TaskCategory {
            UserId = dto.UserId,
            TaskId = dto.TaskId,
            CategoryId = dto.CategoryId
        };

        _context.TaskCategories.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetTaskCategoryById",
            new { id = entity.Id },
            new TaskCategoryDto {
                Id = entity.Id,
                UserId = entity.UserId,
                TaskId = entity.TaskId,
                CategoryId = entity.CategoryId
            });
    }
}
