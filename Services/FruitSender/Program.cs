using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Shared.Models;
using Shared.RabbitMQ;

namespace FruitSender;

/// <summary>
/// FruitSender - Sender1 para o fluxo de frutas da época.
/// Responsável por enviar mensagens contendo nome da fruta, descrição e timestamp.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              HORTIFRUTI SYSTEM - FRUIT SENDER              ║");
        Console.WriteLine("║                    (Sender1 - Frutas)                       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        using var rabbitConnection = new RabbitMQConnection();

        try
        {
            // Configura infraestrutura RabbitMQ
            Console.WriteLine("[SETUP] Configurando infraestrutura RabbitMQ para frutas...");
            rabbitConnection.SetupFruitInfrastructure();
            Console.WriteLine("[SETUP] Infraestrutura configurada com sucesso!");
            Console.WriteLine();

            var channel = rabbitConnection.GetChannel();

            while (true)
            {
                Console.WriteLine("════════════════════════════════════════════════════════════");
                Console.WriteLine("Opções:");
                Console.WriteLine("1 - Enviar fruta manualmente");
                Console.WriteLine("2 - Enviar frutas de exemplo");
                Console.WriteLine("0 - Sair");
                Console.Write("Escolha: ");

                var option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        SendFruitManually(channel);
                        break;
                    case "2":
                        SendExampleFruits(channel);
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
    /// Permite ao usuário inserir dados da fruta manualmente.
    /// </summary>
    static void SendFruitManually(IModel channel)
    {
        Console.WriteLine("\n--- Envio Manual de Fruta ---");

        Console.Write("Nome da fruta: ");
        var name = Console.ReadLine();

        Console.Write("Descrição: ");
        var description = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("[ERRO] Nome e descrição são obrigatórios!");
            return;
        }

        var fruitMessage = new FruitMessage
        {
            Name = name,
            Description = description,
            Timestamp = DateTime.Now
        };

        SendFruitMessage(channel, fruitMessage);
    }

    /// <summary>
    /// Envia algumas frutas de exemplo.
    /// </summary>
    static void SendExampleFruits(IModel channel)
    {
        var examples = new[]
        {
            new FruitMessage { Name = "Morango", Description = "Fruta vermelha e suculenta, rica em vitamina C", Timestamp = DateTime.Now },
            new FruitMessage { Name = "Manga", Description = "Fruta tropical doce e aromática", Timestamp = DateTime.Now },
            new FruitMessage { Name = "Abacaxi", Description = "Fruta cítrica com casca espinhosa e polpa amarela", Timestamp = DateTime.Now }
        };

        foreach (var fruit in examples)
        {
            SendFruitMessage(channel, fruit);
            System.Threading.Thread.Sleep(1000); // Pequena pausa entre envios
        }
    }

    /// <summary>
    /// Envia uma mensagem de fruta para a fila de validação.
    /// </summary>
    static void SendFruitMessage(IModel channel, FruitMessage fruit)
    {
        try
        {
            var messageJson = JsonConvert.SerializeObject(fruit);
            var body = Encoding.UTF8.GetBytes(messageJson);

            // Publica na exchange com routing key para validação
            channel.BasicPublish(
                exchange: RabbitMQConfig.FruitExchange,
                routingKey: RabbitMQConfig.FruitSenderRoutingKey,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[ENVIADO] Fruta '{fruit.Name}' enviada para validação às {fruit.Timestamp:HH:mm:ss}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao enviar fruta: {ex.Message}");
        }
    }
}

// Made with Bob
