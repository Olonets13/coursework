using System.Net.Http.Headers;
using System.Net.Http.Json;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Client.Services;

public class FinanceApiClient(HttpClient http, AuthTokenStore tokenStore)
{
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        await AttachTokenAsync();
        return await http.GetFromJsonAsync<List<CategoryDto>>("api/categories") ?? [];
    }

    public async Task SaveCategoryAsync(CategoryDto category)
    {
        await AttachTokenAsync();
        if (category.Id == 0)
        {
            await http.PostAsJsonAsync("api/categories", category);
            return;
        }

        await http.PutAsJsonAsync($"api/categories/{category.Id}", category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        await AttachTokenAsync();
        await http.DeleteAsync($"api/categories/{id}");
    }

    public async Task<List<AccountDto>> GetAccountsAsync()
    {
        await AttachTokenAsync();
        return await http.GetFromJsonAsync<List<AccountDto>>("api/accounts") ?? [];
    }

    public async Task SaveAccountAsync(AccountDto account)
    {
        await AttachTokenAsync();
        if (account.Id == 0)
        {
            await http.PostAsJsonAsync("api/accounts", account);
            return;
        }

        await http.PutAsJsonAsync($"api/accounts/{account.Id}", account);
    }

    public async Task DeleteAccountAsync(int id)
    {
        await AttachTokenAsync();
        await http.DeleteAsync($"api/accounts/{id}");
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(TransactionFilter? filter = null)
    {
        await AttachTokenAsync();
        var query = new List<string>();

        if (filter?.Type is not null)
        {
            query.Add($"type={filter.Type}");
        }

        if (filter?.CategoryId is not null and > 0)
        {
            query.Add($"categoryId={filter.CategoryId}");
        }

        if (filter?.AccountId is not null and > 0)
        {
            query.Add($"accountId={filter.AccountId}");
        }

        if (filter?.From is not null)
        {
            query.Add($"from={filter.From:yyyy-MM-dd}");
        }

        if (filter?.To is not null)
        {
            query.Add($"to={filter.To:yyyy-MM-dd}");
        }

        if (!string.IsNullOrWhiteSpace(filter?.Search))
        {
            query.Add($"search={Uri.EscapeDataString(filter.Search)}");
        }

        var url = query.Count == 0 ? "api/transactions" : $"api/transactions?{string.Join("&", query)}";
        return await http.GetFromJsonAsync<List<TransactionDto>>(url) ?? [];
    }

    public async Task SaveTransactionAsync(TransactionDto transaction)
    {
        await AttachTokenAsync();
        if (transaction.Id == 0)
        {
            await http.PostAsJsonAsync("api/transactions", transaction);
            return;
        }

        await http.PutAsJsonAsync($"api/transactions/{transaction.Id}", transaction);
    }

    public async Task DeleteTransactionAsync(int id)
    {
        await AttachTokenAsync();
        await http.DeleteAsync($"api/transactions/{id}");
    }

    public async Task<List<BudgetDto>> GetBudgetsAsync(int? month = null, int? year = null)
    {
        await AttachTokenAsync();
        var url = month is null || year is null ? "api/budgets" : $"api/budgets?month={month}&year={year}";
        return await http.GetFromJsonAsync<List<BudgetDto>>(url) ?? [];
    }

    public async Task SaveBudgetAsync(BudgetDto budget)
    {
        await AttachTokenAsync();
        if (budget.Id == 0)
        {
            await http.PostAsJsonAsync("api/budgets", budget);
            return;
        }

        await http.PutAsJsonAsync($"api/budgets/{budget.Id}", budget);
    }

    public async Task DeleteBudgetAsync(int id)
    {
        await AttachTokenAsync();
        await http.DeleteAsync($"api/budgets/{id}");
    }

    public async Task<SummaryDto> GetSummaryAsync()
    {
        await AttachTokenAsync();
        return await http.GetFromJsonAsync<SummaryDto>("api/statistics/summary") ?? new SummaryDto();
    }

    public async Task<List<CategorySummaryDto>> GetCategorySummaryAsync(int? month = null, int? year = null)
    {
        await AttachTokenAsync();
        var url = month is null || year is null
            ? "api/statistics/by-category"
            : $"api/statistics/by-category?month={month}&year={year}";
        return await http.GetFromJsonAsync<List<CategorySummaryDto>>(url) ?? [];
    }

    public async Task<List<MonthlySummaryDto>> GetMonthlySummaryAsync()
    {
        await AttachTokenAsync();
        return await http.GetFromJsonAsync<List<MonthlySummaryDto>>("api/statistics/by-month") ?? [];
    }

    private async Task AttachTokenAsync()
    {
        var token = await tokenStore.GetTokenAsync();
        http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }
}
