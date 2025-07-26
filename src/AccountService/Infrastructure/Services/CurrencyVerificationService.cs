
using AccountService.Features.Interfaces;

namespace AccountService.Infrastructure.Services;

public class CurrencyVerificationService : ICurrencyVerificationService
{
    private static readonly HashSet<string> Currencies = ["RUB", "USD", "EUR"];
    
    public Task<bool> VerifyCurrencyAsync(string currency, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(!string.IsNullOrEmpty(currency) && Currencies.Contains(currency.ToUpper()));
    }
}