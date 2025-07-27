# Sistema de Gerenciamento de Pedidos

## Visão Geral

Este é um sistema robusto para gerenciamento de pedidos de bebidas entre revendas e fornecedores.
O sistema garante que nenhum pedido seja perdido, mesmo em cenários de falhas de comunicação com APIs externas.

## Arquitetura do Sistema

### Componentes Principais

- **API REST** - Endpoints para gerenciamento de revendas e pedidos
- **Outbox Pattern** - Garantia de entrega de pedidos
- **Background Service** - Processamento assíncrono de mensagens
- **Resilience Pattern** - Tratamento de falhas com retry automático
- **Entity Framework** - ORM com consultas otimizadas

### Fluxo de Funcionamento

1. **Cadastro de Revendas** - Dados completos com validações
2. **Criação de Pedidos** - Pedidos de clientes para revendas
3. **Processamento Automático** - Envio para fornecedor via Outbox
4. **Monitoramento** - Acompanhamento do status dos pedidos

## Endpoints da API

### 1. Gerenciamento de Revendas

#### 1.1 Criar Revenda

```http
POST /api/resellers
Content-Type: application/json

{
  "cnpj": "11.222.333/0001-81",
  "razaoSocial": "Distribuidora ABC LTDA",
  "nomeFantasia": "ABC Bebidas",
  "email": "contato@abc.com",
  "phones": [
    {
      "number": "11987654321",
      "isPrimary": true
    }
  ],
  "contacts": [
    {
      "name": "João Silva",
      "isPrimary": true
    }
  ],
  "deliveryAddresses": [
    {
      "Neighborhood":"Centro",
      "street": "Rua das Flores",
      "number": "123",
      "city": "São Paulo",
      "state": "SP",
      "zipCode": "01234-567",
      "isPrimary": true
    }
  ]
}
```

**Resposta:**

```json
{
  "id": 1,
  "cnpj": "11.222.333/0001-81",
  "razaoSocial": "Distribuidora ABC LTDA",
  "nomeFantasia": "ABC Bebidas",
  "email": "contato@abc.com",
  "phones": [...],
  "contacts": [...],
  "deliveryAddresses": [...],
  "isActive": false,
  "createdDate": "2025-07-26T19:15:22.4792063Z",
  "responseId": "49c3db98-3fb6-45da-ad7a-81d66122050b"
}
```

#### 1.2 Outros Endpoints de Revenda

```http
GET    /api/resellers/{id}           # Buscar por ID
GET    /api/resellers/cnpj/{cnpj}    # Buscar por CNPJ
GET    /api/resellers                # Listar todas
PUT    /api/resellers/{id}           # Atualizar
DELETE /api/resellers/{id}           # Excluir
```

### 2. Gerenciamento de Pedidos

#### 2.1 Criar Pedido de Cliente

```http
POST /api/resellers/{resellerId}/orders
Content-Type: application/json

{
  "customerIdentification": "Bar do José",
  "items": [
    {
      "productSku": "SKOL350",
      "productName": "Skol 350ml",
      "quantity": 500,
      "unitPrice": 2.50
    },
    {
      "productSku": "BRAHMA350",
      "productName": "Brahma 350ml",
      "quantity": 600,
      "unitPrice": 2.30
    }
  ]
}
```

**Resposta:**

```json
{
  "orderId": 1,
  "customerIdentification": "Bar do José",
  "items": [...],
  "status": "Pending",
  "orderCreatedAt": "2025-07-27T19:41:49.6785603Z",
  "correlationId": "abc123-def456-ghi789",
  "enqueuedForProcessing": true,
  "responseId": "26610a34-eae6-421e-a02e-c50901d890ba"
}
```

**O que acontece:**

1. Valida se a revenda existe
2. Cria o pedido no banco de dados
3. Verifica se há quantidade suficiente (mínimo 1000 unidades)
4. Enfileira no Outbox para envio ao fornecedor
5. Retorna resposta imediata ao cliente

#### 2.2 Outros Endpoints de Pedidos

```http
GET  /api/orders/{id}                    # Buscar por ID
GET  /api/resellers/{resellerId}/orders  # Listar por revenda
POST /api/orders/process-pending         # Processar pendentes (manual)
```

### 3. Monitoramento do Outbox

```http
GET  /api/outbox/pending-count           # Contagem de mensagens pendentes
GET  /api/outbox/messages/{correlationId} # Buscar por correlationId
POST /api/outbox/process-pending         # Forçar processamento
POST /api/outbox/process-retry           # Forçar retry
```

**Exemplo de resposta do correlationId:**

```json
[
  {
    "id": 1,
    "type": "AmbevOrderSubmission",
    "status": "Completed",
    "retryCount": 0,
    "createdAt": "2024-01-15T10:30:00Z",
    "processedAt": "2024-01-15T10:31:00Z",
    "correlationId": "abc123-def456"
  }
]
```

## Background Service - OutboxProcessorService

### Como Funciona

O `OutboxProcessorService` é um serviço de background que roda continuamente:

- **Intervalo:** Executa a cada 1 minuto
- **Processamento:** Busca e processa mensagens pendentes
- **Retry:** Processa mensagens que falharam anteriormente

