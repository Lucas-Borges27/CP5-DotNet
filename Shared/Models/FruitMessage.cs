using System;

namespace Shared.Models;

/// <summary>
/// Modelo de mensagem para frutas da época.
/// Contém informações sobre a fruta enviada pelo FruitSender.
/// </summary>
public class FruitMessage
{
    /// <summary>
    /// Nome da fruta (ex: "Maçã", "Banana").
    /// Campo obrigatório para validação.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição resumida da fruta (ex: "Fruta vermelha e suculenta").
    /// Campo obrigatório para validação.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data e horário atual do sistema quando a mensagem foi enviada.
    /// Campo obrigatório para validação.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public FruitMessage()
    {
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Construtor com parâmetros para facilitar a criação de instâncias
    /// </summary>
    /// <param name="name">Nome da fruta</param>
    /// <param name="description">Descrição da fruta</param>
    public FruitMessage(string name, string description)
    {
        Name = name;
        Description = description;
        Timestamp = DateTime.Now;
    }

    /// <summary>
    /// Valida se todos os campos obrigatórios estão preenchidos
    /// </summary>
    /// <returns>True se válido, False caso contrário</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name) &&
               !string.IsNullOrWhiteSpace(Description) &&
               Timestamp != default;
    }

    /// <summary>
    /// Retorna uma representação em string do objeto
    /// </summary>
    public override string ToString()
    {
        return $"Fruta: {Name} | Descrição: {Description} | Data/Hora: {Timestamp:dd/MM/yyyy HH:mm:ss}";
    }
}


