namespace AccountService.Features.Interfaces;

public interface ICorrelationMessage
{
    Guid MessageId { get; set; }     
    string? CorrelationId { get; set; }
    string? CausationId { get; set; } 
}