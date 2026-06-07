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
public class TransactionsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TransactionDto>>> GetAll(
        [FromQuery] TransactionType? type,
        [FromQuery] int? categoryId,
        [FromQuery] int? accountId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] string? search)
    {
        var userId = User.GetUserId();
        var query = db.Transactions
            .Include(x => x.Category)
            .Include(x => x.Account)
            .Where(x => x.UserId == userId)
            .AsQueryable();

        if (type.HasValue)
        {
            query = query.Where(x => x.Type == type.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        if (accountId.HasValue)
        {
            query = query.Where(x => x.AccountId == accountId.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.Date.Date >= from.Value.Date);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.Date.Date <= to.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lowered = search.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(lowered) || (x.Note != null && x.Note.ToLower().Contains(lowered)));
        }

        var transactions = await query
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToListAsync();

        return transactions.Select(x => x.ToDto()).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionDto>> GetById(int id)
    {
        var userId = User.GetUserId();
        var transaction = await db.Transactions
            .Include(x => x.Category)
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

        return transaction is null ? NotFound() : transaction.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(TransactionDto dto)
    {
        var userId = User.GetUserId();
        if (!await db.Categories.AnyAsync(x => x.Id == dto.CategoryId && x.UserId == userId))
        {
            return BadRequest("Категорія не знайдена.");
        }

        if (!await db.Accounts.AnyAsync(x => x.Id == dto.AccountId && x.UserId == userId))
        {
            return BadRequest("Рахунок не знайдений.");
        }

        var transaction = new Transaction
        {
            Title = dto.Title.Trim(),
            Amount = dto.Amount,
            Date = dto.Date.Date,
            Type = dto.Type,
            CategoryId = dto.CategoryId,
            AccountId = dto.AccountId,
            UserId = userId,
            Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim()
        };

        db.Transactions.Add(transaction);
        await db.SaveChangesAsync();

        var created = await db.Transactions
            .Include(x => x.Category)
            .Include(x => x.Account)
            .FirstAsync(x => x.Id == transaction.Id && x.UserId == userId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TransactionDto dto)
    {
        var userId = User.GetUserId();
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (transaction is null)
        {
            return NotFound();
        }

        if (!await db.Categories.AnyAsync(x => x.Id == dto.CategoryId && x.UserId == userId))
        {
            return BadRequest("Категорія не знайдена.");
        }

        if (!await db.Accounts.AnyAsync(x => x.Id == dto.AccountId && x.UserId == userId))
        {
            return BadRequest("Рахунок не знайдений.");
        }

        transaction.Title = dto.Title.Trim();
        transaction.Amount = dto.Amount;
        transaction.Date = dto.Date.Date;
        transaction.Type = dto.Type;
        transaction.CategoryId = dto.CategoryId;
        transaction.AccountId = dto.AccountId;
        transaction.Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var transaction = await db.Transactions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (transaction is null)
        {
            return NotFound();
        }

        db.Transactions.Remove(transaction);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
