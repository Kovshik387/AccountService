namespace AccountService.Features.Exceptions;

public class ConflictException : AccountException
{
    public ConflictException(string message = "Conflict") : base(message) { }
}