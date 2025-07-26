namespace AccountService.Features.Exceptions;

public class AccountException : Exception
{
    public AccountException(string message) : base(message) { }
}