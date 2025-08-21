# Rentals Service

Plataforma para cadastro de motos e entregadores, locações com regras de negócio (planos, multas e extras), upload de CNH em storage externo e mensageria para eventos de domínio. Implementado com **Clean Architecture + DDD + SOLID**, **EF Core (PostgreSQL)**, **MassTransit + RabbitMQ (EF Outbox)**, **MinIO** e **MediatR**.

---

## Sumário
- [Stack](#stack)
- [Arquitetura](#arquitetura)
- [Como rodar](#como-rodar)
  - [Com Docker Compose (recomendado)](#com-docker-compose-recomendado)
- [Configuração (appsettings & env)](#configuração-appsettings--env)
- [Banco de dados & Migrations](#banco-de-dados--migrations)
- [Mensageria](#mensageria)
- [Storage de CNH](#storage-de-cnh)
- [Logging & Healthchecks](#logging--healthchecks)
- [Endpoints principais](#endpoints-principais)
- [Testes](#testes)
- [Estrutura de pastas](#estrutura-de-pastas)
- [Troubleshooting](#troubleshooting)

---

## Stack
- **.NET 9 / ASP.NET Core** (Web API)
- **Clean Architecture + DDD + SOLID**
- **MediatR** (CQRS leve: Commands/Queries/Handlers)
- **EF Core 8 (Npgsql)** + Repositories + Unit of Work
- **RabbitMQ + MassTransit** com **EF Outbox** (consistência transacional)
- **MinIO** (S3 compatível) para armazenar **imagem da CNH**
- **Swagger** (documentação da API)
- **Serilog** (logs estruturados) + **HealthChecks**

---

## Arquitetura
**Camadas**:
```
/src
  Rentals.Api              → Interface REST (controllers, DTOs PT-BR, Swagger, health)
  Rentals.Application      → Casos de uso (CQRS), validações, portas (abstrações)
  Rentals.Domain           → Entidades, Value Objects, regras, serviços de domínio
  Rentals.Infrastructure   → EF Core/Npgsql, repositórios, UoW, MassTransit, MinIO
  tests/                   → Testes unitários
```

**Fluxo de negócio (exemplo)** — cadastro de moto → evento:
1. `POST /motos` cria a moto (EF Core).
2. Handler publica **evento** `MotorcycleRegistered` (via **IEventBus** adaptado com MassTransit).
3. **EF Outbox** garante publicação após `SaveChanges()` (idempotência).
4. **Consumer** persiste uma **notificação** quando `ano == 2024` (consulta futura).

**Regras-chave**
- **Locação**: planos (7/15/30/45/50 dias) com diárias e penalidades:
  - 7d → R$30/dia, multa **20%** por diária **não efetivada** (devolução antecipada).
  - 15d → R$28/dia, multa **40%** por diária **não efetivada**.
  - 30d → R$22/dia; 45d → R$20/dia; 50d → R$18/dia.
  - Devolução **após** a previsão: **R$50/dia extra**.
  - Início sempre **no dia seguinte** à criação.
  - Apenas entregadores com **categoria A** podem alugar.
- **CNH**: upload **png/bmp** para **MinIO** (não armazena binário no banco).
- **Moto**: placa única; **remoção proibida** se houver locações (ativas ou históricas).

---

## Como rodar

### Com Docker Compose 
Pré-requisitos: Docker + Docker Compose.

```bash
# na raiz do repositório
docker compose up -d --build
# aplicar as migrations
dotnet ef database update --project src/Rentals.Infrastructure --startup-project src/Rentals.Api
```
IMPORTANTE: Nao esqueca de alterar os arquivos docker-compose.yml e appsettings.json para o seu respectivo USERNAME E PASSWORD, para acessar o banco. 

Serviços expostos (padrão):
- API: http://localhost:5089/swagger
- Postgres: localhost:5432 (db: `rentalsdb`, user: `postgres`, pwd: `452313`)
- RabbitMQ: http://localhost:15672 (guest/guest)
- MinIO: http://localhost:9003 (console) e http://localhost:9002 (S3)


## Configuração (appsettings & env)
O projeto lê a connection string em ordem de precedência:
1. `ConnectionStrings__Default` (env/user-secrets/appsettings)
2. Variáveis padrão PG (`PGHOST`, `PGPORT`, `PGDATABASE`, `PGUSER`, `PGPASSWORD`)
3. Fallback (localhost/app/app)

**Arquivo de exemplo** (`src/Rentals.Api/appsettings.Development.json.example`):
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=rentalsdb;Username=app;Password=app"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Minio": {
    "Endpoint": "http://localhost:9002",
    "AccessKey": "minio",
    "SecretKey": "minio123",
    "Bucket": "cnh-images"
  }
}
```
Copie para `appsettings.Development.json` e ajuste se necessário.


IMPORTANTE: Nao esqueca de alterar os arquivos docker-compose.yml e appsettings.json para o seu respectivo USERNAME E PASSWORD, para acessar o banco. 

---

## Banco de dados & Migrations
- As migrations ficam em `src/Rentals.Infrastructure/Persistence/Migrations`.
- Em **Development**, o startup da API executa **migrate se houver pendentes**.
- Manualmente, você pode rodar:
  ```bash
  dotnet ef database update -p src/Rentals.Infrastructure -s src/Rentals.Api
  ```

---

## Mensageria
- **Evento**: `MotorcycleRegistered` publicado no cadastro de moto.
- **Bus**: MassTransit + RabbitMQ com **EF Outbox** (entrega após commit da transação).
- **Fila do consumer**: `motorcycle-registered-notifications` (definição explícita).
- **Consumer**: salva **notificação** quando `Year == 2024`.
- **Consulta administrativa** (opcional): `GET /notificacoes/motos?ano=2024`.

---

## Storage de CNH
- Upload **multipart/form-data** em endpoint de entregador.
- Aceita **png** ou **bmp**.
- Armazena no **MinIO** (bucket configurável) e guarda apenas a **URL** no banco.

---

## Logging & Healthchecks
- **Serilog** com enriquecedores (ambiente, máquina, processo, thread) e request logging.
- **Healthchecks**:
  - `/health/live` → liveness
  - `/health/ready` → Postgres, RabbitMQ, MinIO, MassTransit bus

Exemplo de resposta `/health/ready`:
```json
{
  "status":"Healthy",
  "entries":{
    "postgres":{"status":"Healthy"},
    "rabbitmq":{"status":"Healthy"},
    "minio":{"status":"Healthy"},
    "masstransit-bus":{"status":"Healthy"}
  }
}
```

---

## Endpoints principais
> **Nomes dos campos e rotas seguem o Swagger de referência (PT-BR)**.

### Motos
- **POST `/motos`** — cadastra moto
  ```json
  {
    "identificador": "moto123",
    "modelo": "CG 160",
    "ano": 2024,
    "placa": "ABC1D23"
  }
  ```
- **GET `/motos?placa=ABC1D23&page=1&pageSize=20`** — lista com filtro por placa
- **GET `/motos/{identificador}`** — busca por identificador
- **GET `/motos/placa/{placa}`** — busca por placa
- **PUT `/motos/{identificador}/placa`** — altera placa
- **DELETE `/motos/{identificador}`** — remove moto (**bloqueado** se houver locações)

### Entregadores
- **POST `/entregadores`** — cadastra entregador (CNPJ e CNH únicos)
- **PUT `/entregadores/{identificador}/cnh`** — upload de imagem da CNH (**png/bmp**)

### Locações
- **POST `/locacoes`** — cria locação
  ```json
  {
    "entregador_id": "entregador123",
    "moto_id": "moto123",
    "data_inicio": "2024-01-01T00:00:00Z",
    "data_termino": "2024-01-07T23:59:59Z",
    "data_previsao_termino": "2024-01-07T23:59:59Z",
    "plano": 7
  }
  ```
- **GET `/locacoes/{id}`** — detalhe da locação
- **PUT `/locacoes/{id}/devolucao`** — informa devolução
  ```json
  {
    "data_devolucao": "2024-01-07T18:00:00Z"
  }
  ```

### Erros (exemplo)
- `400`:
  ```json
  { "mensagem": "Dados inválidos" }
  ```
- `404`:
  ```json
  { "mensagem": "Moto não encontrada" }
  ```

---

## Testes
### Unitários
- VO: **Plate**, **Cnpj**, **CnhNumber**
- Serviço: **PricingService** (diárias, multa, extra)
- Entidade: **Rental** (datas, devolução antecipada/tardia)
- Handlers: `CreateRentalHandler` (categoria A, moto já locada), `CreateMotorcycleHandler` (publicação de evento)

Rodar:
```bash
dotnet test tests/Rentals.UnitTests -v n
```

### Integração (opcional)
- Testcontainers (Postgres, RabbitMQ, MinIO) + WebApplicationFactory.
- Ex.: criar moto (ano 2024) → verificar notificação persistida pelo consumer.

---

## Estrutura de pastas
```
src/
  Rentals.Api/
    Controllers/         # Controllers 
    Contracts/           # DTOs HTTP (PT-BR)
    Swagger/             # exemplos e configuração
  Rentals.Application/
    Abstractions/        # portas (ex.: IEventBus, repos, UoW)
    Behaviors/           # pipeline MediatR (ex.: Validation/Logging)
    Motorcycles/         # Commands/Queries/Handlers/Validators
    Rentals/             # idem para locações
    Services/            # IPricingService
  Rentals.Domain/
    Entities/ ValueObjects/ Enums/ Services/
  Rentals.Infrastructure/
    Persistence/         # DbContext, configs, repositórios, migrations
    Messaging/           # MassTransit (event bus, consumers, definitions)
    Storage/             # MinIO/Local storage
```

---

## Troubleshooting
- **PendingModelChangesWarning** no startup:
  - Geração de migration ficou para trás do modelo. Gere uma nova migration e commit.
  - Em dev, o startup aplica **só se houver pendentes**.
- **Erro de conexão com Postgres**:
  - Verifique `ConnectionStrings__Default` ou use variáveis PG (`PGHOST`, `PGUSER`, …).
- **Upload CNH falha**:
  - Confirme `Minio:Endpoint/AccessKey/SecretKey/Bucket`
- **Fila do consumer**:
  - RabbitMQ UI: http://localhost:15672 → `motorcycle-registered-notifications` deve aparecer **ready**.

---

## Licença
Uso educacional/demonstração.

