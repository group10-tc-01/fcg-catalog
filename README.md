# 🎮 FCG.Catalog - API de Catálogo de Jogos e Biblioteca

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

## 📋 Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [Responsabilidade](#-responsabilidade)
- [Arquitetura](#-arquitetura)
- [Tecnologias e Bibliotecas](#-tecnologias-e-bibliotecas)
- [Modelo de Dados](#-modelo-de-dados)
- [Regras de Negócio](#-regras-de-negócio)
- [Endpoints da API](#-endpoints-da-api)
- [Eventos](#-eventos)
- [Configuração e Execução](#-configuração-e-execução)

---

## 🎯 Sobre o Projeto

**FCG.Catalog** é uma API RESTful desenvolvida em .NET 8 para gerenciamento completo de catálogo de jogos, promoções e biblioteca digital de usuários. A aplicação implementa processamento assíncrono de compras via **Event-Driven Architecture** com **Apache Kafka**, garantindo consistência entre catálogo, pagamentos e biblioteca pessoal.

### 🚀 Responsabilidade

A API é responsável por:

- 🎮 **Gerenciamento completo do catálogo de jogos**
- 🏷️ **Sistema de promoções com cálculo automático de descontos**
- 📚 **Biblioteca pessoal de jogos por usuário**
- 🛒 **Fluxo de compra integrado com sistema de pagamentos**
- 🔄 **Consumo e publicação de eventos de domínio**
- 🔍 **Busca e filtros avançados no catálogo**
- ⚡ **Processamento assíncrono de aquisições via Kafka**
- 🛡️ **Validação de duplicatas e integridade de biblioteca**

---

## 🏛️ Arquitetura

A aplicação segue os princípios da **Clean Architecture**, garantindo separação de responsabilidades, testabilidade e manutenibilidade do código.

### Estrutura de Camadas

```
┌─────────────────────────────────────────┐
│       FCG.Catalog.WebApi                │  ← Camada de Apresentação (API REST)
│   Controllers, Middlewares, Filters    │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│     FCG.Catalog.Application             │  ← Camada de Aplicação (Use Cases)
│   UseCases, Validations, DTOs          │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│       FCG.Catalog.Domain                │  ← Camada de Domínio (Regras de Negócio)
│   Entities, Exceptions, Events         │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│    FCG.Catalog.Infrastructure.*         │  ← Camada de Infraestrutura
│  SqlServer, Kafka, Auth (JWT)   │
└─────────────────────────────────────────┘
```

### Camadas do Projeto

#### 1️⃣ **Domain** (`FCG.Catalog.Domain`)
- Entidades de negócio: `Game`, `Promotion`, `Library`, `LibraryGame`
- Exceções de domínio: `DomainException`, `NotFoundException`, `ConflictException`
- Enums: `GameCategory`, `PromotionStatus`
- Value Objects: `Price`, `Discount`
- Abstrações: `BaseEntity`, `IUnitOfWork`

#### 2️⃣ **Application** (`FCG.Catalog.Application`)
- **Use Cases** (CQRS): Commands e Queries
  - Games: Criar, atualizar, deletar, listar jogos
  - Promotions: Criar, atualizar, deletar promoções
  - Library: Iniciar compra, listar biblioteca
- **Validações** com FluentValidation
- **Abstrações**: Repositories, Messaging, Pagination
- **Behaviors**: Validação, Logging, Transaction

#### 3️⃣ **Infrastructure**
- **SqlServer** (`FCG.Catalog.Infrastructure.SqlServer`): Persistência com Entity Framework Core
- **Auth** (`FCG.Catalog.Infrastructure.Auth`): Implementação JWT
- **Kafka** (`FCG.Catalog.Infrastructure.Kafka`): Produção e consumo de eventos
  - Consumers: `UserCreatedEventConsumer`, `PaymentProcessedEventConsumer`
  - Producers: `OrderPlacedEventProducer`

#### 4️⃣ **Presentation** (`FCG.Catalog.WebApi`)
- Controllers versionados (`/v1/...`)
- Middlewares customizados (Exception Handler, Correlation ID)
- Health Checks
- Swagger/OpenAPI

---

## 🛠️ Tecnologias e Bibliotecas

### Core Framework
- **.NET 8** - Framework principal
- **C# 12** - Linguagem de programação

### Comunicação Assíncrona
- **Apache Kafka** (`Confluent.Kafka 2.6.1`) - Mensageria para Event-Driven Architecture
- **MediatR** (`13.1.0`) - Mediator pattern para CQRS

### Persistência
- **Entity Framework Core 9.0** - ORM
- **SQL Server 2022** - Banco de dados relacional
- **Migrations** - Controle de versionamento do schema

### Segurança
- **JWT Bearer Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer 8.0.22`)
- **Authorization Policies** - Controle de acesso baseado em roles

### Validação e Qualidade
- **FluentValidation** (`12.1.0`) - Validação de objetos
- **Serilog** (`4.3.0`) - Logging estruturado
- **Seq** - Centralização de logs

### API e Documentação
- **Swagger/OpenAPI** (`Swashbuckle.AspNetCore 6.6.2`)
- **API Versioning** (`Asp.Versioning.Http 8.1.0`)

### Observabilidade
- **Health Checks** - Monitoramento de saúde da aplicação
- **Correlation ID** - Rastreamento de requisições

### Testes
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions fluentes
- **Testcontainers** - Testes de integração

---

## 💾 Modelo de Dados

### Tabela `Games`

```sql
CREATE TABLE Games (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(2000) NULL,
    Price DECIMAL(18,2) NOT NULL,
    Category NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,

    CONSTRAINT CK_Game_Price CHECK (Price > 0),
    INDEX IX_Games_Category (Category),
    INDEX IX_Games_IsActive (IsActive)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único do jogo (GUID) |
| `Title` | NVARCHAR(200) | Título do jogo |
| `Description` | NVARCHAR(2000) | Descrição detalhada do jogo |
| `Price` | DECIMAL(18,2) | Preço base do jogo (deve ser > 0) |
| `Category` | NVARCHAR(50) | Categoria do jogo |
| `IsActive` | BIT | Indica se jogo está ativo para venda |
| `CreatedAt` | DATETIME2 | Data/hora de criação |
| `UpdatedAt` | DATETIME2 | Data/hora da última atualização |

**Categorias permitidas:** `Action`, `Adventure`, `RPG`, `Strategy`, `Sports`, `Racing`, `Simulation`, `Other`

### Tabela `Promotions`

```sql
CREATE TABLE Promotions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GameId UNIQUEIDENTIFIER NOT NULL,
    DiscountPercentage DECIMAL(5,2) NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_Promotions_Games FOREIGN KEY (GameId) 
        REFERENCES Games(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Promotion_Discount CHECK (DiscountPercentage > 0 AND DiscountPercentage <= 100),
    CONSTRAINT CK_Promotion_Dates CHECK (EndDate > StartDate),
    INDEX IX_Promotions_GameId (GameId),
    INDEX IX_Promotions_Dates (StartDate, EndDate)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único da promoção |
| `GameId` | UNIQUEIDENTIFIER | Referência ao jogo em promoção |
| `DiscountPercentage` | DECIMAL(5,2) | Percentual de desconto (1-100%) |
| `StartDate` | DATETIME2 | Data/hora de início da promoção |
| `EndDate` | DATETIME2 | Data/hora de fim da promoção |
| `IsActive` | BIT | Flag para ativar/desativar promoção manualmente |
| `CreatedAt` | DATETIME2 | Data/hora de criação |

### Tabela `Libraries`

```sql
CREATE TABLE Libraries (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL UNIQUE,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    INDEX IX_Libraries_UserId (UserId)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único da biblioteca |
| `UserId` | UNIQUEIDENTIFIER | Identificador do usuário (sem FK - cross-database) |
| `CreatedAt` | DATETIME2 | Data/hora de criação |

⚠️ **Importante:** `UserId` não possui Foreign Key pois `Users` está em outro banco de dados (microserviço separado).

### Tabela `LibraryGames`

```sql
CREATE TABLE LibraryGames (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    LibraryId UNIQUEIDENTIFIER NOT NULL,
    GameId UNIQUEIDENTIFIER NOT NULL,
    PurchasePrice DECIMAL(18,2) NOT NULL,
    PurchasedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT FK_LibraryGames_Libraries FOREIGN KEY (LibraryId) 
        REFERENCES Libraries(Id) ON DELETE CASCADE,
    CONSTRAINT FK_LibraryGames_Games FOREIGN KEY (GameId) 
        REFERENCES Games(Id),
    CONSTRAINT UQ_LibraryGames_LibraryId_GameId UNIQUE (LibraryId, GameId),
    CONSTRAINT CK_LibraryGame_Price CHECK (PurchasePrice >= 0),
    INDEX IX_LibraryGames_LibraryId (LibraryId),
    INDEX IX_LibraryGames_GameId (GameId)
);
```

**Campos:**
| Campo | Tipo | Descrição |
|-------|------|-----------|
| `Id` | UNIQUEIDENTIFIER | Identificador único do registro |
| `LibraryId` | UNIQUEIDENTIFIER | Referência à biblioteca do usuário |
| `GameId` | UNIQUEIDENTIFIER | Referência ao jogo adquirido |
| `PurchasePrice` | DECIMAL(18,2) | Preço pago pelo jogo (com desconto se aplicável) |
| `PurchasedAt` | DATETIME2 | Data/hora da aquisição |

⚠️ **Importante:** Constraint `UNIQUE (LibraryId, GameId)` garante que um usuário não pode ter o mesmo jogo duplicado na biblioteca.

---

## 📐 Regras de Negócio

### RN-CAT-001: Cadastro de Jogo (Admin)
- ✅ **Apenas Admin** pode criar, atualizar ou deletar jogos
- ✅ Título deve ter entre **3 e 200 caracteres**
- ✅ Preço deve ser **maior que zero**
- ✅ **Categorias permitidas**: `Action`, `Adventure`, `RPG`, `Strategy`, `Sports`, `Racing`, `Simulation`, `Other`
- ✅ Campo `Description` é opcional (máximo 2000 caracteres)
- ✅ `IsActive` define se o jogo aparece no catálogo público

### RN-CAT-002: Listagem de Jogos
- ✅ Endpoint `GET /v1/games` (público ou autenticado)
- ✅ **Filtros disponíveis**: 
  - `Category`: Filtrar por categoria específica
  - `PriceMin` / `PriceMax`: Faixa de preço
  - `Title`: Busca parcial no título (case-insensitive)
- ✅ **Ordenação disponível**: `Price`, `Title`, `CreatedAt`
- ✅ **Paginação obrigatória** (`pageNumber`, `pageSize`)
- ✅ **Usuários comuns**: apenas jogos ativos (`IsActive = true`)
- ✅ **Admin**: pode ver jogos inativos

### RN-CAT-003: Cálculo de Preço com Promoção
Ao listar jogos, calcular preço final automaticamente:

**Se jogo tem promoção ativa:**
- ✅ Promoção com `IsActive = true`
- ✅ Data atual entre `StartDate` e `EndDate`
- ✅ **Fórmula**: `PriceFinal = Price - (Price × DiscountPercentage / 100)`

**Retornar sempre:**
```json
{
  "price": 100.00,           // Preço original
  "discountPercentage": 25,  // Desconto aplicado (se houver)
  "finalPrice": 75.00        // Preço com desconto
}
```

### RN-CAT-004: Gestão de Promoções (Admin)
- ✅ **Apenas Admin** pode criar/atualizar/deletar promoções
- ✅ `DiscountPercentage` entre **1% e 100%**
- ✅ `EndDate` deve ser **posterior a** `StartDate`
- ✅ **Apenas uma promoção ativa por jogo** simultaneamente
- ✅ Validar se jogo existe antes de criar promoção
- ✅ Promoção pode ser desativada manualmente (`IsActive = false`)

### RN-CAT-005: Criação de Biblioteca
- ✅ Biblioteca é criada **automaticamente** ao consumir `UserCreatedEvent`
- ✅ **Apenas uma Library por UserId**
- ✅ **Idempotência**: verificar se Library já existe
- ✅ Biblioteca vazia (sem jogos) no momento da criação
- ✅ Biblioteca não pode ser criada manualmente via API

### RN-CAT-006: Iniciação de Compra
**Endpoint:** `POST /v1/library/purchase`

**Validações obrigatórias:**
1. ✅ **Usuário autenticado** (extrair `UserId` do JWT)
2. ✅ **Jogo existe** e está ativo (`IsActive = true`)
3. ✅ **Jogo NÃO está na biblioteca** do usuário (evitar duplicata)
4. ✅ Calcular **preço final** (considerar promoção ativa)

**Fluxo:**
- ✅ Publicar `OrderPlacedEvent` com:
  - `UserId`: Identificador do usuário
  - `GameId`: Identificador do jogo
  - `Amount`: Preço final calculado
- ✅ Retornar status: `"Aguardando processamento de pagamento"`
- ✅ **Não adicionar** jogo à biblioteca ainda (aguardar confirmação de pagamento)

### RN-CAT-007: Adição de Jogo à Biblioteca
Consumir `PaymentProcessedEvent`:

**Se `Status = 'Approved'`:**
- ✅ Adicionar jogo à biblioteca (`LibraryGames`)
- ✅ Salvar **preço pago** em `PurchasePrice`
- ✅ Registrar data/hora em `PurchasedAt`
- ✅ **Idempotência**: verificar se jogo já está na biblioteca

**Se `Status = 'Rejected'`:**
- ✅ **Não fazer nada** (jogo não é adicionado)
- ✅ Usuário pode tentar novamente quando tiver saldo

### RN-CAT-008: Listagem de Biblioteca
- ✅ Endpoint: `GET /v1/library`
- ✅ **Usuário comum**: listar apenas **sua própria biblioteca**
- ✅ **Admin**: pode consultar biblioteca de qualquer usuário
- ✅ **Retornar**: 
  - Dados completos do jogo (`Title`, `Description`, `Category`)
  - Preço pago (`PurchasePrice`)
  - Data de aquisição (`PurchasedAt`)
- ✅ **Ordenação padrão**: `PurchasedAt DESC` (mais recentes primeiro)
- ✅ **Paginação obrigatória**

---

## 🔌 Endpoints da API

### Catálogo de Jogos (Games)

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `POST` | `/v1/games` | ✅ Sim | Admin | Criar novo jogo |
| `GET` | `/v1/games` | ❌ Não | Público | Listar jogos (com filtros) |
| `GET` | `/v1/games/{id}` | ❌ Não | Público | Obter detalhes do jogo |
| `PUT` | `/v1/games/{id}` | ✅ Sim | Admin | Atualizar jogo |
| `DELETE` | `/v1/games/{id}` | ✅ Sim | Admin | Deletar jogo |

**POST /v1/games** _(Admin apenas)_
```json
Request:
{
  "title": "The Legend of Adventure",
  "description": "Um jogo épico de aventura em mundo aberto",
  "price": 199.90,
  "category": "Adventure",
  "isActive": true
}

Response: 201 Created
{
  "id": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "title": "The Legend of Adventure",
  "description": "Um jogo épico de aventura em mundo aberto",
  "price": 199.90,
  "category": "Adventure",
  "isActive": true,
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**GET /v1/games?category=Action&priceMax=150&pageNumber=1&pageSize=10**
```json
Response: 200 OK
{
  "data": [
    {
      "id": "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q",
      "title": "Combat Arena",
      "description": "Batalhas intensas em arenas futuristas",
      "price": 89.90,
      "discountPercentage": 30,
      "finalPrice": 62.93,
      "category": "Action",
      "isActive": true,
      "hasActivePromotion": true,
      "createdAt": "2026-01-15T08:00:00Z"
    },
    {
      "id": "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r",
      "title": "Speed Fighter",
      "description": "Lute em alta velocidade contra inimigos épicos",
      "price": 120.00,
      "discountPercentage": null,
      "finalPrice": 120.00,
      "category": "Action",
      "isActive": true,
      "hasActivePromotion": false,
      "createdAt": "2026-01-10T14:30:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 5,
  "totalRecords": 48
}
```

**GET /v1/games/{id}**
```json
Response: 200 OK
{
  "id": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "title": "The Legend of Adventure",
  "description": "Um jogo épico de aventura em mundo aberto com gráficos impressionantes",
  "price": 199.90,
  "discountPercentage": 25,
  "finalPrice": 149.93,
  "category": "Adventure",
  "isActive": true,
  "hasActivePromotion": true,
  "promotion": {
    "id": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a",
    "discountPercentage": 25,
    "startDate": "2026-01-15T00:00:00Z",
    "endDate": "2026-01-31T23:59:59Z",
    "isActive": true
  },
  "createdAt": "2026-01-10T10:30:00Z",
  "updatedAt": "2026-01-15T09:00:00Z"
}
```

### Promoções (Promotions)

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `POST` | `/v1/promotions` | ✅ Sim | Admin | Criar nova promoção |
| `PUT` | `/v1/promotions/{id}` | ✅ Sim | Admin | Atualizar promoção |
| `DELETE` | `/v1/promotions/{id}` | ✅ Sim | Admin | Deletar promoção |

**POST /v1/promotions** _(Admin apenas)_
```json
Request:
{
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "discountPercentage": 30,
  "startDate": "2026-01-20T00:00:00Z",
  "endDate": "2026-02-10T23:59:59Z",
  "isActive": true
}

Response: 201 Created
{
  "id": "8b7a6c5d-4e3f-2d1c-0b9a-8f7e6d5c4b3a",
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "gameTitle": "The Legend of Adventure",
  "discountPercentage": 30,
  "startDate": "2026-01-20T00:00:00Z",
  "endDate": "2026-02-10T23:59:59Z",
  "isActive": true,
  "createdAt": "2026-01-18T10:30:00Z"
}
```

### Biblioteca (Library)

| Método | Endpoint | Autenticação | Autorização | Descrição |
|--------|----------|--------------|-------------|-----------|
| `POST` | `/v1/library/purchase` | ✅ Sim | User | Iniciar compra de jogo |
| `GET` | `/v1/library` | ✅ Sim | User | Listar biblioteca |

**POST /v1/library/purchase**
```json
Request:
{
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p"
}

Response: 202 Accepted
{
  "orderId": "f1e2d3c4-b5a6-9870-1234-567890abcdef",
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "gameTitle": "The Legend of Adventure",
  "amount": 149.93,
  "status": "Aguardando processamento de pagamento",
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**GET /v1/library?pageNumber=1&pageSize=10**
```json
Response: 200 OK
{
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "data": [
    {
      "id": "5d4c3b2a-1e0f-9g8h-7i6j-5k4l3m2n1o0p",
      "game": {
        "id": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
        "title": "The Legend of Adventure",
        "description": "Um jogo épico de aventura em mundo aberto",
        "category": "Adventure",
        "currentPrice": 199.90
      },
      "purchasePrice": 149.93,
      "purchasedAt": "2026-01-18T10:35:00Z"
    },
    {
      "id": "6e5d4c3b-2f1g-0h9i-8j7k-6l5m4n3o2p1q",
      "game": {
        "id": "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q",
        "title": "Combat Arena",
        "description": "Batalhas intensas em arenas futuristas",
        "category": "Action",
        "currentPrice": 89.90
      },
      "purchasePrice": 89.90,
      "purchasedAt": "2026-01-15T14:20:00Z"
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 1,
  "totalRecords": 8
}
```

---

## 📨 Eventos

A aplicação utiliza **Apache Kafka** para comunicação assíncrona baseada em eventos (Event-Driven Architecture).

### 📥 Eventos Consumidos

#### UserCreatedEvent

**Tópico Kafka:** `user-created`

```json
{
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "name": "João Silva",
  "email": "joao@example.com",
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**Ação:**
- ✅ Criar `Library` vazia para o usuário
- ✅ Verificar idempotência (não duplicar bibliotecas)
- ✅ Associar `UserId` à nova biblioteca

#### PaymentProcessedEvent

**Tópico Kafka:** `payment-processed`

```json
{
  "correlationId": "f1e2d3c4-b5a6-9870-1234-567890abcdef",
  "paymentId": "9c8b7a6d-5e4f-3d2c-1b0a-9f8e7d6c5b4a",
  "orderId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "amount": 149.93,
  "status": "Approved",
  "processedAt": "2026-01-18T10:30:05Z"
}
```

**Ação (Status = Approved):**
- ✅ Buscar `Library` do usuário
- ✅ Adicionar jogo à `LibraryGames`
- ✅ Salvar preço pago (`PurchasePrice = amount`)
- ✅ Registrar data de aquisição (`PurchasedAt`)
- ✅ Verificar duplicatas (idempotência)

**Ação (Status = Rejected):**
- ✅ Não adicionar jogo à biblioteca
- ✅ Apenas logar evento para auditoria

### 📤 Eventos Publicados

#### OrderPlacedEvent

**Tópico Kafka:** `order-placed`

```json
{
  "correlationId": "f1e2d3c4-b5a6-9870-1234-567890abcdef",
  "orderId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "userId": "7b9e2c1a-8f4d-4e5b-9c3d-1a2b3c4d5e6f",
  "gameId": "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
  "amount": 149.93,
  "createdAt": "2026-01-18T10:30:00Z"
}
```

**Quando é disparado:**
- ✅ Ao chamar `POST /v1/library/purchase`
- ✅ Após validar que jogo existe e não está na biblioteca
- ✅ Após calcular preço final (com desconto se aplicável)

**Consumidores esperados:**
- ✅ **PaymentsAPI**: Processar pagamento do usuário

---

## ⚙️ Configuração e Execução

### Pré-requisitos

- ✅ .NET 8 SDK
- ✅ Docker e Docker Compose
- ✅ SQL Server 2022
- ✅ Apache Kafka (via Docker)
- 
### Configuração de Ambiente

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=fcg_catalog;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-chars",
    "Issuer": "FCG.Users.API",
    "Audience": "FCG.Catalog.API"
  },
  "KafkaSettings": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "fcg-catalog-api",
    "Topics": {
      "UserCreated": "user-created",
      "OrderPlaced": "order-placed",
      "PaymentProcessed": "payment-processed"
    }
  },
  "CatalogSettings": {
    "DefaultPageSize": 10,
    "MaxPageSize": 100,
    "CacheEnabled": true
  }
}
```

### Execução com Docker Compose

```bash
# Subir infraestrutura (SQL Server, Kafka)
docker-compose up -d