using System.Text;
using System.Text.Json;
using Elastic.Clients.Elasticsearch.Aggregations;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.RabbitMq.Configuration;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string Host { get; init; } = string.Empty;

    public int Port { get; init; } = 5672;

    public string Username { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string VirtualHost { get; init; } = "/";
}

internal sealed class RabbitMqConnection : IAsyncDisposable
{
    private readonly ConnectionFactory _factory;

    private IConnection? _connection;

    private readonly SemaphoreSlim _lock = new(1, 1);

    public RabbitMqConnection(IOptions<RabbitMqOptions> options)
    {
        RabbitMqOptions settings = options.Value;

        _factory = new ConnectionFactory
        {
            HostName = settings.Host,
            Port = settings.Port,
            UserName = settings.Username,
            Password = settings.Password,
            VirtualHost = settings.VirtualHost
        };
    }

    public async Task<IConnection> GetAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _lock.WaitAsync(cancellationToken);

        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            _connection = await _factory.CreateConnectionAsync(cancellationToken);

            return _connection;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

        _lock.Dispose();
    }
}

