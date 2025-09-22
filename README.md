# Projeto de Gestão da Mottu

📌 Nota: Projeto desenvolvido para fins acadêmicos na disciplina de Advanced Business Development with .NET

Este é um sistema desenvolvido em ASP.NET Core Minimal API para o cadastro e gerenciamento de motos, funcionários e filiais da Mottu.

A aplicação permite realizar CRUD completo (criação, listagem, edição e exclusão) para todas as entidades do domínio, garantindo controle centralizado e eficiente das informações.

Além disso, conta com recursos de versionamento de API, suporte a idempotência, HealthChecks e Rate Limiting, proporcionando maior confiabilidade, estabilidade e padronização para o consumo dos serviços.

## Integrantes

RM555679 - Lavinia Soo Hyun Park

## Funcionalidades

- Cadastro de filiais, motos e funcionários
- Integração com o banco Oracle utilizando Entity Framework Core
- Documentação da API interativa gerada com Scalar (OpenAPI)
- Versionamento de API para suportar evolução e compatibilidade entre versões
- Idempotência na requisição de Cadastro para evitar operações duplicadas acidentais
- HealthChecks e monitoramento do banco Oracle para verificar disponibilidade
- Rate Limiting para controle de consumo e proteção contra sobrecarga
- Relacionamentos entre entidades (ex.: motos vinculadas a uma filial, funcionário associado a filial)

## Como Rodar o Projeto

**IMPORTANTE**

Clone o repositório e dê `git clone [link repositorio]` na pasta desejada para poder rodá-lo em sua IDE

### 1. Requisitos

- .NET SDK 9.0 instalado (https://dotnet.microsoft.com/en-us/download)
- Oracle XE local ou acesso ao banco da sua instituição
- IDE: Visual Studio ou Rider (ou VS Code)
- API Client: Insomnia (utilizado nesse projeto), Postman ou outro de sua preferência

### 2. Configuração da conexão com o Banco de Dados

No arquivo `appsettings.json`, configure sua conexão Oracle:

```
"ConnectionStrings": {
    "DefaultConnection": "Data Source=[ORACLE-URL]:1521/[ORACLE-HOST];User Id=[ORACLE-USER];Password=[ORACLE-PASSWORD]"
  }
```

**Os campos que precisam ser preenchidos estão definidos por colchetes**

### 3. Executando o Projeto (Utilizando o terminal)

1. Restaure os pacotes

```
dotnet restore
```

2. Rode as migrations (ele irá criar automaticamente as tabelas no banco de dados)

```
dotnet ef database update
```

3. Inicie o servidor

```
dotnet run
```

4. Copie o link da URL gerado pelo .NET (ex.: http://localhost:5011)

## Efetuar testes no programa
