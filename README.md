# SistemaEstoque API

API REST para gerenciamento de estoque, desenvolvida em **ASP.NET Core Web API**, com autenticação via **JWT**, persistência em **PostgreSQL/Neon**, deploy na **Azure App Service** e pipeline automatizado com **GitHub Actions**.

O projeto foi criado com foco em estudo, prática profissional e portfólio para vaga de **Backend C#/.NET Júnior**.

---

## Link da API online

Swagger publicado na Azure:

https://sistemaestoque-api-jorge.azurewebsites.net/swagger

---

## Status do projeto

Projeto funcional e publicado online.

Principais validações concluídas:

- API compilando sem erro.
- Testes automatizados passando.
- Deploy automático funcionando via GitHub Actions.
- Swagger online funcionando.
- Autenticação JWT funcionando.
- Cadastro, login e autorização funcionando.
- CRUD de categorias funcionando.
- CRUD de produtos funcionando.
- Registro e cancelamento de pedidos funcionando.
- Baixa de estoque ao registrar pedido funcionando.
- Devolução de estoque ao cancelar pedido funcionando.
- Dashboard funcionando.
- Relatório de produtos mais vendidos funcionando.

---

## Tecnologias utilizadas

- C#
- .NET
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Neon Database
- JWT Bearer Authentication
- Swagger / OpenAPI
- xUnit
- Azure App Service
- GitHub Actions
- Visual Studio

---

## Arquitetura do projeto

O projeto está dividido em camadas para separar responsabilidades e aproximar a estrutura de um projeto profissional.

```txt
SistemaEstoque
├── SistemaEstoque.Api
│   ├── Controllers
│   ├── Dtos
│   ├── Exceptions
│   ├── Services
│   └── Program.cs
│
├── SistemaEstoque.Data
│   ├── Context
│   ├── Entities
│   ├── Migrations
│   └── Repositories
│
├── SistemaEstoque.Tests
│   ├── Controllers
│   ├── Dtos
│   ├── Exceptions
│   └── Services
│
└── .github
    └── workflows
```

---

## Responsabilidades das camadas

### SistemaEstoque.Api

Camada responsável pela API.

Contém:

- Controllers
- DTOs
- Services
- Tratamento global de erros
- Configuração JWT
- Configuração Swagger
- Injeção de dependência
- Configuração do banco

### SistemaEstoque.Data

Camada responsável pelo acesso a dados.

Contém:

- Entidades
- DbContext
- Repositories
- Interfaces dos repositories
- Migrations do Entity Framework Core

### SistemaEstoque.Tests

Camada responsável pelos testes automatizados.

Contém testes para:

- DTOs
- Services
- Controllers
- Tratamento global de erros
- Regras de negócio

---

## Funcionalidades

### Autenticação

- Cadastro de usuário.
- Login de usuário.
- Geração de token JWT.
- Proteção dos endpoints com `[Authorize]`.
- Hash de senha com PBKDF2.
- Validação de credenciais inválidas.
- Bloqueio de email duplicado.

### Categorias

- Listar categorias.
- Buscar categoria por ID.
- Cadastrar categoria.
- Atualizar categoria.
- Remover categoria.
- Bloquear cadastro de categoria duplicada.
- Bloquear remoção de categoria com produtos vinculados.

### Produtos

- Listar produtos.
- Buscar produto por ID.
- Cadastrar produto.
- Atualizar produto.
- Remover produto.
- Ativar e desativar produto.
- Bloquear produto duplicado na mesma categoria.
- Bloquear remoção de produto usado em pedido.
- Validar estoque mínimo.
- Validar preço.
- Validar quantidade em estoque.

### Pedidos

- Listar pedidos.
- Buscar pedido por ID.
- Registrar pedido.
- Cancelar pedido.
- Baixar estoque ao registrar pedido.
- Devolver estoque ao cancelar pedido.
- Bloquear venda maior que o estoque.
- Bloquear venda de produto inativo.
- Agrupar itens repetidos do mesmo produto no pedido.
- Calcular total do pedido.
- Calcular total dos itens.

### Dashboard

Endpoint para visão geral do estoque.

