using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PersonalExpensesManager.Client.Services;

public class AppAuthenticationStateProvider(AuthTokenStore tokenStore) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await tokenStore.GetUserAsync();
        var token = await tokenStore.GetTokenAsync();

        if (user is null || string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        ], "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
