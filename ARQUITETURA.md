# Diagrama Textual do Sistema Hortifruti

## Arquitetura Geral

```
┌─────────────────────────────────────────────────────────────────┐
│                    SISTEMA HORTIFRUTI                           │
│                  MICROSERVICES + RABBITMQ                       │
└─────────────────────────────────────────────────────────────────┘
                                    │
                    ┌───────────────┼───────────────┐
                    │               │               │
          ┌─────────▼─────────┐   ┌─▼─────────────┐│
          │   FLUXO FRUTAS    │   │ FLUXO USUÁRIOS ││
          └───────────────────┘   └───────────────┘│
                                                  │
┌─────────────────────────────────────────────────┼─────────────────────────────────────────────────┐
│                                                 │                                                 │
│  FruitSender ──(RabbitMQ)──► ValidationService ──(RabbitMQ)──► FruitReceiver                    │
│     │                        │        │                        │        │                        │
│     └─ Envia frutas          └─ Recebe│valida                 └─ Envia validadas                │
│                                       │                        │                                 │
│  UserSender ───(RabbitMQ)───► ValidationService ──(RabbitMQ)───► UserReceiver                   │
│     │                        │        │                        │        │                        │
│     └─ Envia usuários        └─ Recebe│valida                 └─ Envia validados                │
│                                       │                        │                                 │
└─────────────────────────────────────────────────────────────────────────────────────────────────┘
```

## Detalhes Técnicos - RabbitMQ

### Exchanges (Direct Type)
```
fruit_exchange ──┬─ fruit.sender ───► fruit_sender_to_validation
                 │
                 └─ fruit.validated ─► fruit_validation_to_receiver

user_exchange ───┬─ user.sender ───► user_sender_to_validation
                 │
                 └─ user.validated ──► user_validation_to_receiver
```

### Fluxo de Mensagens - Frutas

```
1. FruitSender
   ├── Cria FruitMessage { Name, Description, Timestamp }
   ├── Serializa para JSON
   └── Publica em fruit_exchange com routing key "fruit.sender"

2. RabbitMQ
   ├── Recebe mensagem na exchange fruit_exchange
   ├── Roteia para queue fruit_sender_to_validation
   └── Entrega para ValidationService

3. ValidationService
   ├── Consome da queue fruit_sender_to_validation
   ├── Desserializa FruitMessage
   ├── Valida campos obrigatórios
   ├── Se válido: republica com routing key "fruit.validated"
   └── Se inválido: loga erro

4. RabbitMQ
   ├── Recebe mensagem validada
   ├── Roteia para queue fruit_validation_to_receiver
   └── Entrega para FruitReceiver

5. FruitReceiver
   ├── Consome da queue fruit_validation_to_receiver
   ├── Desserializa FruitMessage
   └── Exibe dados da fruta
```

### Fluxo de Mensagens - Usuários

```
1. UserSender
   ├── Cria UserMessage { FullName, Address, RG, CPF, RegistrationTimestamp }
   ├── Serializa para JSON
   └── Publica em user_exchange com routing key "user.sender"

2. RabbitMQ
   ├── Recebe mensagem na exchange user_exchange
   ├── Roteia para queue user_sender_to_validation
   └── Entrega para ValidationService

3. ValidationService
   ├── Consome da queue user_sender_to_validation
   ├── Desserializa UserMessage
   ├── Valida campos obrigatórios
   ├── Se válido: republica com routing key "user.validated"
   └── Se inválido: loga erro

4. RabbitMQ
   ├── Recebe mensagem validada
   ├── Roteia para queue user_validation_to_receiver
   └── Entrega para UserReceiver

5. UserReceiver
   ├── Consome da queue user_validation_to_receiver
   ├── Desserializa UserMessage
   └── Exibe dados do usuário
```

## Componentes do Sistema

### Producers (Senders)
- **FruitSender**: Aplicação console para envio de frutas
- **UserSender**: Aplicação console para cadastro de usuários
- Ambos publicam mensagens nas exchanges apropriadas

### Consumer/Producer (ValidationService)
- **ValidationService**: Serviço que consome de ambas as queues de entrada
- Valida dados obrigatórios
- Se válido, republica nas queues de saída
- Se inválido, loga erro (não reenvia)

### Consumers (Receivers)
- **FruitReceiver**: Consome frutas validadas e exibe
- **UserReceiver**: Consome usuários validados e exibe

### Message Broker
- **RabbitMQ**: Gerencia roteamento, filas e entrega de mensagens
- Exchanges do tipo "direct" para roteamento baseado em routing keys
- Queues duráveis para persistência
- Management UI para monitoramento

## Fluxo de Dados

```
Input ──► Sender ──► Exchange ──► Queue ──► Validation ──► Exchange ──► Queue ──► Receiver ──► Output
                (publish)     (route)    (deliver)    (validate)    (publish)    (route)    (consume)
```

## Benefícios da Arquitetura

- **Desacoplamento**: Serviços independentes
- **Escalabilidade**: Cada componente pode ser escalado separadamente
- **Resiliência**: Falha em um serviço não afeta os outros
- **Manutenibilidade**: Código organizado e responsabilidades claras
- **Monitoramento**: RabbitMQ UI permite acompanhar mensagens