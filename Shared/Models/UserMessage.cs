using System;
using System.Text.RegularExpressions;

namespace Shared.Models;

/// <summary>
/// Modelo de mensagem para usuários.
/// Contém informações sobre o usuário enviado pelo UserSender.
/// </summary>
public class UserMessage
{
    /// <summary>
    /// Nome completo do usuário.
    /// Campo obrigatório para validação.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Endereço do usuário.
    /// Campo obrigatório para validação.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// RG do usuário.
    /// Campo obrigatório para validação.
    /// </summary>
    public string RG { get; set; } = string.Empty;

    /// <summary>
    /// CPF do usuário.
    /// Campo obrigatório para validação.
    /// </summary>
    public string CPF { get; set; } = string.Empty;

    /// <summary>
    /// Data e hora de registro do usuário.
    /// Campo obrigatório para validação.
    /// </summary>
    public DateTime RegistrationTimestamp { get; set; }

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public UserMessage()
    {
        RegistrationTimestamp = DateTime.Now;
    }

    /// <summary>
    /// Construtor com parâmetros para facilitar a criação de instâncias
    /// </summary>
    /// <param name="fullName">Nome completo</param>
    /// <param name="address">Endereço</param>
    /// <param name="rg">RG</param>
    /// <param name="cpf">CPF</param>
    public UserMessage(string fullName, string address, string rg, string cpf)
    {
        FullName = fullName;
        Address = address;
        RG = rg;
        CPF = cpf;
        RegistrationTimestamp = DateTime.Now;
    }

    /// <summary>
    /// Valida se todos os campos obrigatórios estão preenchidos
    /// </summary>
    /// <returns>True se válido, False caso contrário</returns>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FullName) &&
               !string.IsNullOrWhiteSpace(Address) &&
               !string.IsNullOrWhiteSpace(RG) &&
               !string.IsNullOrWhiteSpace(CPF) &&
               RegistrationTimestamp != default &&
               IsValidCPF(CPF);
    }

    /// <summary>
    /// Valida o formato do CPF (apenas números, 11 dígitos)
    /// </summary>
    /// <param name="cpf">CPF a ser validado</param>
    /// <returns>True se válido, False caso contrário</returns>
    private bool IsValidCPF(string cpf)
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
    /// Retorna uma representação em string do objeto
    /// </summary>
    public override string ToString()
    {
        return $"Nome: {FullName} | Endereço: {Address} | RG: {RG} | CPF: {CPF} | Registro: {RegistrationTimestamp:dd/MM/yyyy HH:mm:ss}";
    }
}

// Made with Bob
