# Blazor Web App with Azure Easy Auth

This sample features:

- A Blazor Web App with Azure Easy Auth Support.
  - This adds a `AzureEasyAuthAuthenticationStateProvider` and `AzureEasyAuthAuthenticationStateProvider` services to the
    server and client Blazor apps respectively to capture authentication state and flow it between the server and client.
  - This fetches the user principal id stored in the request header by AzureEasyAuth and call the end point .auth/me to get the user info and user claims.
  - Creates a new `ClaimsIdentity` and `ClaimsPrincipal` and set as `AuthenticationState`
  - The `ClaimsPrincipal` is converted to `UserInfo` json to persist on the PesistantComponentState.
  - The client side `AuthenticationStateProvider` reads from `UserInfo` json and converts it to ClaimsPrincipal.
  - The role claims are used to perform authorization on the client side pages using `Policy`


## Article for this sample app

[App Service authentication recommendations - Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/identity-scenarios)

[Authentication and authorization - Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview-authentication-authorization)

[Configure Microsoft Entra authentication - Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad?tabs=workforce-configuration)

[OAuth tokens in AuthN/AuthZ - Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-oauth-tokens?source=recommendations)

[How to get user name for Blazor server applications (EasyAuth)? - Microsoft Q&A](https://learn.microsoft.com/en-us/answers/questions/1373805/how-to-get-user-name-for-blazor-server-application)

[ASP.NET Core Blazor authentication state](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/authentication-state?view=aspnetcore-8.0&pivots=server)

## Configure the sample

Setup Azure Easy Auth using the article mentioned [here](https://learn.microsoft.com/en-us/azure/app-service/configure-authentication-provider-aad?tabs=workforce-configuration) on any App Service. Deploy the application to AppService using Publish option in Visual Studio.

## Run the sample locally

### Visual Studio

1. Open the `BlazorWebAppOidc` solution file in Visual Studio.
1. Select the `BlazorWebAppOidc` project in **Solution Explorer** and start the app with either Visual Studio's Run button or by selecting **Start Debugging** from the **Debug** menu.

### App Service
Access the url from Azure App Service. It should automatically redirect the user to login page and get the user authenticated and return the userClaims. If the authentication and authorization is successful, the weather page will display the weather details.

### .NET CLI

In a command shell, navigate to the `BlazorWebAppOidc` project folder and use the `dotnet run` command to run the sample.
