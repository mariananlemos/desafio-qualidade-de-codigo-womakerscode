<div align="center">
  <h1>🛒 Desafio ECommerce — Refatoração</h1>
  <h3>Bootcamp de Desenvolvimento Backend com .NET | WoMakersCode</h3>
  <img src="https://capsule-render.vercel.app/api?type=rect&color=C77DFF&height=6&width=800" alt="" />
  <h2><strong>Desafio Qualidade de Código e Arquitetura — Squad Carmem Portinho</strong></h2>
</div>

## 📖 Sobre o Projeto

Este projeto é a refatoração de uma API de e-commerce (.NET / ASP.NET Core Web API) que foi **intencionalmente construída com problemas de qualidade de código**. O desafio consistiu em identificar esses problemas e aplicar boas práticas de arquitetura, SOLID, injeção de dependência, DTOs, transações e tratamento de erros.

O projeto original (`EcommercePiorApi`) simulava um monólito de e-commerce com todas as responsabilidades concentradas em um único "God Controller", sem camadas, sem injeção de dependência e sem tratamento de erros. A squad refatorou o código para uma arquitetura em camadas, com separação de responsabilidades e boas práticas de mercado.

## 🚀 Tecnologias e Dependências

* **Linguagem:** C# (.NET 10)
* **Framework:** ASP.NET Core Web API
* **ORM:** Entity Framework Core
* **Banco de Dados:** SQLite (arquivo local, sem necessidade de servidor)
* **Documentação:** Swagger / OpenAPI

---

## ⚙️ Como executar o projeto localmente

Para rodar este projeto na sua máquina, você precisará do [SDK do .NET](https://dotnet.microsoft.com/download) instalado. Utilize o terminal (PowerShell, Bash ou Git Bash) do seu VS Code.

### 1. Clonar o repositório

```bash
git clone https://github.com/mariananlemos/desafio-qualidade-de-codigo-womakerscode
cd desafio-qualidade-de-codigo-womakerscode/Ecommerce
```

### 2. Executar a API

Restaure as dependências e inicie o servidor:

```bash
dotnet restore
dotnet run
```

A API estará rodando localmente. Acesse a documentação do Swagger pelo navegador:

```text
http://localhost:5000/swagger
```

O banco SQLite (ecommerce.db) é criado automaticamente na primeira execução, com produtos de exemplo já cadastrados.

## 🏗️ Arquitetura (após refatoração)

O projeto foi separado em camadas:

* Domain/Entities — entidades de domínio (Produto, Pedido, ItemPedido)
* Application/DTOs — objetos de transferência (PedidoRequest, ItemRequest)
* Application/Services — regras de negócio (PedidoService, ProdutoService, etc.)
* Application/Interfaces — contratos dos serviços (IPedidoService, etc.)
* Infrastructure/Data — AppDbContext (EF Core + SQLite)
* Infrastructure/Integrations — mocks de pagamento e logística
* Controllers — endpoints HTTP finos, delegando aos serviços

## 🔧 Problemas atacados na refatoração

| # | Problema original | Solução aplicada |
|---|---|---|
| 1 | God Controller | Controllers separados por domínio + Services |
| 2 | Sem camadas | Domain / Application / Infrastructure / API |
| 3 | Sem injeção de dependência | DI configurada no `Program.cs` |
| 4 | Connection string hardcoded | Configuração via `AppDbContext` |
| 5 | Entidades expostas diretamente | DTOs de request (`PedidoRequest`, `ItemRequest`) |
| 6 | Números mágicos | Constantes nomeadas no `PedidoService` |
| 7 | Falta de transação | `BeginTransactionAsync` + compensação de estoque |
| 8 | Uso incorreto de `.Result` | `await` + métodos `async Task` |
| 9 | Duplicação de código | Lógica centralizada nos Services |
| 10 | Classe utilitária "faz-tudo" | Responsabilidades separadas |
| 11 | Pagamento com `Random` | `PagamentoService` com interface |
| 12 | Tratamento de erros ausente | `PedidoResultado` + try/catch |
| 13 | Status como string | (a documentar) |
| 14 | Tudo em um único projeto | Separação em pastas/camadas |

## 👩‍💻 Equipe e Plano de Ação

O desenvolvimento foi colaborativo, com cada integrante responsável por uma parte da refatoração.

### Luana Ferreira (Integrante A) — Arquitetura e Camadas [✅ CONCLUÍDO]

* Separação do projeto em pastas/camadas: Domain, Application, Infrastructure, API.
* Organização de Models, Services, DTOs e Repositories.
* Remoção do conceito de "tudo em uma pasta".

### Aline Shimoi (Integrante B) — SOLID e Injeção de Dependência [✅ CONCLUÍDO]

* Quebra do `EcommerceController` (God Controller).
* Criação de interfaces e Services.
* Configuração de Dependency Injection e injeção do `DbContext`.

### Mariana Lemos (Integrante C) — Regras de Negócio e Refatoração [✅ CONCLUÍDO]

* Remoção de números mágicos.
* Criação de Enum para Status.
* Refatoração de duplicações e separação de responsabilidades da classe `Utils`.
* Melhoria no pagamento e logística.

### Marcia Moreira (Integrante D) — Integração, Qualidade e Testes [🔄 EM ANDAMENTO]

* DTOs de request/response.
* Tratamento de exceções e middleware global de erro.
* Ajuste de `async/await` (remoção de `.Result`).
* Correção de transação de pedidos.
* Documentação e README.
* Integração final.

## 🔀 Regras de Colaboração e Versionamento (Git Flow)

Para garantir que o código de todas se integre sem quebrar o projeto, siga o padrão de repositório da equipe:

* Nunca faça commits diretos na branch main.
* Para cada tarefa, crie uma branch nova a partir da main utilizando os prefixos padronizados:
* feat/nome-da-tarefa — Para funcionalidades novas
* fix/nome-do-ajuste — Para correções de bugs
* docs/nome-do-texto — Para atualizações de README

Quando finalizar sua parte, envie para o GitHub (git push) e abra um Pull Request (PR) apontando para a main.

Uma colega de equipe deverá fazer o Code Review do seu PR, verificando se o código resolve o objetivo da tarefa. O código só entra na main após revisão e aprovação.

<div align="center"> <img src="https://capsule-render.vercel.app/api?type=rect&color=C77DFF&height=6&width=800" alt="" /> </div>
