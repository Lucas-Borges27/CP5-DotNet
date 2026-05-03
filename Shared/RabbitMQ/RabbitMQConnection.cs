using RabbitMQ.Client;
using System;

namespace Shared.RabbitMQ;

/// <summary>
/// Classe responsável por gerenciar a conexão com o RabbitMQ.
/// Implementa métodos para criar conexões, canais e configurar a infraestrutura.
/// </summary>
public class RabbitMQConnection : IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private bool _disposed = false;

    /// <summary>
    /// Obtém ou cria uma conexão com o RabbitMQ.
    /// </summary>
    /// <returns>Conexão ativa com o RabbitMQ.</returns>
    public IConnection GetConnection()
    {
        if (_connection == null || !_connection.IsOpen)
        {
            var factory = new ConnectionFactory
            {
                HostName = RabbitMQConfig.HostName,
                Port = RabbitMQConfig.Port,
                UserName = RabbitMQConfig.UserName,
                Password = RabbitMQConfig.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60)
            };

            try
            {
                _connection = factory.CreateConnection();
                Console.WriteLine($"[RabbitMQ] Conexão estabelecida com sucesso em {RabbitMQConfig.HostName}:{RabbitMQConfig.Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Falha ao conectar ao RabbitMQ: {ex.Message}");
                throw;
            }
        }

        return _connection;
    }

    /// <summary>
    /// Obtém ou cria um canal de comunicação com o RabbitMQ.
    /// </summary>
    /// <returns>Canal ativo para comunicação.</returns>
    public IModel GetChannel()
    {
        if (_channel == null || _channel.IsClosed)
        {
            var connection = GetConnection();
            _channel = connection.CreateModel();
            Console.WriteLine("[RabbitMQ] Canal criado com sucesso");
        }

        return _channel;
    }

    /// <summary>
    /// Configura a infraestrutura do RabbitMQ para o fluxo de frutas.
    /// Cria exchange, filas e bindings necessários.
    /// </summary>
    public void SetupFruitInfrastructure()
    {
        var channel = GetChannel();

        try
        {
            // Declara a exchange para frutas (tipo direct)
            channel.ExchangeDeclare(
                exchange: RabbitMQConfig.FruitExchange,
                type: "direct",
                durable: true,
                autoDelete: false
            );
            Console.WriteLine($"[RabbitMQ] Exchange '{RabbitMQConfig.FruitExchange}' declarada");

            // Declara a fila sender to validation
            channel.QueueDeclare(
                queue: RabbitMQConfig.FruitSenderToValidationQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Console.WriteLine($"[RabbitMQ] Fila '{RabbitMQConfig.FruitSenderToValidationQueue}' declarada");

            // Declara a fila validation to receiver
            channel.QueueDeclare(
                queue: RabbitMQConfig.FruitValidationToReceiverQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Console.WriteLine($"[RabbitMQ] Fila '{RabbitMQConfig.FruitValidationToReceiverQueue}' declarada");

            // Binding para sender to validation
            channel.QueueBind(
                queue: RabbitMQConfig.FruitSenderToValidationQueue,
                exchange: RabbitMQConfig.FruitExchange,
                routingKey: RabbitMQConfig.FruitSenderRoutingKey
            );
            Console.WriteLine($"[RabbitMQ] Binding criado: {RabbitMQConfig.FruitSenderToValidationQueue} -> {RabbitMQConfig.FruitExchange} com routing key '{RabbitMQConfig.FruitSenderRoutingKey}'");

            // Binding para validation to receiver
            channel.QueueBind(
                queue: RabbitMQConfig.FruitValidationToReceiverQueue,
                exchange: RabbitMQConfig.FruitExchange,
                routingKey: RabbitMQConfig.FruitValidatedRoutingKey
            );
            Console.WriteLine($"[RabbitMQ] Binding criado: {RabbitMQConfig.FruitValidationToReceiverQueue} -> {RabbitMQConfig.FruitExchange} com routing key '{RabbitMQConfig.FruitValidatedRoutingKey}'");

            Console.WriteLine("[RabbitMQ] Infraestrutura de frutas configurada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao configurar infraestrutura de frutas: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Configura a infraestrutura do RabbitMQ para o fluxo de usuários.
    /// Cria exchange, filas e bindings necessários.
    /// </summary>
    public void SetupUserInfrastructure()
    {
        var channel = GetChannel();

        try
        {
            // Declara a exchange para usuários (tipo direct)
            channel.ExchangeDeclare(
                exchange: RabbitMQConfig.UserExchange,
                type: "direct",
                durable: true,
                autoDelete: false
            );
            Console.WriteLine($"[RabbitMQ] Exchange '{RabbitMQConfig.UserExchange}' declarada");

            // Declara a fila sender to validation
            channel.QueueDeclare(
                queue: RabbitMQConfig.UserSenderToValidationQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Console.WriteLine($"[RabbitMQ] Fila '{RabbitMQConfig.UserSenderToValidationQueue}' declarada");

            // Declara a fila validation to receiver
            channel.QueueDeclare(
                queue: RabbitMQConfig.UserValidationToReceiverQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            Console.WriteLine($"[RabbitMQ] Fila '{RabbitMQConfig.UserValidationToReceiverQueue}' declarada");

            // Binding para sender to validation
            channel.QueueBind(
                queue: RabbitMQConfig.UserSenderToValidationQueue,
                exchange: RabbitMQConfig.UserExchange,
                routingKey: RabbitMQConfig.UserSenderRoutingKey
            );
            Console.WriteLine($"[RabbitMQ] Binding criado: {RabbitMQConfig.UserSenderToValidationQueue} -> {RabbitMQConfig.UserExchange} com routing key '{RabbitMQConfig.UserSenderRoutingKey}'");

            // Binding para validation to receiver
            channel.QueueBind(
                queue: RabbitMQConfig.UserValidationToReceiverQueue,
                exchange: RabbitMQConfig.UserExchange,
                routingKey: RabbitMQConfig.UserValidatedRoutingKey
            );
            Console.WriteLine($"[RabbitMQ] Binding criado: {RabbitMQConfig.UserValidationToReceiverQueue} -> {RabbitMQConfig.UserExchange} com routing key '{RabbitMQConfig.UserValidatedRoutingKey}'");

            Console.WriteLine("[RabbitMQ] Infraestrutura de usuários configurada com sucesso");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao configurar infraestrutura de usuários: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Libera os recursos utilizados pela conexão e canal.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            if (_channel != null && _channel.IsOpen)
            {
                _channel.Close();
                Console.WriteLine("[RabbitMQ] Canal fechado");
            }

            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                Console.WriteLine("[RabbitMQ] Conexão fechada");
            }

            _disposed = true;
        }
    }
}

// Made with Bob
