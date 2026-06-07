using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpensesManager.Api.Data;
using PersonalExpensesManager.Api.Models;
using PersonalExpensesManager.Api.Services;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var userId = User.GetUserId();
        var categories = await db.Categories
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Kind)
            .ThenBy(x => x.Name)
            .Select(x => x.ToDto())
            .ToListAsync();

        return categories;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CategoryDto dto)
    {
        var userId = User.GetUserId();
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Color = dto.Color,
            Kind = dto.Kind,
            UserId = userId
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoryDto dto)
    {
        var userId = User.GetUserId();
        var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = dto.Name.Trim();
        category.Color = dto.Color;
        category.Kind = dto.Kind;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var hasTransactions = await db.Transactions.AnyAsync(x => x.CategoryId == id && x.UserId == userId);
        var hasBudgets = await db.Budgets.AnyAsync(x => x.CategoryId == id && x.UserId == userId);
        if (hasTransactions || hasBudgets)
        {
            return Conflict("Категорію не можна видалити, доки з нею пов'язані операції або бюджети.");
        }

        var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (category is null)
        {
            return NotFound();
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
