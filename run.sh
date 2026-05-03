#!/bin/bash

# Script de execução do Sistema Hortifruti
# Facilita iniciar todos os serviços em terminais separados

echo "╔════════════════════════════════════════════════════════════╗"
echo "║              SISTEMA DE GESTÃO HORTIFRUTI                 ║"
echo "║                 Script de Execução                        ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""

# Verificar se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "[ERRO] .NET SDK não encontrado. Instale o .NET 8.0 SDK primeiro."
    exit 1
fi

# Verificar se Docker está rodando
if ! docker info &> /dev/null; then
    echo "[ERRO] Docker não está rodando. Inicie o Docker Desktop primeiro."
    exit 1
fi

echo "[SETUP] Verificando RabbitMQ..."
if ! docker ps | grep -q hortifruti-rabbitmq; then
    echo "[SETUP] Iniciando RabbitMQ..."
    docker-compose up -d
    echo "[SETUP] Aguardando RabbitMQ iniciar..."
    sleep 10
else
    echo "[SETUP] RabbitMQ já está rodando."
fi

echo "[SETUP] Compilando projetos..."
dotnet build

if [ $? -ne 0 ]; then
    echo "[ERRO] Falha na compilação. Verifique os erros acima."
    exit 1
fi

echo ""
echo "════════════════════════════════════════════════════════════"
echo "INSTRUÇÕES DE EXECUÇÃO:"
echo "════════════════════════════════════════════════════════════"
echo ""
echo "Abra 5 terminais separados e execute os comandos abaixo:"
echo ""
echo "Terminal 1 - ValidationService:"
echo "  cd Services/ValidationService && dotnet run"
echo ""
echo "Terminal 2 - FruitReceiver:"
echo "  cd Services/FruitReceiver && dotnet run"
echo ""
echo "Terminal 3 - UserReceiver:"
echo "  cd Services/UserReceiver && dotnet run"
echo ""
echo "Terminal 4 - FruitSender:"
echo "  cd Services/FruitSender && dotnet run"
echo ""
echo "Terminal 5 - UserSender:"
echo "  cd Services/UserSender && dotnet run"
echo ""
echo "════════════════════════════════════════════════════════════"
echo ""
echo "Após iniciar todos os serviços:"
echo "1. No FruitSender, pressione 2 para enviar frutas de exemplo"
echo "2. No UserSender, pressione 2 para enviar usuários de exemplo"
echo "3. Observe as mensagens sendo processadas nos receivers"
echo ""
echo "Para parar: Pressione Ctrl+C em cada terminal"
echo "Para parar RabbitMQ: docker-compose down"
echo ""
echo "RabbitMQ Management UI: http://localhost:15672 (guest/guest)"
echo ""