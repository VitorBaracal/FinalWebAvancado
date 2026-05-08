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

    [HttpGet("{id:int}", Name = "GetById")]
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

        return CreatedAtRoute("GetById",
            new { id = user.Id },
            new UserDto {
                Id = user.Id,
                Name = user.Name,
                Login = user.Login
            }
        );
    }
}