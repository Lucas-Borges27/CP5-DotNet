namespace Shared.RabbitMQ;

/// <summary>
/// Configurações centralizadas do RabbitMQ para o Sistema Hortifruti.
/// Define todas as constantes utilizadas para comunicação com o broker.
/// </summary>
public static class RabbitMQConfig
{
    // ========== CONFIGURAÇÕES DE CONEXÃO ==========

    /// <summary>
    /// Hostname do servidor RabbitMQ (localhost para desenvolvimento).
    /// </summary>
    public const string HostName = "localhost";

    /// <summary>
    /// Porta de comunicação AMQP (5672).
    /// </summary>
    public const int Port = 5672;

    /// <summary>
    /// Usuário padrão do RabbitMQ.
    /// </summary>
    public const string UserName = "guest";

    /// <summary>
    /// Senha padrão do RabbitMQ.
    /// </summary>
    public const string Password = "guest";

    // ========== FLUXO 1: FRUTAS DA ÉPOCA ==========

    /// <summary>
    /// Exchange para o fluxo de frutas (tipo direct).
    /// </summary>
    public const string FruitExchange = "fruit_exchange";

    /// <summary>
    /// Queue para frutas enviadas pelo Sender1 para validação.
    /// </summary>
    public const string FruitSenderToValidationQueue = "fruit_sender_to_validation";

    /// <summary>
    /// Queue para frutas validadas enviadas para o Receiver1.
    /// </summary>
    public const string FruitValidationToReceiverQueue = "fruit_validation_to_receiver";

    /// <summary>
    /// Routing key para enviar frutas do sender para validação.
    /// </summary>
    public const string FruitSenderRoutingKey = "fruit.sender";

    /// <summary>
    /// Routing key para enviar frutas validadas para o receiver.
    /// </summary>
    public const string FruitValidatedRoutingKey = "fruit.validated";

    // ========== FLUXO 2: USUÁRIOS ==========

    /// <summary>
    /// Exchange para o fluxo de usuários (tipo direct).
    /// </summary>
    public const string UserExchange = "user_exchange";

    /// <summary>
    /// Queue para usuários enviados pelo Sender2 para validação.
    /// </summary>
    public const string UserSenderToValidationQueue = "user_sender_to_validation";

    /// <summary>
    /// Queue para usuários validados enviados para o Receiver2.
    /// </summary>
    public const string UserValidationToReceiverQueue = "user_validation_to_receiver";

    /// <summary>
    /// Routing key para enviar usuários do sender para validação.
    /// </summary>
    public const string UserSenderRoutingKey = "user.sender";

    /// <summary>
    /// Routing key para enviar usuários validados para o receiver.
    /// </summary>
    public const string UserValidatedRoutingKey = "user.validated";
}


