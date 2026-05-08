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
}
