# Guia para Adicionar o Projeto ao GitHub

## 📦 Repositório de Destino
**URL:** https://github.com/Lucas-Borges27/CP5-DotNet.git

---

## 🚀 Passo a Passo

### Opção 1: Inicializar Git e Fazer Push (Recomendado)

```bash
# 1. Navegar até a pasta do projeto
cd /Users/mgregoriolaura/Downloads/cp5/HortifrutiSystem

# 2. Inicializar repositório Git (se ainda não foi feito)
git init

# 3. Adicionar o repositório remoto
git remote add origin https://github.com/Lucas-Borges27/CP5-DotNet.git

# 4. Verificar se o remote foi adicionado
git remote -v

# 5. Adicionar todos os arquivos ao staging
git add .

# 6. Fazer o primeiro commit
git commit -m "feat: Sistema Hortifruti completo com microservices e RabbitMQ

- Implementação de 5 microservices (.NET 10.0)
- Dois fluxos independentes (Frutas e Usuários)
- Validação robusta de CPF com dígitos verificadores
- Mensageria assíncrona com RabbitMQ
- Documentação completa
- Testes implementados
- Código limpo sem duplicações"

# 7. Fazer push para o repositório
git push -u origin main

# Se o branch for master ao invés de main:
# git push -u origin master
```

### Opção 2: Se o Repositório Já Existe Localmente

```bash
# 1. Navegar até a pasta do projeto
cd /Users/mgregoriolaura/Downloads/cp5/HortifrutiSystem

# 2. Verificar status
git status

# 3. Adicionar arquivos modificados
git add .

# 4. Fazer commit
git commit -m "feat: Atualização completa do sistema

- Correção de código duplicado
- Implementação de validação de CPF
- Atualização para .NET 10.0
- Documentação expandida com arquitetura"

# 5. Fazer push
git push origin main
```

### Opção 3: Clonar e Substituir (Se o Repo Já Tem Conteúdo)

```bash
# 1. Clonar o repositório em outra pasta
cd /Users/mgregoriolaura/Downloads/cp5
git clone https://github.com/Lucas-Borges27/CP5-DotNet.git CP5-DotNet-temp

# 2. Copiar arquivos do projeto atual para o clonado
cp -r HortifrutiSystem/* CP5-DotNet-temp/

# 3. Entrar na pasta clonada
cd CP5-DotNet-temp

# 4. Adicionar mudanças
git add .

# 5. Fazer commit
git commit -m "feat: Sistema Hortifruti completo"

# 6. Fazer push
git push origin main
```

---

## 📝 Arquivos a Serem Incluídos

### ✅ Arquivos Essenciais
```
HortifrutiSystem/
├── .gitignore                    ✅ Já existe
├── README.md                     ✅ Atualizado
├── ARQUITETURA.md                ✅ Documentação
├── PASSO_A_PASSO.md              ✅ Guia de execução
├── TESTES.md                     ✅ Cenários de teste
├── VIDEO_ROTEIRO.md              ✅ Roteiro para vídeo
├── CORRECOES_REALIZADAS.md       ✅ Histórico
├── GUIA_PODMAN.md                ✅ Comandos Podman
├── CHECKLIST_REQUISITOS.md       ✅ Conformidade
├── docker-compose.yml            ✅ RabbitMQ
├── HortifrutiSystem.sln          ✅ Solução
├── Services/                     ✅ Microservices
└── Shared/                       ✅ Biblioteca compartilhada
```

### ❌ Arquivos a Ignorar (já no .gitignore)
```
bin/
obj/
*.user
*.suo
.vs/
```

---

## 🔐 Autenticação GitHub

### Se Pedir Credenciais:

**Opção 1: Personal Access Token (Recomendado)**
1. Acesse: https://github.com/settings/tokens
2. Clique em "Generate new token (classic)"
3. Selecione scopes: `repo` (todos)
4. Copie o token gerado
5. Use como senha quando o Git pedir

**Opção 2: SSH Key**
```bash
# Gerar chave SSH (se não tiver)
ssh-keygen -t ed25519 -C "seu-email@example.com"

# Copiar chave pública
cat ~/.ssh/id_ed25519.pub

# Adicionar em: https://github.com/settings/keys

# Mudar remote para SSH
git remote set-url origin git@github.com:Lucas-Borges27/CP5-DotNet.git
```

---

## ✅ Verificação Pós-Push

Após fazer o push, verifique:

1. **Acesse o repositório:**
   https://github.com/Lucas-Borges27/CP5-DotNet

2. **Verifique se os arquivos estão lá:**
   - README.md com arquitetura
   - Todos os serviços (Services/)
   - Biblioteca Shared
   - Documentação completa

3. **Teste o clone:**
   ```bash
   cd /tmp
   git clone https://github.com/Lucas-Borges27/CP5-DotNet.git
   cd CP5-DotNet
   dotnet build
   ```

---

## 🐛 Troubleshooting

### Erro: "remote origin already exists"
```bash
git remote remove origin
git remote add origin https://github.com/Lucas-Borges27/CP5-DotNet.git
```

### Erro: "failed to push some refs"
```bash
# Fazer pull primeiro
git pull origin main --rebase

# Depois push
git push origin main
```

### Erro: "Permission denied"
- Verifique se você tem acesso ao repositório
- Use Personal Access Token como senha
- Ou configure SSH key

### Arquivos Grandes
Se houver erro com arquivos grandes:
```bash
# Verificar tamanho dos arquivos
find . -type f -size +50M

# Adicionar ao .gitignore se necessário
echo "*.dll" >> .gitignore
echo "*.pdb" >> .gitignore
```

---

## 📋 Checklist Final

Antes de fazer push, verifique:

- [ ] Código compilando: `dotnet build`
- [ ] .gitignore configurado
- [ ] README.md atualizado
- [ ] Documentação completa
- [ ] Sem arquivos binários desnecessários
- [ ] Sem senhas ou tokens no código
- [ ] Commit message descritivo

---

## 🎯 Comando Rápido (Tudo de Uma Vez)

```bash
cd /Users/mgregoriolaura/Downloads/cp5/HortifrutiSystem
git init
git remote add origin https://github.com/Lucas-Borges27/CP5-DotNet.git
git add .
git commit -m "feat: Sistema Hortifruti completo - CheckPoint 5"
git push -u origin main
```

---

## 📞 Suporte

Se tiver problemas:
1. Verifique se tem permissão no repositório
2. Confirme que o repositório existe
3. Use Personal Access Token para autenticação
4. Verifique o .gitignore

**Boa sorte com o push! 🚀**