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
public class CategoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context) {
        _context = context;
    }

     [HttpGet]
    public async Task<IActionResult> ListTasksAsync() {

        var category = await _context.Categories.ToListAsync();

        var response = category.OrderBy(u => u.Id).Select(c => new CategoryDto {
            Id = c.Id,
            UserId = c.UserId,
            Name = c.Name,
            
        });

        return Ok(response);
    }

    [HttpGet("{id:int}", Name = "GetCategoryById")]
    public async Task<IActionResult> GetByIdAsync(int id) {
        var category = await _context.Categories.FindAsync(id);

        if (category == null) return NotFound();

        return Ok(new CategoryDto {
            Id = category.Id,
            UserId = category.UserId,
            Name = category.Name,
            ColorHex = category.ColorHex
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategoryAsync(PostCategoryDto dto) {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userExists = await _context.User.AnyAsync(u => u.Id == dto.UserId);
        if (!userExists)
            return BadRequest(new { message = EnumMessageReponse.InvalidUserReference });

        var entity = new Api.Models.Category {
            UserId = dto.UserId,
            Name = dto.Name,
            ColorHex = dto.ColorHex
        };

        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetCategoryById",
            new { id = entity.Id },
            new CategoryDto {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                ColorHex = entity.ColorHex
            });
    }

     [HttpPut("{id:int}")]
public async Task<IActionResult> UpdateAsync(int id, PutCategoryDto dto)
{
    var category = await _context.Categories
        .FirstOrDefaultAsync(c => c.Id == id);

    if (category is null)
        return NotFound();

    if (dto.Name is not null)
        category.Name = dto.Name;

    if (dto.ColorHex is not null)
        category.ColorHex = dto.ColorHex;

    await _context.SaveChangesAsync();

    return NoContent();
}

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) return NotFound();

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
