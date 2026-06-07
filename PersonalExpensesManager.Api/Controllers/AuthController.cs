using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpensesManager.Api.Data;
using PersonalExpensesManager.Api.Models;
using PersonalExpensesManager.Api.Services;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, PasswordService passwordService, JwtService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var exists = await db.Users.AnyAsync(x => x.Email == email);
        if (exists)
        {
            return Conflict("Користувач з таким email вже існує.");
        }

        var user = new User
        {
            FullName = dto.FullName.Trim(),
            Email = email,
            PasswordHash = passwordService.Hash(dto.Password),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        UserSeedService.AddStarterData(user);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return jwtService.CreateToken(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (user is null || !passwordService.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized("Невірний email або пароль.");
        }

        return jwtService.CreateToken(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var userId = User.GetUserId();
        var user = await db.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        return new CurrentUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }
}
