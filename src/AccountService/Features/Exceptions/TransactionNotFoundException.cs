namespace AccountService.Features.Exceptions;

public class TransactionNotFoundException : AccountException
{
    public TransactionNotFoundException(string message) : base(message) { }
}