Retorna:

- Total de produtos.
- Total de categorias.
- Total de pedidos.
- Produtos com estoque baixo.
- Quantidade total em estoque.
- Valor total do estoque.

### Relatórios

Endpoint de produtos mais vendidos.

Retorna:

- ID do produto.
- Nome do produto.
- Quantidade vendida.
- Valor total vendido.

---

## Autenticação JWT

A API utiliza JWT para proteger os endpoints.

Fluxo básico:

1. Criar usuário em `/api/Auth/register`.
2. Fazer login em `/api/Auth/login`.
3. Copiar o token retornado.
4. Clicar em `Authorize` no Swagger.
5. Colar o token JWT.
6. Acessar os endpoints protegidos.

Endpoints públicos:

```txt
POST /api/Auth/register
POST /api/Auth/login
```

Endpoints protegidos:

```txt
/api/Categorias
/api/Produtos
/api/Pedidos
/api/Dashboard
/api/Relatorios
```

---

## Principais endpoints

### Auth

```txt
POST /api/Auth/register
POST /api/Auth/login
```

### Categorias

```txt
GET    /api/Categorias
GET    /api/Categorias/{id}
POST   /api/Categorias
PUT    /api/Categorias/{id}
DELETE /api/Categorias/{id}
```

### Produtos

```txt
GET    /api/Produtos
GET    /api/Produtos/{id}
POST   /api/Produtos
PUT    /api/Produtos/{id}
DELETE /api/Produtos/{id}
```

### Pedidos

```txt
GET    /api/Pedidos
GET    /api/Pedidos/{id}
POST   /api/Pedidos
DELETE /api/Pedidos/{id}
```

### Dashboard

```txt
GET /api/Dashboard
```

### Relatórios

```txt
GET /api/Relatorios/produtos-mais-vendidos
```

---

## Exemplos de uso

### Registrar usuário

```json
{
  "nome": "Jorge Henrique",
  "email": "jorge@email.com",
  "senha": "123456"
}
```

### Login

```json
{
  "email": "jorge@email.com",
  "senha": "123456"
}
```

Resposta esperada:

```json
{
  "nome": "Jorge Henrique",
  "email": "jorge@email.com",
  "token": "token-jwt"
}
```

### Criar categoria

```json
{
  "nome": "Alimentos"
}
```

### Criar produto

```json
{
  "nome": "Arroz 5kg",
  "preco": 25.9,
  "quantidadeEmEstoque": 10,
  "estoqueMinimo": 2,
  "categoriaId": 1
}
```

### Atualizar produto

```json
{
  "nome": "Arroz 5kg",
  "preco": 27.9,
  "quantidadeEmEstoque": 15,
  "estoqueMinimo": 3,
  "ativo": true,
  "categoriaId": 1
}
```

### Criar pedido

```json
{
  "itens": [
    {
      "produtoId": 1,
      "quantidade": 2
    }
  ]
}
```

---

## Regras de negócio implementadas

- Usuário não pode ser cadastrado com email duplicado.
- Login falha com email ou senha inválidos.
- Categoria não pode ser duplicada.
- Categoria com produto vinculado não pode ser removida.
- Produto não pode ser duplicado dentro da mesma categoria.
- Produto usado em pedido não pode ser removido.
- Produto pode ser desativado.
- Produto inativo não pode ser vendido.
- Pedido não pode ser criado sem itens.
- Pedido não pode vender quantidade maior que o estoque.
- Pedido baixa o estoque automaticamente.
- Cancelamento de pedido devolve o estoque automaticamente.
- Itens repetidos do mesmo produto são agrupados no pedido.
- Dashboard calcula totais do estoque.
- Relatório agrupa produtos vendidos.

---

## Tratamento global de erros

A API possui um tratamento global de exceções.

Exemplos de respostas:

```txt
400 Bad Request
```

Para dados inválidos ou regras de negócio violadas.

```txt
401 Unauthorized
```

Para credenciais inválidas.

```txt
404 Not Found
```

Para recursos não encontrados.

```txt
409 Conflict
```

Para conflitos, como cadastro duplicado.

```txt
500 Internal Server Error
```

