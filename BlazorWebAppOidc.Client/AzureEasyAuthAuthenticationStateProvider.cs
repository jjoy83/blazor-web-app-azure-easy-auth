using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorWebAppOidc.Client
{
    using System.Security.Claims;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    internal sealed class AzureEasyAuthAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
          Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

        public AzureEasyAuthAuthenticationStateProvider(PersistentComponentState state)
        {
            Console.WriteLine("AzureEasyAuthAuthenticationStateProvider: inside authentication state provider on client");
            if (!state.TryTakeFromJson<UserInfo>(nameof(UserInfo), out var userInfo) || userInfo is null)
            {
                return;
            }
            Console.WriteLine("AzureEasyAuthAuthenticationStateProvider: userInfo is not null");
            authenticationStateTask = Task.FromResult(new AuthenticationState(userInfo.ToClaimsPrincipal()));
            Console.WriteLine($"AzureEasyAuthAuthenticationStateProvider: authentication task from userinfo {0}", authenticationStateTask.Result.User.Claims.FirstOrDefault());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
    }
}

