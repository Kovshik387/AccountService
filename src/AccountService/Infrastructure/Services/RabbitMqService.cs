using System.Text;
using System.Text.Json;
using AccountService.Converters;
using AccountService.Features.Interfaces;
using AccountService.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AccountService.Infrastructure.Services;

public sealed class RabbitMqService : IMessageBus, IAsyncDisposable, IDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private readonly JsonSerializerOptions _jsonSerializerOptions = JsonOption.Options;

    private readonly ILogger<RabbitMqService> _logger;

    public RabbitMqService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqService> logger)
    {
        var settings = options.Value;
        _factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
            VirtualHost = settings.VHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
        };
        _logger = logger;
    }

    public async Task Subscribe<T>(string queueName, OnDataReceiveEvent<T>? onReceive,
        CancellationToken cancellationToke)
    {
        if (onReceive is null) return;

        var connection = await EnsureConnection();
        var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToke);
        await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: cancellationToke);
        await channel.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken: cancellationToke);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var model = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
                if (model is null) throw new JsonException("Deserialized null");

                await onReceive(model);
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: cancellationToke);
            }
            catch (JsonException)
            {
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false,
                    cancellationToken: cancellationToke);
            }
            catch (Exception)
            {
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true,
                    cancellationToken: cancellationToke);
            }
        };

        _ = await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumerTag: string.Empty,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer,
            cancellationToken: cancellationToke);
    }

    public async Task PushAsync<T>(string queueName, T data, CancellationToken cancellationToken)
    {
        var connection = await EnsureConnection();

        var options = new CreateChannelOptions(true, true);

        await using var channel = await connection.CreateChannelAsync(options, cancellationToken);

        await channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false,
            cancellationToken: cancellationToken);
        var props = new BasicProperties
        {
            Persistent = true
        };

        var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, _jsonSerializerOptions));
        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: props,
            body: payload, cancellationToken: cancellationToken);
    }

    private async Task<IConnection> EnsureConnection()
    {
        if (_connection is not { IsOpen: true })
            _connection = await _factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        try
        {
            _connection?.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Close error {ex.Message}");
        }

        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_connection is not null) await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Close error {ex.Message}");
        }

        if (_connection is not null) await _connection.DisposeAsync();
    }
}