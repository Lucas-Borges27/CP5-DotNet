using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using Shared.RabbitMQ;

namespace FruitReceiver;

/// <summary>
/// FruitReceiver - Receiver1 para o fluxo de frutas.
/// Consome mensagens validadas e exibe os dados das frutas.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║             HORTIFRUTI SYSTEM - FRUIT RECEIVER             ║");
        Console.WriteLine("║                    (Receiver1 - Frutas)                     ║");
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

            // Configura consumidor
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var fruitMessage = JsonConvert.DeserializeObject<FruitMessage>(messageJson);

                    if (fruitMessage != null)
                    {
                        DisplayFruitMessage(fruitMessage);
                    }
                    else
                    {
                        Console.WriteLine("[RECEIVER] [FRUTA] Mensagem inválida recebida");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERRO] [FRUTA] Falha no processamento: {ex.Message}");
                }
            };

            channel.BasicConsume(
                queue: RabbitMQConfig.FruitValidationToReceiverQueue,
                autoAck: true,
                consumer: consumer
            );

            Console.WriteLine($"[RECEIVER] Aguardando frutas validadas na fila: {RabbitMQConfig.FruitValidationToReceiverQueue}");
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
    /// Exibe os dados da fruta recebida.
    /// </summary>
    static void DisplayFruitMessage(FruitMessage fruit)
    {
        Console.WriteLine();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                    FRUTA RECEBIDA                          ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"Nome: {fruit.Name}");
        Console.WriteLine($"Descrição: {fruit.Description}");
        Console.WriteLine($"Data/Hora: {fruit.Timestamp:dd/MM/yyyy HH:mm:ss}");
        Console.WriteLine("════════════════════════════════════════════════════════════");
    }
}