#### Fluxo de Processamento

1. **Busca Mensagens Pendentes** - Consulta mensagens com status "Pending"
2. **Busca Mensagens de Retry** - Consulta mensagens agendadas para retry
3. **Processa Cada Mensagem** - Executa a lógica específica do tipo
4. **Atualiza Status** - Marca como "Completed", "Failed" ou "Retry"

#### Estratégia de Retry

- **Exponential Backoff:** 2, 4, 8, 16 minutos
- **Máximo de Tentativas:** 3 por mensagem
- **Logs Detalhados:** Rastreamento completo do processo

### Tipos de Mensagens

#### AmbevOrderSubmission

- **Propósito:** Envio de pedidos para o fornecedor
- **Processo:**
  1. Busca pedidos no banco
  2. Verifica quantidade mínima (1000 unidades)
  3. Envia para API do fornecedor
  4. Atualiza status dos pedidos

## Regras de Negócio

### Pedidos

- **Quantidade Mínima:** 1000 unidades por envio
- **Agrupamento:** Pedidos da mesma revenda são agrupados
- **Status:** Pending → SentToAmbev → Confirmed/Failed

### Revendas

- **CNPJ:** Obrigatório e único
- **Contatos:** Pelo menos um principal
- **Endereços:** Pelo menos um de entrega

### Resiliência

- **Retry Automático:** Até 3 tentativas
- **Backoff Exponencial:** Intervalos crescentes
- **Persistência:** Nenhum pedido é perdido

## Configuração

### Variáveis de Ambiente

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=AmbevOrderSystem.db"
  },
  "AmbevApi": {
    "BaseUrl": "https://api.ambev.com/",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "UseMockService": true
  },
  "OrderProcessing": {
    "MinimumQuantity": 1000,
    "ProcessingIntervalMinutes": 5
  }
}
```

## Testando a API

### Collection do Postman

Para facilitar os testes da API, uma collection completa do Postman está disponível na raiz do projeto:

**`AmbevOrderSystem.postman_collection.json`**

Esta collection inclui:

- Todos os endpoints da API
- Exemplos de payloads válidos
- Variáveis de ambiente configuradas
- Cenários de sucesso e erro

**Como usar:**

1. Abra o Postman
2. Importe o arquivo `AmbevOrderSystem.postman_collection.json`
3. Configure a variável `base_url` para `https://localhost:7028`
4. Execute os requests na ordem sugerida

## Monitoramento e Observabilidade

### Logs

- **Structured Logging:** Com Serilog
- **Níveis:** Information, Warning, Error
- **Contexto:** CorrelationId para rastreamento

### Métricas

- **Pedidos Pendentes:** Contagem em tempo real
- **Taxa de Sucesso:** Processamento vs falhas
- **Tempo de Processamento:** Performance das operações

### Health Checks

- **Endpoint:** `/health`
- **Verificações:** Banco de dados, serviços externos
- **Status:** Healthy, Degraded, Unhealthy

## Exemplo de Uso Completo

### 1. Criar Revenda

```bash
curl -X POST https://localhost:7028/api/resellers \
  -H "Content-Type: application/json" \
  -d '{
    "cnpj": "12.345.678/0001-90",
    "razaoSocial": "Revenda Exemplo LTDA",
    "nomeFantasia": "Revenda Exemplo",
    "email": "contato@revendaexemplo.com"
  }'
```

### 2. Criar Pedido

```bash
curl -X POST https://localhost:7028/api/resellers/1/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerIdentification": "Cliente A",
    "items": [
      {
        "productSku": "SKU001",
        "productName": "Cerveja Premium",
        "quantity": 1000,
        "unitPrice": 10.00
      }
    ]
  }'
```

### 3. Monitorar Status

```bash
# Verificar contagem de mensagens pendentes
curl https://localhost:7028/api/outbox/pending-count

# Rastrear processamento usando correlationId
curl https://localhost:7028/api/outbox/messages/abc123-def456-ghi789
```

## Rastreamento com CorrelationId

O `correlationId` retornado na criação do pedido permite rastrear todo o processamento:

```bash
# 1. Criar pedido (recebe correlationId)
POST /api/resellers/1/orders
# Resposta: { "correlationId": "abc123-def456-ghi789", "enqueuedForProcessing": true }

# 2. Rastrear processamento
GET /api/outbox/messages/abc123-def456-ghi789
# Resposta: Lista de mensagens com status de processamento
```

## Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **Polly** - Biblioteca de resiliência
- **Serilog** - Logging estruturado
- **FluentValidation** - Validações
- **AutoMapper** - Mapeamento de objetos
- **Swagger** - Documentação da API

## Executando o Projeto

### Pré-requisitos

- .NET 8 SDK
- Visual Studio 2022 ou VS Code

### Execução

```bash
# Restaurar dependências
dotnet restore

# Executar testes
dotnet test

# Executar aplicação
dotnet run --project src/AmbevOrderSystem.Api
```

### Acessos

- **API:** https://localhost:7028
- **Swagger:** https://localhost:7028/swagger
- **Health Check:** https://localhost:7028/health
