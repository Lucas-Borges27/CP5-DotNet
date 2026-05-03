using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using Shared.RabbitMQ;

namespace UserReceiver;

/// <summary>
/// UserReceiver - Receiver2 para o fluxo de usuários.
/// Consome mensagens validadas e exibe os dados dos usuários.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║             HORTIFRUTI SYSTEM - USER RECEIVER              ║");
        Console.WriteLine("║                    (Receiver2 - Usuários)                   ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        using var rabbitConnection = new RabbitMQConnection();

        try
        {
            // Configura infraestrutura RabbitMQ
            Console.WriteLine("[SETUP] Configurando infraestrutura RabbitMQ para usuários...");
            rabbitConnection.SetupUserInfrastructure();
            Console.WriteLine("[SETUP] Infraestrutura configurada com sucesso!");
            Console.WriteLine();

            var channel = rabbitConnection.GetChannel();

            // Configura consumidor
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var userMessage = JsonConvert.DeserializeObject<UserMessage>(messageJson);

                    if (userMessage != null)
                    {
                        DisplayUserMessage(userMessage);
                    }
                    else
                    {
                        Console.WriteLine("[RECEIVER] [USUÁRIO] Mensagem inválida recebida");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] [USUÁRIO] Falha no processamento: {ex.Message}");
                }
            };

            channel.BasicConsume(
                queue: RabbitMQConfig.UserValidationToReceiverQueue,
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine($"[RECEIVER] Aguardando usuários validados na fila: {RabbitMQConfig.UserValidationToReceiverQueue}");
            Console.WriteLine("Pressione Ctrl+C para sair.");

            // Mantém o receiver rodando
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
    /// Exibe os dados do usuário recebido.
    /// </summary>
    static void DisplayUserMessage(UserMessage user)
    {
        Console.WriteLine();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    USUÁRIO RECEBIDO                        ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"Nome Completo: {user.FullName}");
        Console.WriteLine($"Endereço: {user.Address}");
        Console.WriteLine($"RG: {user.RG}");
        Console.WriteLine($"CPF: {user.CPF}");
        Console.WriteLine($"Data/Hora Registro: {user.RegistrationTimestamp:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine("════════════════════════════════════════════════════════════");
    }
}