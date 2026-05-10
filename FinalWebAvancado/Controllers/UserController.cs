using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static BCrypt.Net.BCrypt;
using Api.Data;
using Api.Dto;
using Api.Enums;
using Api.Models;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAsync()
    {
        var users = await _context.User.OrderBy(u => u.Id).Select(u => new UserDto {
                Id = u.Id,
                Name = u.Name,
                Login = u.Login
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<IActionResult> GetByIdAsync(int id) {
        var user = await _context.User.FindAsync(id);

        if (user == null) return NotFound();

        return Ok(new UserDto {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login
            });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateUserAsync(PostUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (dto.Password !=  dto.ConfirmPassword) return BadRequest(new { EnumMessageReponse.DistinctPasswords });

        var exists = await _context.User.AnyAsync(u => u.Login == dto.Login);

        if (exists) return BadRequest(new { message = EnumMessageReponse.UsedLogin });

        string passwordHash = HashPassword(dto.Password);

        var user = new User {
            Name = dto.Name,
            Login = dto.Login,
            PasswordHash = passwordHash 
        };

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtRoute("GetUserById",
            new { id = user.Id },
            new UserDto {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login
            }
        );
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateAsync(int id, PutUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return NotFound();

        if (dto.Password is not null || dto.ConfirmPassword is not null)
        {
            if (dto.Password is null || dto.ConfirmPassword is null || dto.Password != dto.ConfirmPassword)
                return BadRequest(new { message = EnumMessageReponse.DistinctPasswords });
        }

        if (dto.Login is not null)
        {
            var loginTaken = await _context.User.AnyAsync(u => u.Login == dto.Login && u.Id != id);
            if (loginTaken)
                return BadRequest(new { message = EnumMessageReponse.UsedLogin });
        }

        if (dto.Name is not null)
            user.Name = dto.Name;
        if (dto.Login is not null)
            user.Login = dto.Login;
        if (dto.Password is not null)
            user.PasswordHash = HashPassword(dto.Password);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return NotFound();

        var hasDependencies =
            await _context.Tasks.AnyAsync(t => t.UserId == id)
            || await _context.Categories.AnyAsync(c => c.UserId == id)
            || await _context.TaskCategories.AnyAsync(tc => tc.UserId == id);

        if (hasDependencies)
            return BadRequest(new { message = EnumMessageReponse.UserHasDependentData });

        _context.User.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}