# SistemaEstoque

API REST desenvolvida em C# com ASP.NET Core para gerenciamento de estoque, produtos, categorias e pedidos.

O projeto foi criado com foco em praticar arquitetura em camadas, Entity Framework Core, SQL Server, DTOs, Services e Repository Pattern.

---

## Tecnologias utilizadas

- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger
- LINQ
- DTOs
- Repository Pattern
- Migrations

---

## Arquitetura do projeto

O projeto segue uma arquitetura em camadas:

```text
Controllers
    ↓
Services
    ↓
Repositories
    ↓
DbContext
    ↓
SQL Server