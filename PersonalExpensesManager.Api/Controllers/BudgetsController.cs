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
public class BudgetsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] int? month, [FromQuery] int? year)
    {
        var userId = User.GetUserId();
        var targetMonth = month ?? DateTime.Today.Month;
        var targetYear = year ?? DateTime.Today.Year;

        var budgets = await db.Budgets
            .Include(x => x.Category)
            .Where(x => x.UserId == userId && x.Month == targetMonth && x.Year == targetYear)
            .OrderBy(x => x.Category!.Name)
            .ToListAsync();

        var expenseTransactions = await db.Transactions
            .Where(x => x.UserId == userId && x.Type == TransactionType.Expense && x.Date.Month == targetMonth && x.Date.Year == targetYear)
            .ToListAsync();

        var expenses = expenseTransactions
            .GroupBy(x => x.CategoryId)
            .ToDictionary(x => x.Key, x => x.Sum(t => t.Amount));

        return budgets.Select(budget => new BudgetDto
        {
            Id = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category?.Name ?? string.Empty,
            CategoryColor = budget.Category?.Color ?? "#3B82F6",
            Limit = budget.Limit,
            Month = budget.Month,
            Year = budget.Year,
            Spent = expenses.GetValueOrDefault(budget.CategoryId)
        }).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> Create(BudgetDto dto)
    {
        var userId = User.GetUserId();
        if (!await db.Categories.AnyAsync(x => x.Id == dto.CategoryId && x.UserId == userId))
        {
            return BadRequest("Категорія не знайдена.");
        }

        var exists = await db.Budgets.AnyAsync(x => x.UserId == userId && x.CategoryId == dto.CategoryId && x.Month == dto.Month && x.Year == dto.Year);
        if (exists)
        {
            return Conflict("Бюджет для цієї категорії та місяця вже існує.");
        }

        var budget = new Budget
        {
            CategoryId = dto.CategoryId,
            Limit = dto.Limit,
            Month = dto.Month,
            Year = dto.Year,
            UserId = userId
        };

        db.Budgets.Add(budget);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { month = budget.Month, year = budget.Year }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, BudgetDto dto)
    {
        var userId = User.GetUserId();
        var budget = await db.Budgets.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (budget is null)
        {
            return NotFound();
        }

        if (!await db.Categories.AnyAsync(x => x.Id == dto.CategoryId && x.UserId == userId))
        {
            return BadRequest("Категорія не знайдена.");
        }

        var duplicate = await db.Budgets.AnyAsync(x =>
            x.UserId == userId && x.Id != id && x.CategoryId == dto.CategoryId && x.Month == dto.Month && x.Year == dto.Year);
        if (duplicate)
        {
            return Conflict("Бюджет для цієї категорії та місяця вже існує.");
        }

        budget.CategoryId = dto.CategoryId;
        budget.Limit = dto.Limit;
        budget.Month = dto.Month;
        budget.Year = dto.Year;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var budget = await db.Budgets.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (budget is null)
        {
            return NotFound();
        }

        db.Budgets.Remove(budget);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
