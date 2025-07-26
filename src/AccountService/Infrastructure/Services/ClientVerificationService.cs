using AccountService.Features.Interfaces;

namespace AccountService.Infrastructure.Services;

public class ClientVerificationService : IClientVerificationService
{
    public Task<bool> VerifyClientAsync(Guid clientId)
    {
        return Task.FromResult(true);
    }
}