using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Models;
using Shared.RabbitMQ;

namespace UserSender;

/// <summary>
/// UserSender - Sender2 para o fluxo de usuários.
/// Responsável por enviar mensagens contendo dados de cadastro de usuários.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              HORTIFRUTI SYSTEM - USER SENDER               ║");
        Console.WriteLine("║                    (Sender2 - Usuários)                     ║");
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

            while (true)
            {
                Console.WriteLine("════════════════════════════════════════════════════════════");
                Console.WriteLine("Opções:");
                Console.WriteLine("1 - Cadastrar usuário manualmente");
                Console.WriteLine("2 - Enviar usuários de exemplo");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        SendUserManually(channel);
                        break;
                    case "2":
                        SendExampleUsers(channel);
                        break;
                    case "0":
                        Console.WriteLine("Saindo...");
                        return;
                    default:
                        Console.WriteLine("Opção inválida!");
                        break;
                }

                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha na execução: {ex.Message}");
        }
    }

    /// <summary>
    /// Permite ao usuário inserir dados do usuário manualmente.
    /// </summary>
    static void SendUserManually(IModel channel)
    {
        Console.WriteLine("\n--- Cadastro Manual de Usuário ---");

        Console.Write("Nome completo: ");
        var fullName = Console.ReadLine();

        Console.Write("Endereço: ");
        var address = Console.ReadLine();

        Console.Write("RG: ");
        var rg = Console.ReadLine();

        Console.Write("CPF: ");
        var cpf = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(address) ||
            string.IsNullOrWhiteSpace(rg) || string.IsNullOrWhiteSpace(cpf))
        {
            Console.WriteLine("[ERRO] Todos os campos são obrigatórios!");
            return;
        }

        var userMessage = new UserMessage
        {
            FullName = fullName,
            Address = address,
            RG = rg,
            CPF = cpf,
            RegistrationTimestamp = DateTime.Now
        };

        SendUserMessage(channel, userMessage);
    }

    /// <summary>
    /// Envia alguns usuários de exemplo.
    /// </summary>
    static void SendExampleUsers(IModel channel)
    {
        var examples = new[]
        {
            new UserMessage
            {
                FullName = "João Silva",
                Address = "Rua das Flores, 123",
                RG = "12.345.678-9",
                CPF = "123.456.789-00",
                RegistrationTimestamp = DateTime.Now
            },
            new UserMessage
            {
                FullName = "Maria Santos",
                Address = "Av. Brasil, 456",
                RG = "98.765.432-1",
                CPF = "987.654.321-00",
                RegistrationTimestamp = DateTime.Now
            }
        };

        foreach (var user in examples)
        {
            SendUserMessage(channel, user);
            System.Threading.Thread.Sleep(1000); // Pequena pausa entre envios
        }
    }

    /// <summary>
    /// Envia uma mensagem de usuário para a fila de validação.
    /// </summary>
    static void SendUserMessage(IModel channel, UserMessage user)
    {
        try
        {
            var messageJson = JsonConvert.SerializeObject(user);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Publica na exchange com routing key para validação
            channel.BasicPublish(
                exchange: RabbitMQConfig.UserExchange,
                routingKey: RabbitMQConfig.UserSenderRoutingKey,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[ENVIADO] Usuário '{user.FullName}' enviado para validação às {user.RegistrationTimestamp:HH:mm:ss}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao enviar usuário: {ex.Message}");
        }
    }
}

// Made with Bob
