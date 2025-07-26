namespace AccountService.Features.Interfaces;

public interface ICurrencyVerificationService
{
    public Task<bool> VerifyCurrencyAsync(string currency, CancellationToken cancellationToken);
}