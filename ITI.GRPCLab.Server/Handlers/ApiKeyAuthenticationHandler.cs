
using ITI.GRPCLab.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ITI.AuthDemo.Server.Handlers;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyAuthenticationService _apiKeyAuthenticationService;
    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyAuthenticationService apiKeyAuthenticationService) : base(options, logger, encoder, clock)
    {
        _apiKeyAuthenticationService = apiKeyAuthenticationService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        //var isAuthenticationAttributeExists = Context.GetEndpoint()?.Metadata.GetMetadata<IAuthenticationFeature>() != null;

        //if (!isAuthenticationAttributeExists)
        //{
        //    return Task.FromResult(AuthenticateResult.NoResult());
        //}

        // Check if API Key is present in the request headers
        var isAuthenticated = _apiKeyAuthenticationService.Authenticate();

        if (!isAuthenticated)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        // create user identity
        var claims = new[] 
        { 
            new Claim(ClaimTypes.Name, "API UserName"),
            new Claim(ClaimTypes.Role, "API User")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);

        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));

    }
}
