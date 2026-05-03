# Sistema de Gestão Hortifruti

Sistema completo em C# (.NET 10.0) com arquitetura de microservices utilizando RabbitMQ para mensageria assíncrona. Implementa dois fluxos independentes: gestão de frutas da época e cadastro de usuários com validação robusta de CPF.
## 👥 Participantes

| Nome | RM |
|------|-----|
| Bruno Carlos Soares | RM559250 |
| Davi Praxedes Santos | RM563719 |
| João dos Santos Cardoso de Jesus | RM560400 |
| Kauê Vinicius Samartino da Silva | RM559317 |
| Lucas Borges de Souza | RM560027 |
| Pedro Henrique da Silva | RM560393 |

## 🎥 Vídeo de Apresentação

**Link do Vídeo:** [Assistir no YouTube](https://youtu.be/7R_d6W8Crnk?si=e_7i-RT7OH7ZYQLc)

> 📹 Vídeo demonstrando a arquitetura, execução e testes do sistema

---


##  Índice

- [Arquitetura do Sistema](#-arquitetura-do-sistema)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação e Execução](#-instalação-e-execução)
- [Configuração RabbitMQ](#-configuração-rabbitmq)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Modelos de Dados](#-modelos-de-dados)
- [Testes](#-testes)
- [Monitoramento](#-monitoramento)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)

---

## 🏗️ Arquitetura do Sistema

### Visão Geral

```
┌─────────────────────────────────────────────────────────────────┐
│                    SISTEMA HORTIFRUTI                           │
│                  MICROSERVICES + RABBITMQ                       │
└─────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┼───────────────┐
                    │               │               │
          ┌─────────▼─────────┐   ┌─▼─────────────┐
          │   FLUXO FRUTAS    │   │ FLUXO USUÁRIOS │
          └───────────────────┘   └────────────────┘
```

### Fluxos Independentes

#### FLUXO 1 - FRUTAS DA ÉPOCA
```
FruitSender ──(RabbitMQ)──► ValidationService ──(RabbitMQ)──► FruitReceiver
     │                            │                                │
     └─ Envia dados              └─ Valida dados                  └─ Exibe dados
        - Nome                      - Nome obrigatório               - Frutas validadas
        - Descrição                 - Descrição obrigatória          - Formatação
        - Timestamp                 - Timestamp válido                - Logs
```

#### FLUXO 2 - USUÁRIOS
```
UserSender ───(RabbitMQ)───► ValidationService ──(RabbitMQ)───► UserReceiver
     │                            │                                │
     └─ Envia dados              └─ Valida dados                  └─ Exibe dados
        - Nome Completo             - Nome obrigatório               - Usuários validados
        - Endereço                  - Endereço obrigatório           - Formatação
        - RG                        - RG obrigatório                 - Logs
        - CPF                       - CPF obrigatório
        - Timestamp                 - CPF válido (dígitos)
                                    - Timestamp válido
```

### Componentes do Sistema

#### Producers (Senders)
- **FruitSender**: Aplicação console para envio de frutas da época
  - Envio manual interativo
  - Envio de exemplos pré-configurados
  - Validação básica de entrada

- **UserSender**: Aplicação console para cadastro de usuários
  - Cadastro manual interativo
  - Envio de exemplos pré-configurados
  - Validação básica de entrada

#### Consumer/Producer (ValidationService)
- **ValidationService**: Serviço central de validação
  - Consome de 2 filas (frutas e usuários)
  - Valida campos obrigatórios
  - **Valida CPF com algoritmo oficial** (dígitos verificadores)
  - Republica mensagens válidas
  - Rejeita mensagens inválidas com logs

#### Consumers (Receivers)
- **FruitReceiver**: Receptor de frutas validadas
  - Consome frutas aprovadas
  - Exibe dados formatados
  - Logs de recebimento

- **UserReceiver**: Receptor de usuários validados
  - Consome usuários aprovados
  - Exibe dados formatados
  - Logs de recebimento

#### Shared Library
- **Shared**: Biblioteca compartilhada
  - Models: FruitMessage, UserMessage
  - RabbitMQ: Config, Connection
  - Reutilização de código

### Detalhes Técnicos - RabbitMQ

#### Exchanges (Direct Type)
```
fruit_exchange ──┬─ fruit.sender ───► fruit_sender_to_validation
                 │
                 └─ fruit.validated ─► fruit_validation_to_receiver

user_exchange ───┬─ user.sender ───► user_sender_to_validation
                 │
                 └─ user.validated ──► user_validation_to_receiver
```

#### Fluxo de Mensagens Detalhado

**Frutas:**
1. FruitSender cria FruitMessage e serializa para JSON
2. Publica em fruit_exchange com routing key "fruit.sender"
3. RabbitMQ roteia para queue fruit_sender_to_validation
4. ValidationService consome, valida e republica com "fruit.validated"
5. RabbitMQ roteia para queue fruit_validation_to_receiver
6. FruitReceiver consome e exibe

**Usuários:**
1. UserSender cria UserMessage e serializa para JSON
2. Publica em user_exchange com routing key "user.sender"
3. RabbitMQ roteia para queue user_sender_to_validation
4. ValidationService consome, valida CPF e republica com "user.validated"
5. RabbitMQ roteia para queue user_validation_to_receiver
6. UserReceiver consome e exibe

---

## 📋 Pré-requisitos

- **.NET 10.0 SDK** ou superior
- **Docker Desktop** ou **Podman** (para RabbitMQ)
- **Visual Studio 2022** (recomendado) ou **VS Code** com extensão C#

---

## 🚀 Instalação e Execução

### 1. Clonar o Repositório
```bash
git clone <url-do-repositorio>
cd HortifrutiSystem
```

### 2. Iniciar RabbitMQ

**Com Docker:**
```bash
docker-compose up -d
```

**Com Podman:**
```bash
# Iniciar máquina Podman (se necessário)
podman machine start

# Executar RabbitMQ
podman run -d \
  --name hortifruti-rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=guest \
  -e RABBITMQ_DEFAULT_PASS=guest \
  docker.io/library/rabbitmq:3-management
```

**Verificar:**
- Management UI: http://localhost:15672
- Usuário: `guest`
- Senha: `guest`

### 3. Compilar a Solução
```bash
# Restaurar dependências
dotnet restore

# Compilar todos os projetos
dotnet build
```

### 4. Executar os Serviços

**IMPORTANTE:** Execute em terminais separados na ordem abaixo:

#### Terminal 1 - ValidationService
```bash
cd Services/ValidationService
dotnet run
```

#### Terminal 2 - FruitReceiver
```bash
cd Services/FruitReceiver
dotnet run
```

#### Terminal 3 - UserReceiver
```bash
cd Services/UserReceiver
dotnet run
```

#### Terminal 4 - FruitSender
```bash
cd Services/FruitSender
dotnet run
```

#### Terminal 5 - UserSender
```bash
cd Services/UserSender
dotnet run
```

---

## 🔧 Configuração RabbitMQ

### Exchanges
- `fruit_exchange` (direct) - Para mensagens de frutas
- `user_exchange` (direct) - Para mensagens de usuários

### Filas
- `fruit_sender_to_validation` - Frutas do sender para validação
- `fruit_validation_to_receiver` - Frutas validadas para receiver
- `user_sender_to_validation` - Usuários do sender para validação
- `user_validation_to_receiver` - Usuários validados para receiver

### Routing Keys
- `fruit.sender` - Roteamento de frutas para validação
- `fruit.validated` - Roteamento de frutas validadas
- `user.sender` - Roteamento de usuários para validação
- `user.validated` - Roteamento de usuários validados

---

## 📝 Estrutura do Projeto

```
HortifrutiSystem/
├── HortifrutiSystem.sln
├── docker-compose.yml
├── README.md
├── ARQUITETURA.md
├── PASSO_A_PASSO.md
├── TESTES.md
├── Services/
│   ├── FruitSender/
│   │   ├── FruitSender.csproj
│   │   └── Program.cs
│   ├── UserSender/
│   │   ├── UserSender.csproj
│   │   └── Program.cs
│   ├── ValidationService/
│   │   ├── ValidationService.csproj
│   │   └── Program.cs
│   ├── FruitReceiver/
│   │   ├── FruitReceiver.csproj
│   │   └── Program.cs
│   └── UserReceiver/
│       ├── UserReceiver.csproj
│       └── Program.cs
└── Shared/
    ├── Shared.csproj
    ├── Models/
    │   ├── FruitMessage.cs
    │   └── UserMessage.cs
    └── RabbitMQ/
        ├── RabbitMQConfig.cs
        └── RabbitMQConnection.cs
```

---

## 📊 Modelos de Dados

### FruitMessage
```csharp
public class FruitMessage
{
    public string Name { get; set; }           // Nome da fruta
    public string Description { get; set; }    // Descrição da fruta
    public DateTime Timestamp { get; set; }    // Data/hora do envio
}
```

**Validações:**
- Nome: obrigatório, não vazio
- Descrição: obrigatória, não vazia
- Timestamp: válido, não default

### UserMessage
```csharp
public class UserMessage
{
    public string FullName { get; set; }              // Nome completo
    public string Address { get; set; }               // Endereço
    public string RG { get; set; }                    // RG
    public string CPF { get; set; }                   // CPF
    public DateTime RegistrationTimestamp { get; set; } // Data/hora do registro
}
```

**Validações:**
- Nome Completo: obrigatório, não vazio
- Endereço: obrigatório, não vazio
- RG: obrigatório, não vazio
- CPF: obrigatório, não vazio, **formato válido com dígitos verificadores**
- Timestamp: válido, não default

**Validação de CPF:**
- Remove caracteres não numéricos
- Verifica 11 dígitos
- Rejeita dígitos repetidos (111.111.111-11)
- Valida dígitos verificadores com algoritmo oficial

---

## 🧪 Testes

### Teste 1: Frutas Válidas
1. Execute o FruitSender (Terminal 4)
2. Escolha opção `2` (Enviar frutas de exemplo)
3. Observe no ValidationService (Terminal 1) as validações
4. Verifique no FruitReceiver (Terminal 2) as frutas recebidas

**Resultado esperado:** 3 frutas válidas processadas e exibidas

### Teste 2: Frutas Inválidas
1. No FruitSender, escolha opção `1` (manual)
2. Deixe o nome vazio
3. Insira uma descrição qualquer

**Resultado esperado:** Mensagem rejeitada no ValidationService

### Teste 3: Usuários Válidos
1. Execute o UserSender (Terminal 5)
2. Escolha opção `2` (Enviar usuários de exemplo)
3. Observe no ValidationService (Terminal 1) as validações de CPF
4. Verifique no UserReceiver (Terminal 3) os usuários recebidos

**Resultado esperado:** 4 usuários válidos processados e exibidos

### Teste 4: CPF Inválido
1. No UserSender, escolha opção `1` (manual)
2. Insira dados válidos mas CPF: `111.111.111-11`

**Resultado esperado:** Mensagem rejeitada por CPF inválido

### Teste 5: Monitoramento RabbitMQ
1. Acesse http://localhost:15672
2. Login: guest/guest
3. Vá em "Queues"
4. Observe as 4 filas e suas mensagens

**Resultado esperado:** Filas criadas, mensagens fluindo

---

## 🔍 Monitoramento

### RabbitMQ Management UI
- **URL:** http://localhost:15672
- **Usuário:** guest
- **Senha:** guest

**Funcionalidades:**
- Visualizar filas, exchanges e mensagens
- Monitorar consumo e produção
- Ver estatísticas em tempo real
- Gerenciar conexões

### Logs dos Serviços
Cada serviço imprime logs detalhados no console:
- `[SETUP]` - Configuração inicial
- `[ENVIADO]` - Mensagens enviadas
- `[VALIDATION]` - Processamento de validação
- `[RECEIVER]` - Mensagens recebidas
- `[ERRO]` - Falhas e exceções

---

## 🛠️ Desenvolvimento

### Adicionar Novos Campos
1. Modifique os modelos em `Shared/Models/`
2. Atualize a validação no `ValidationService`
3. Recompile todos os projetos: `dotnet build`

### Debugging
- Use breakpoints no Visual Studio
- Verifique logs no console
- Monitore filas no RabbitMQ UI
- Use `dotnet watch run` para hot reload

---

## 📚 Tecnologias Utilizadas

- **C# .NET 10.0** - Linguagem e framework principal
- **RabbitMQ 3** - Message broker para mensageria assíncrona
- **RabbitMQ.Client 6.8.1** - Cliente .NET para RabbitMQ
- **Newtonsoft.Json 13.0.3** - Serialização JSON
- **Docker/Podman** - Containerização do RabbitMQ

---

## 📖 Documentação Adicional

- **ARQUITETURA.md** - Diagramas detalhados e fluxos
- **PASSO_A_PASSO.md** - Guia completo de execução
- **TESTES.md** - Cenários de teste detalhados
- **VIDEO_ROTEIRO.md** - Roteiro para vídeo de apresentação
- **CORRECOES_REALIZADAS.md** - Histórico de correções
- **GUIA_PODMAN.md** - Comandos específicos do Podman

---

## ✨ Funcionalidades Principais

- ✅ Arquitetura de microservices
- ✅ Mensageria assíncrona com RabbitMQ
- ✅ Dois fluxos independentes (Frutas e Usuários)
- ✅ Validação robusta de dados
- ✅ **Validação completa de CPF** (dígitos verificadores)
- ✅ Tratamento de erros
- ✅ Logs detalhados
- ✅ Interface console interativa
- ✅ Exemplos pré-configurados
- ✅ Documentação completa

---

## 📄 Licença

Este projeto é para fins educacionais - CheckPoint 5 ABD TDS.

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs dos serviços
2. Confirme se o RabbitMQ está rodando: `podman ps` ou `docker ps`
3. Acesse o Management UI: http://localhost:15672
4. Consulte a documentação em PASSO_A_PASSO.md

---

**Desenvolvido com ❤️ para o CheckPoint 5**