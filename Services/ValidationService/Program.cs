using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using Shared.RabbitMQ;

namespace ValidationService;

/// <summary>
/// ValidationService - Serviço de validação para ambos os fluxos.
/// Consome mensagens dos senders, valida os dados e reenvia para os receivers se válido.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║            HORTIFRUTI SYSTEM - VALIDATION SERVICE         ║");
        Console.WriteLine("║              Validação de Dados (Frutas e Usuários)        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        using var rabbitConnection = new RabbitMQConnection();

        try
        {
            // Configura infraestrutura para ambos os fluxos
            Console.WriteLine("[SETUP] Configurando infraestrutura RabbitMQ...");
            rabbitConnection.SetupFruitInfrastructure();
            rabbitConnection.SetupUserInfrastructure();
            Console.WriteLine("[SETUP] Infraestrutura configurada com sucesso!");
            Console.WriteLine();

            var channel = rabbitConnection.GetChannel();

            // Configura consumidores para frutas
            SetupFruitConsumer(channel);

            // Configura consumidores para usuários
            SetupUserConsumer(channel);

            Console.WriteLine("[VALIDATION] Serviço de validação iniciado. Aguardando mensagens...");
            Console.WriteLine("Pressione Ctrl+C para sair.");

            // Mantém o serviço rodando
            var waitHandle = new System.Threading.ManualResetEvent(false);
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                waitHandle.Set();
            };
            waitHandle.WaitOne();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha na execução: {ex.Message}");
        }
    }

    /// <summary>
    /// Configura o consumidor para mensagens de frutas.
    /// </summary>
    static void SetupFruitConsumer(IModel channel)
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var fruitMessage = JsonConvert.DeserializeObject<FruitMessage>(messageJson);

                if (fruitMessage == null)
                {
                    Console.WriteLine("[VALIDATION] [FRUTA] Mensagem inválida recebida");
                    return;
                }

                Console.WriteLine($"[VALIDATION] [FRUTA] Recebida: {fruitMessage.Name}");

                // Validação: verifica se todos os campos estão preenchidos
                if (ValidateFruitMessage(fruitMessage))
                {
                    // Reenvia para o receiver
                    SendValidatedFruit(channel, fruitMessage);
                    Console.WriteLine($"[VALIDATION] [FRUTA] '{fruitMessage.Name}' validada e enviada para receiver");
                }
                else
                {
                    Console.WriteLine($"[VALIDATION] [FRUTA] '{fruitMessage.Name}' rejeitada - dados inválidos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] [FRUTA] Falha no processamento: {ex.Message}");
            }
        };

        channel.BasicConsume(
            queue: RabbitMQConfig.FruitSenderToValidationQueue,
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine($"[VALIDATION] Consumidor de frutas configurado para fila: {RabbitMQConfig.FruitSenderToValidationQueue}");
    }

    /// <summary>
    /// Configura o consumidor para mensagens de usuários.
    /// </summary>
    static void SetupUserConsumer(IModel channel)
    {
        var consumer = new EventingBasicConsumer(channel);

        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var userMessage = JsonConvert.DeserializeObject<UserMessage>(messageJson);

                if (userMessage == null)
                {
                    Console.WriteLine("[VALIDATION] [USUÁRIO] Mensagem inválida recebida");
                    return;
                }

                Console.WriteLine($"[VALIDATION] [USUÁRIO] Recebido: {userMessage.FullName}");

                // Validação: verifica se todos os campos estão preenchidos
                if (ValidateUserMessage(userMessage))
                {
                    // Reenvia para o receiver
                    SendValidatedUser(channel, userMessage);
                    Console.WriteLine($"[VALIDATION] [USUÁRIO] '{userMessage.FullName}' validado e enviado para receiver");
                }
                else
                {
                    Console.WriteLine($"[VALIDATION] [USUÁRIO] '{userMessage.FullName}' rejeitado - dados inválidos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] [USUÁRIO] Falha no processamento: {ex.Message}");
            }
        };

        channel.BasicConsume(
            queue: RabbitMQConfig.UserSenderToValidationQueue,
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine($"[VALIDATION] Consumidor de usuários configurado para fila: {RabbitMQConfig.UserSenderToValidationQueue}");
    }

    /// <summary>
    /// Valida os dados da mensagem de fruta.
    /// </summary>
    static bool ValidateFruitMessage(FruitMessage fruit)
    {
        return !string.IsNullOrWhiteSpace(fruit.Name) &&
               !string.IsNullOrWhiteSpace(fruit.Description) &&
               fruit.Timestamp != default;
    }

    /// <summary>
    /// Valida os dados da mensagem de usuário.
    /// </summary>
    static bool ValidateUserMessage(UserMessage user)
    {
        return !string.IsNullOrWhiteSpace(user.FullName) &&
               !string.IsNullOrWhiteSpace(user.Address) &&
               !string.IsNullOrWhiteSpace(user.RG) &&
               !string.IsNullOrWhiteSpace(user.CPF) &&
               user.RegistrationTimestamp != default &&
               IsValidCPF(user.CPF);
    }

    /// <summary>
    /// Valida o formato do CPF (apenas números, 11 dígitos e dígitos verificadores).
    /// </summary>
    static bool IsValidCPF(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        cpf = Regex.Replace(cpf, @"[^\d]", "");

        // Verifica se tem 11 dígitos
        if (cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais (CPF inválido)
        bool allSame = true;
        for (int i = 1; i < 11; i++)
        {
            if (cpf[i] != cpf[0])
            {
                allSame = false;
                break;
            }
        }
        if (allSame)
            return false;

        // Validação dos dígitos verificadores
        int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        int resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        string digito = resto.ToString();
        tempCpf = tempCpf + digito;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito = digito + resto.ToString();

        return cpf.EndsWith(digito);
    }

    /// <summary>
    /// Envia fruta validada para o receiver.
    /// </summary>
    static void SendValidatedFruit(IModel channel, FruitMessage fruit)
    {
        var messageJson = JsonConvert.SerializeObject(fruit);
        var body = Encoding.UTF8.GetBytes(messageJson);

        channel.BasicPublish(
            exchange: RabbitMQConfig.FruitExchange,
            routingKey: RabbitMQConfig.FruitValidatedRoutingKey,
            basicProperties: null,
            body: body
        );
    }

    /// <summary>
    /// Envia usuário validado para o receiver.
    /// </summary>
    static void SendValidatedUser(IModel channel, UserMessage user)
    {
        var messageJson = JsonConvert.SerializeObject(user);
        var body = Encoding.UTF8.GetBytes(messageJson);

        channel.BasicPublish(
            exchange: RabbitMQConfig.UserExchange,
            routingKey: RabbitMQConfig.UserValidatedRoutingKey,
            basicProperties: null,
            body: body
        );
    }
}