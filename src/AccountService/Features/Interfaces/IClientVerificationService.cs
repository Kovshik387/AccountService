namespace AccountService.Features.Interfaces;

public interface IClientVerificationService
{
    public Task<bool> VerifyClientAsync(Guid clientId);
}