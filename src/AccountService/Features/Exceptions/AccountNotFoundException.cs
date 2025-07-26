namespace AccountService.Features.Exceptions;

public class AccountNotFoundException : AccountException
{
    public AccountNotFoundException(string message) : base(message) { }
}