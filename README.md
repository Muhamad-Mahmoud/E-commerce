<div align="center">

# 🛍️ E-Commerce REST API

### A Production-Grade E-Commerce Backend

**Built with ASP.NET Core (.NET 9) · Clean Architecture · Result Pattern**

<br/>

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Stripe](https://img.shields.io/badge/Stripe-008CDD?style=for-the-badge&logo=stripe&logoColor=white)](https://stripe.com/)
[![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://xunit.net/)

<br/>

[Key Highlights](#-key-highlights) · [Features](#-features) · [Architecture](#-architecture) · [Getting Started](#️-getting-started) · [API Reference](#-api-reference)

---

*A scalable and maintainable e-commerce backend covering the **full flow** — from authentication*
*and cart management to order creation and Stripe payment processing.*

</div>

<br/>

## ⭐ Key Highlights

<table>
<tr>
<td width="50%">

### 🧱 Result Pattern
All service methods return `Result<T>` instead of throwing exceptions — enforcing **explicit, typed error handling** across the entire codebase. All error codes and messages live in a centralized `DomainErrors` class — no magic strings.

</td>
<td width="50%">

### 🔒 Concurrency Control
**Optimistic concurrency** via EF Core `RowVersion` tokens on critical entities like `ProductVariant` and `Order` — preventing race conditions, double-booking, and overselling with automatic rollback.

</td>
</tr>
<tr>
<td width="50%">

### 🧪 Comprehensive Testing
4 test suites covering the most complex service with **xUnit + Moq + FluentAssertions** — including **parallel race condition simulation**.

</td>
<td width="50%">

### 🏛️ Clean Architecture
Strict 4-layer separation with enforced dependency rule — inner layers know nothing about outer layers. Controllers act as thin pass-throughs with zero business logic.

</td>
</tr>
</table>

<br/>

## 🏗️ Architecture

The solution follows **Clean Architecture** with a strict dependency rule:

| Layer | Responsibility |
|:--|:--|
| **ECommerce.Domain** | Entities · Enums · Errors · Result/Error · Exceptions |
| **ECommerce.Application** | DTOs · Service Interfaces & Implementations |
| **ECommerce.Infrastructure** | EF Core · Repositories · UoW · Stripe · Identity |
| **ECommerce.API** | Controllers · Middleware · Configuration |
| **ECommerce.Tests.Unit** | xUnit · Moq · FluentAssertions |

> **Dependency Rule:** `API → Application → Domain ← Infrastructure`

<br/>

## ✨ Features

### 🔐 Authentication & Authorization

| Feature | Details |
|:--|:--|
| JWT Authentication | Access token + Refresh token pair |
| Role-Based Access | `Admin` and `User` roles |
| Identity Provider | ASP.NET Core Identity |
| Session Continuity | Automatic token refresh mechanism |

### 🛒 Shopping Cart

- Add, update, and remove cart items
- **Stock validation on add** — prevents adding more items than available inventory
- Cart persistence per authenticated user
- Automatic cart clearing after order creation
- Product variant support (size, color, SKU)

### 📦 Products & Categories

- Product listing with **pagination, filtering, and sorting**
- Full CRUD for products and categories *(Admin only)*
- Product variants with individual SKU, price, and stock tracking
- Product image management

### 📋 Orders

- Order creation **directly from cart** with atomic stock deduction
- **Transactional processing** — explicit `BEGIN → COMMIT / ROLLBACK`
- Status lifecycle: `Pending` → `Processing` → `Shipped` → `Delivered`
- **Invalid status transition prevention** (e.g., can't go from `Delivered` → `Processing`)
- Order cancellation with **automatic stock restoration**
- Order history & search with pagination
- Admin-only status updates

### ⭐ Reviews & Ratings

- 1–5 star reviews with title and comment
- **Duplicate review prevention** — one review per user per product
- **Admin approval** required before visibility (`IsApproved = false` by default)
- Average rating calculation per product
- Review ownership enforcement (only author can delete)

### 💗 Wishlist

- Save products for later
- Duplicate item prevention (silently ignores if already exists)
- Clear entire wishlist
- Explicit error when removing a product not in the wishlist

### 📍 Addresses

- Full CRUD for user addresses
- **Default address management** — setting a new default auto-unsets the previous one
- Address ownership enforcement

### 💳 Payments (Stripe)

- Stripe Checkout integration *(Test Mode)*
- Dedicated Payments API *(separated from Orders)*
- Webhook support for payment fulfillment
- Secure checkout session creation
- Frontend-ready design — API returns checkout URL

<br/>

## 🧩 Cross-Cutting Concerns

| Concern | Implementation |
|:--|:--|
| **Error Handling** | `Result<T>` pattern + centralized `DomainErrors` |
| **Concurrency** | Optimistic concurrency via `RowVersion` on critical entities |
| **Transactions** | Explicit `IUnitOfWork` with `Begin` / `Commit` / `Rollback` |
| **Logging** | Structured logging with **Serilog** |
| **Data Access** | Repository + Unit of Work patterns |
| **Mapping** | AutoMapper for Entity ↔ DTO transformations |
| **API Docs** | Swagger / OpenAPI with XML comments |

<br/>

## 🧪 Unit Tests

The `OrderService` — the most complex service — is covered by **4 comprehensive test suites**:

| Test Suite | Scenarios |
|:--|:--|
| **CreateTests** | Empty cart · Invalid user · Insufficient stock · Successful order · Generic exception rollback |
| **GetTests** | Order not found · Unauthorized access · Successful retrieval |
| **UpdateTests** | Invalid status transitions · Non-existent order · Successful update |
| **ConcurrencyTests** | Concurrency exception handling · **Parallel race condition simulation** |

> The concurrency tests simulate real-world scenarios where two users attempt to place orders simultaneously, verifying that exactly one succeeds and the other receives a typed failure result.

<br/>

## 🛠️ Tech Stack

| Category | Technology |
|:--|:--|
| **Runtime** | ASP.NET Core Web API (.NET 9) |
| **ORM** | Entity Framework Core 9 |
| **Database** | SQL Server |
| **Auth** | ASP.NET Core Identity + JWT Bearer |
| **Payments** | Stripe API (Checkout + Webhooks) |
| **Logging** | Serilog |
| **Mapping** | AutoMapper |
| **Testing** | xUnit · Moq · FluentAssertions |
| **Docs** | Swagger / OpenAPI |

<br/>

## ▶️ Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository
2. Run `dotnet restore`
3. Apply migrations: `dotnet ef database update --project ECommerce.Infrastructure`
4. Start the API: `dotnet run --project ECommerce.API`
5. Run tests: `dotnet test ECommerce.Tests.Unit`

> 🌐 Swagger UI available at **`https://localhost:7000/swagger`**

<br/>

## 🔐 Authentication

All protected endpoints require a JWT token: `Authorization: Bearer <token>`

| Endpoint | Description |
|:--|:--|
| `POST /api/auth/register` | Create a new account |
| `POST /api/auth/login` | Sign in and receive JWT |
| `POST /api/auth/refresh-token` | Refresh an expired token |

<br/>

## 💳 Stripe Test Mode

| Field | Value |
|:--|:--|
| Card Number | `4242 4242 4242 4242` |
| Expiry | Any future date |
| CVC | Any 3 digits |

> ⚠️ Payments run in **Test Mode only**. The API returns a Stripe checkout URL — redirection is handled by the client.

---

<div align="center">

**Built with ❤️ using .NET 9 and Clean Architecture**

</div>
