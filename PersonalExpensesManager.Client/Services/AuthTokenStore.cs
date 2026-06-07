using System.Text.Json;
using Microsoft.JSInterop;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Client.Services;

public class AuthTokenStore(IJSRuntime js)
{
    private const string TokenKey = "personalExpenses.token";
    private const string UserKey = "personalExpenses.user";

    public async Task SaveAsync(AuthResponseDto response)
    {
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, response.Token);
        await js.InvokeVoidAsync("localStorage.setItem", UserKey, JsonSerializer.Serialize(response.User));
    }

    public async Task<string?> GetTokenAsync()
    {
        return await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
    }

    public async Task<CurrentUserDto?> GetUserAsync()
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", UserKey);
        return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<CurrentUserDto>(json);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        await js.InvokeVoidAsync("localStorage.removeItem", UserKey);
    }
}
