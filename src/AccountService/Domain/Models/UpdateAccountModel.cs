namespace AccountService.Domain.Models;

public record UpdateAccountModel
{
    public required Guid Id { get; init; }
    public decimal InterestRate { get; init; }
    public uint XMin { get; init; }
}