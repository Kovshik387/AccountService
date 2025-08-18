namespace AccountService.Features.Interfaces;

public delegate Task OnDataReceiveEvent<in T>(T data);

public interface IMessageBus
{
    Task Subscribe<T>(string queueName, OnDataReceiveEvent<T>? onReceive, CancellationToken cancellationToken);

    Task PushAsync<T>(string queueName, T data, CancellationToken cancellationToken);
}