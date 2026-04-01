# Aerarium

Gerenciador financeiro pessoal desenvolvido pelo time Animus.

## Tech Stack

- .NET 10, ASP.NET Core Minimal APIs
- Entity Framework Core 10 com PostgreSQL
- ASP.NET Identity para autenticação (JWT Bearer)
- [Mediator](https://github.com/martinothamar/Mediator) (source-generated) para CQRS
- FluentValidation para validação de requests
- Scalar para documentação OpenAPI
- xUnit + FluentAssertions para testes

## Estrutura do Projeto

```
src/
├── Api/              # Endpoints, middleware, configuração de DI
├── Application/      # Commands, queries, handlers, validators
├── Domain/           # Entidades, value objects, enums
└── Infrastructure/   # EF Core, serviços externos

tests/
├── UnitTests/        # Testes de domínio e application
└── IntegrationTests/ # Testes de API e banco de dados
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (para o PostgreSQL)

## Primeiros Passos

### 1. Subir o banco de dados

```bash
docker compose up -d
```

Isso inicia um container PostgreSQL na porta `5432` com as credenciais:

| Campo    | Valor           |
|----------|-----------------|
| Host     | localhost       |
| Porta    | 5432            |
| Database | aerarium        |
| Usuário  | aerarium        |
| Senha    | aerarium_dev    |

### 2. Aplicar migrations

```bash
dotnet ef database update -p src/Infrastructure -s src/Api
```

### 3. Rodar a API

```bash
dotnet run --project src/Api
```

A documentação da API estará disponível em:
- OpenAPI: `http://localhost:5281/openapi/v1.json`
- Scalar UI: `http://localhost:5281/scalar/v1`

## Comandos Úteis

| Comando | Descrição |
|---------|-----------|
| `dotnet build` | Compilar a solução |
| `dotnet test` | Executar todos os testes |
| `dotnet run --project src/Api` | Rodar a API |
| `dotnet ef migrations add <Nome> -p src/Infrastructure -s src/Api` | Criar migration |
| `dotnet ef database update -p src/Infrastructure -s src/Api` | Aplicar migrations |
| `dotnet format` | Formatar código |

## Git Workflow

- Branches: `feature/`, `bugfix/`, `hotfix/`
- Commits: `type: description` (feat, fix, refactor, test, docs)

## Licença

Projeto privado — Animus.