Para erros internos ou configuração incorreta.

---

## Testes automatizados

O projeto possui testes automatizados com xUnit.

Os testes cobrem:

- Validações de DTOs.
- Regras de negócio de categorias.
- Regras de negócio de produtos.
- Regras de negócio de pedidos.
- Regras de autenticação.
- Dashboard.
- Relatórios.
- Controllers.
- Tratamento global de erros.

Quantidade atual:

```txt
107 testes automatizados
```

Comando para rodar os testes:

```bash
dotnet test .\SistemaEstoque.Api.slnx
```

---

## Pipeline CI/CD

O projeto utiliza GitHub Actions para:

- Restaurar dependências.
- Compilar a solução.
- Rodar os testes automatizados.
- Publicar a API.
- Fazer deploy automático na Azure App Service.

Fluxo:

```txt
push na main
↓
restore
↓
build
↓
test
↓
publish
↓
deploy Azure
```

---

## Banco de dados

O projeto utiliza PostgreSQL hospedado no Neon.

O Entity Framework Core é usado para:

- Mapeamento das entidades.
- Criação das tabelas.
- Migrations.
- Consultas.
- Persistência dos dados.

---

## Deploy

A API está publicada na Azure App Service.

Link do Swagger online:

```txt
https://sistemaestoque-api-jorge.azurewebsites.net/swagger
```

---

## Como rodar o projeto localmente

### 1. Clonar o repositório

```bash
git clone https://github.com/jorgehenriquenc/SistemaEstoque.git
```

### 2. Entrar na pasta do projeto

```bash
cd SistemaEstoque
```

### 3. Restaurar dependências

```bash
dotnet restore .\SistemaEstoque.Api.slnx
```

### 4. Configurar variáveis locais

A API precisa das seguintes configurações:

```txt
ConnectionStrings:DefaultConnection
Jwt:Key
Jwt:Issuer
Jwt:Audience
```

Exemplo de estrutura no `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SistemaEstoque;Username=postgres;Password=sua-senha"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-com-tamanho-suficiente",
    "Issuer": "SistemaEstoque",
    "Audience": "SistemaEstoque"
  }
}
```

Não envie senhas reais para o GitHub.

### 5. Rodar migrations

```bash
dotnet ef database update --project .\SistemaEstoque.Data --startup-project .\SistemaEstoque.Api
```

### 6. Rodar a API

```bash
dotnet run --project .\SistemaEstoque.Api
```

### 7. Abrir Swagger local

```txt
https://localhost:porta/swagger
```

A porta pode variar conforme a configuração local.

---

## Como rodar os testes

```bash
dotnet test .\SistemaEstoque.Api.slnx
```

Resultado esperado:

```txt
Failed: 0
```

---

## Estrutura das entidades principais

### Usuario

Representa o usuário autenticado da API.

Campos principais:

- Id
- Nome
- Email
- SenhaHash

### Categoria

Representa uma categoria de produtos.

Campos principais:

- Id
- Nome
- Produtos

### Produto

Representa um produto do estoque.

Campos principais:

- Id
- Nome
- Preco
- QuantidadeEmEstoque
- EstoqueMinimo
- Ativo
- CategoriaId
- Categoria

### Pedido

Representa uma venda/pedido.

Campos principais:

- Id
- DataPedido
- Itens

### ItemPedido

Representa um item dentro de um pedido.

Campos principais:

- Id
- PedidoId
- ProdutoId
- Quantidade
- PrecoUnitario

---

## O que foi praticado neste projeto

- Criação de API REST com ASP.NET Core.
- Organização em camadas.
- Uso de DTOs.
- Repository Pattern.
- Service Layer.
- Entity Framework Core.
- Relacionamentos entre entidades.
- Migrations.
- PostgreSQL.
- Autenticação JWT.
- Hash de senha.
- Swagger.
- Tratamento global de erros.
- Testes automatizados.
- Deploy em nuvem.
- CI/CD com GitHub Actions.
- Boas práticas para portfólio backend.

---

## Autor

Desenvolvido por Jorge Henrique.

GitHub:

https://github.com/jorgehenriquenc