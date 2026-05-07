using Api.Data;
using Api.Dto;
using Api.Enums;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService) {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto) {

        var user = await _context.User
            .FirstOrDefaultAsync(u => u.Login == dto.Login);

        if (user is null)
            return Unauthorized(new { message = EnumMessageReponse.InvalidLogin });

        if (!Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = EnumMessageReponse.InvalidLogin });

        var token = _tokenService.GenerateToken(user.Login);

        return Ok(new {
            token,
            name = user.Name,
            user = user.Login
        });
    }  

}