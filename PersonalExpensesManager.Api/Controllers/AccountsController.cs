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
public class AccountsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll()
    {
        var userId = User.GetUserId();
        var accounts = await db.Accounts
            .Include(x => x.Transactions)
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return accounts
            .Select(account => account.ToDto(
                account.InitialBalance
                + account.Transactions.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount)
                - account.Transactions.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount)))
            .ToList();
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(AccountDto dto)
    {
        var userId = User.GetUserId();
        var account = new Account
        {
            Name = dto.Name.Trim(),
            Currency = dto.Currency.Trim().ToUpperInvariant(),
            InitialBalance = dto.InitialBalance,
            UserId = userId
        };

        db.Accounts.Add(account);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = account.Id }, account.ToDto(account.InitialBalance));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AccountDto dto)
    {
        var userId = User.GetUserId();
        var account = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (account is null)
        {
            return NotFound();
        }

        account.Name = dto.Name.Trim();
        account.Currency = dto.Currency.Trim().ToUpperInvariant();
        account.InitialBalance = dto.InitialBalance;

        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();
        var hasTransactions = await db.Transactions.AnyAsync(x => x.AccountId == id && x.UserId == userId);
        if (hasTransactions)
        {
            return Conflict("Рахунок не можна видалити, доки з ним пов'язані операції.");
        }

        var account = await db.Accounts.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (account is null)
        {
            return NotFound();
        }

        db.Accounts.Remove(account);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
