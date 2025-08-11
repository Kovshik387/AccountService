using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AccountService.IntegrationTests.Fakers;

public class AuthFaker : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public new const string Scheme = "Test";

    [Obsolete("Obsolete")]
    public AuthFaker(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity([
            new Claim(ClaimTypes.NameIdentifier, "test"),
            new Claim(ClaimTypes.Name, "integration-user")
        ], Scheme);

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}