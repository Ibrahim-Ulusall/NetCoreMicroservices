using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace EventBus.RabbitMQ;
public class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IServiceProvider _serviceProvider;
    private ILogger _logger;
    private readonly int _retryCount;
    private IConnection _connection;
    private bool _disposed;
    private object lockObject = new object();
    public bool IsConnected => _connection != null && _connection.IsOpen;
    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5, IServiceProvider serviceProvider)
    {
        _logger = _serviceProvider.GetService(typeof(ILogger<RabbitMQPersistentConnection>)) as ILogger<RabbitMQPersistentConnection>;
        _connectionFactory = connectionFactory;
        _serviceProvider = serviceProvider;
        _retryCount = retryCount;
    }

    public IModel CreateModel() => _connection.CreateModel();
    public bool TryConnect()
    {
        lock (lockObject)
        {
            var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogError("RabbitMQPersistentConnection Messasge : RabbitMQ bağlantısı başarısız . {message}", ex.Message);
                });
            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();
            });
            if (IsConnected)
            {
                _connection.ConnectionShutdown += ConnectionShutdown;
                _connection.ConnectionBlocked += ConnectionBlocked;
                _connection.CallbackException += CallbackException;
                _logger.LogInformation("RabbitMQ Connection Successfully");
                return true;
            }
            _logger.LogError("RabbitMQ Connection Fail");
            return false;
        }
    }

    private void CallbackException(object? sender, global::RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void ConnectionBlocked(object? sender, global::RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    private void ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        if (_disposed) return;
        TryConnect();
    }

    public void Dispose()
    {
        _disposed = true;
        _connection.Dispose();
    }
}
