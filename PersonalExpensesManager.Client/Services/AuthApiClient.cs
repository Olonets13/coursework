using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using PersonalExpensesManager.Shared.Dto;

namespace PersonalExpensesManager.Client.Services;

public class AuthApiClient(HttpClient http, AuthTokenStore tokenStore, AuthenticationStateProvider stateProvider)
{
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", dto);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>() ?? new AuthResponseDto();
        await tokenStore.SaveAsync(auth);
        Notify();
        return auth;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", dto);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
        }

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>() ?? new AuthResponseDto();
        await tokenStore.SaveAsync(auth);
        Notify();
        return auth;
    }

    public async Task LogoutAsync()
    {
        await tokenStore.ClearAsync();
        Notify();
    }

    public async Task<CurrentUserDto?> GetStoredUserAsync()
    {
        return await tokenStore.GetUserAsync();
    }

    private void Notify()
    {
        if (stateProvider is AppAuthenticationStateProvider appProvider)
        {
            appProvider.NotifyUserChanged();
        }
    }
}
