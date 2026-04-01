# Aerarium

Gerenciador financeiro pessoal desenvolvido pelo time Animus.

## Tech Stack

### Backend
- .NET 10, ASP.NET Core Minimal APIs
- Entity Framework Core 10 com PostgreSQL
- ASP.NET Identity para autenticação (JWT Bearer)
- [Mediator](https://github.com/martinothamar/Mediator) (source-generated) para CQRS
- FluentValidation para validação de requests
- Scalar para documentação OpenAPI
- xUnit + FluentAssertions para testes

### Frontend
- Angular 21 (standalone components, signals, zoneless)
- TypeScript strict mode
- SCSS para estilos
- Vitest como test runner

## Estrutura do Projeto

```
src/
├── Api/              # Endpoints, middleware, configuração de DI
├── Application/      # Commands, queries, handlers, validators
├── Domain/           # Entidades, value objects, enums
├── Infrastructure/   # EF Core, serviços externos
└── Frontend/         # Aplicação Angular
    └── src/
        └── app/
            ├── core/         # Services, guards, interceptors
            ├── features/     # Feature modules (lazy loaded)
            ├── models/       # Interfaces e tipos (DTOs)
            └── shared/       # Componentes reutilizáveis

tests/
├── UnitTests/        # Testes de domínio e application
└── IntegrationTests/ # Testes de API e banco de dados
```

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 24+](https://nodejs.org/) e npm
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
make migrate
```

### 3. Instalar dependências do frontend

```bash
make fe-install
```

### 4. Rodar a aplicação

Em terminais separados:

```bash
# Terminal 1 — API (.NET)
make run

# Terminal 2 — Frontend (Angular)
make fe
```

- Frontend: `http://localhost:4200`
- API: `http://localhost:5281`
- Scalar UI: `http://localhost:5281/scalar/v1`

## Comandos Úteis

| Comando | Descrição |
|---------|-----------|
| `make build` | Compilar o backend |
| `make run` | Rodar a API |
| `make test` | Executar todos os testes |
| `make migration name=Nome` | Criar migration |
| `make migrate` | Aplicar migrations |
| `make fmt` | Formatar código |
| `make fe-install` | Instalar dependências do frontend |
| `make fe` | Rodar o dev server Angular |
| `make fe-build` | Build de produção do frontend |

## Git Workflow

- Branches: `feature/`, `bugfix/`, `hotfix/`
- Commits: `type: description` (feat, fix, refactor, test, docs)

## Licença

Projeto privado — Animus.
