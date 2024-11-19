using BlazorWebAppOidc.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Security.Claims;
using System.Net;
using System.Security.Principal;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace BlazorWebAppOidc
{
    internal sealed class AzureEasyAuthAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
    { 

        private readonly PersistentComponentState _persistentComponentState;
        private readonly PersistingComponentStateSubscription _subscription;
        private Task<AuthenticationState>? _authenticationStateTask;
        private IHttpContextAccessor _httpContextAccessor;

        public AzureEasyAuthAuthenticationStateProvider(PersistentComponentState state, IHttpContextAccessor httpContextAccessor)
        {
            _persistentComponentState = state;
            AuthenticationStateChanged += OnAuthenticationStateChanged;
            _subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
            _httpContextAccessor = httpContextAccessor;
        }

        //public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask ??
        //   throw new InvalidOperationException();

        private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
        {
            _authenticationStateTask = authenticationStateTask;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Create a user on current thread from provided header
            var context = _httpContextAccessor.HttpContext;
            var user = new ClaimsPrincipal(new ClaimsIdentity());
           
            if (context != null && context.Request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL-ID"))
            {
                // Read headers from Azure
                var azureAppServicePrincipalIdHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"][0];
                var azureAppServicePrincipalNameHeader = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"][0];

                #region extract claims via call /.auth/me
                //invoke /.auth/me
                var cookieContainer = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer
                };
                string uriString = $"{context.Request.Scheme}://{context.Request.Host}";
                foreach (var c in context.Request.Cookies)
                {
                    cookieContainer.Add(new Uri(uriString), new Cookie(c.Key, c.Value));
                }
                string jsonResult = string.Empty;
                using (HttpClient client = new HttpClient(handler))
                {
                    var res = await client.GetAsync($"{uriString}/.auth/me");
                    jsonResult = await res.Content.ReadAsStringAsync();
                }

                //parse json
                var obj = JsonArray.Parse(jsonResult);
                string user_id = obj[0]["user_id"].GetValue<string>(); //user_id

                // Create claims id
                List<Claim> claims = new List<Claim>();
                foreach (var claim in obj[0]["user_claims"].AsArray())
                {
                    claims.Add(new Claim(claim["typ"].ToString(), claim["val"].ToString()));
                }

                // Set user in current context as claims principal
                var identity = new ClaimsIdentity(azureAppServicePrincipalNameHeader);
                identity.AddClaims(claims);
                #endregion

                // Set current thread user to identity
                user = new ClaimsPrincipal(identity);
            };

            return new AuthenticationState(user);
        }

        public void SetAuthenticationState(Task<AuthenticationState> task)
        {
            _authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            var authenticationState = await GetAuthenticationStateAsync();
            var principal = authenticationState.User;
            Console.WriteLine("AzureEasyAuthAuthenticationStateProvider: inside onPersistingAsync on server");

            if (principal.Identity?.IsAuthenticated == true)
            {
                _persistentComponentState.PersistAsJson(nameof(UserInfo), UserInfo.FromClaimsPrincipal(principal));
                Console.WriteLine("AzureEasyAuthAuthenticationStateProvider: after persisting on server");
            }
        }

        public void Dispose()
        {
            _subscription.Dispose();
        }
    }
}
