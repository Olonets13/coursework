using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalExpensesManager.Api.Data;
using PersonalExpensesManager.Api.Services;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class StatisticsController(AppDbContext db) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<SummaryDto>> GetSummary()
    {
        var userId = User.GetUserId();
        var today = DateTime.Today;
        var transactions = await db.Transactions.Where(x => x.UserId == userId).ToListAsync();

        var totalIncome = transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount);
        var totalExpenses = transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount);
        var monthItems = transactions.Where(x => x.Date.Month == today.Month && x.Date.Year == today.Year).ToList();

        return new SummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses,
            MonthIncome = monthItems.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount),
            MonthExpenses = monthItems.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount),
            TransactionsCount = transactions.Count
        };
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<List<CategorySummaryDto>>> GetByCategory([FromQuery] int? month, [FromQuery] int? year)
    {
        var targetMonth = month ?? DateTime.Today.Month;
        var targetYear = year ?? DateTime.Today.Year;

        var transactions = await db.Transactions
            .Include(x => x.Category)
            .Where(x => x.UserId == User.GetUserId() && x.Type == TransactionType.Expense && x.Date.Month == targetMonth && x.Date.Year == targetYear)
            .ToListAsync();

        var data = transactions
            .GroupBy(x => new { x.CategoryId, x.Category!.Name, x.Category.Color })
            .Select(x => new CategorySummaryDto
            {
                CategoryId = x.Key.CategoryId,
                CategoryName = x.Key.Name,
                CategoryColor = x.Key.Color,
                Amount = x.Sum(t => t.Amount),
                Count = x.Count()
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        return data;
    }

    [HttpGet("by-month")]
    public async Task<ActionResult<List<MonthlySummaryDto>>> GetByMonth()
    {
        var userId = User.GetUserId();
        var transactions = await db.Transactions.Where(x => x.UserId == userId).ToListAsync();

        var data = transactions
            .GroupBy(x => new { x.Date.Year, x.Date.Month })
            .Select(x => new MonthlySummaryDto
            {
                Year = x.Key.Year,
                Month = x.Key.Month,
                Income = x.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount),
                Expenses = x.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount)
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToList();

        return data;
    }
}
