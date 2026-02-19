# 🛍️ E-Commerce REST API

A **production-grade E-Commerce REST API** built with **ASP.NET Core (.NET 9)** following **Clean Architecture** principles.  
Designed with a strong emphasis on **robust error handling**, **concurrency safety**, and **testability**.

---

## 🚀 Overview

This project demonstrates how to build a scalable, maintainable, and resilient backend for an e-commerce system — covering the full flow from authentication and shopping cart management to order creation and payment processing.

---

## ⭐ Key Highlights

### 🧱 Result Pattern — Zero Exception-Based Control Flow

All service methods return `Result<T>` or `Result` instead of throwing exceptions. This enforces **explicit, typed error handling** across the entire application.

- **No hidden exceptions** — every failure is visible in the return type
- **Centralized error-to-HTTP mapping** — `NotFound` → 404, `Unauthorized` → 403, fallback → 400
- **All error codes and messages** live in `DomainErrors.cs` — no magic strings scattered across the codebase

---

### 🔒 Optimistic Concurrency Control

The system uses **EF Core's `RowVersion`** (optimistic concurrency tokens) on critical entities (`ProductVariant`, `Order`) to prevent race conditions like double-booking or overselling.

**How it works in practice:**
1. Two users add the last item to their carts and checkout simultaneously
2. The first `CommitTransactionAsync()` succeeds
3. The second detects a `RowVersion` mismatch → `ConcurrencyConflictException`
4. The service catches it, **rolls back the transaction**, and returns `Result.Failure` — no data corruption

---

### 🧪 Unit Tests with xUnit + Moq + FluentAssertions

The `OrderService` — the most complex service — is covered by **4 comprehensive test suites**:

| Test Suite | Scenarios Covered |
|---|---|
| `OrderServiceCreateTests` | Empty cart, invalid user, insufficient stock, successful order, generic exception rollback |
| `OrderServiceGetTests` | Order not found, unauthorized access, successful retrieval |
| `OrderServiceUpdateTests` | Invalid status transitions, non-existent order, successful update |
| `OrderServiceConcurrencyTests` | `ConcurrencyConflictException` handling, **parallel race condition simulation** |

The concurrency tests simulate real-world scenarios where two users attempt to place orders simultaneously, verifying that exactly one succeeds and the other receives a typed failure result.

---

### 🏛️ Clean Architecture (4-Layer)

The solution follows strict Clean Architecture with an enforced **dependency rule** — inner layers know nothing about outer layers:

- **Domain** → Entities, Enums, Errors, Shared (`Result`/`Error`), Exceptions
- **Application** → DTOs, Interfaces (Services + Repositories), Service implementations
- **Infrastructure** → EF Core, Repository implementations, UnitOfWork, Stripe, Identity
- **API** → Controllers, Middleware, Configuration
- **Tests.Unit** → xUnit tests with Moq + FluentAssertions

---

## ✨ Implemented Features

### 🔐 Authentication & Authorization
- JWT-based authentication with access + refresh tokens
- Role-based authorization (`Admin` / `User`)
- Secure user management with ASP.NET Core Identity
- Token refresh mechanism for seamless session continuity

### 🛒 Shopping Cart
- Add, update, and remove cart items
- **Stock validation on add** — prevents adding more items than available inventory
- Cart persistence per authenticated user
- Automatic cart clearing after order creation
- Product variant support (size, color, SKU)

### 📦 Products & Categories
- Product listing with **pagination, filtering, and sorting**
- Full CRUD for products and categories (Admin only)
- Product variants with individual SKU, price, and stock tracking
- Product image management

### ⭐ Reviews & Ratings
- Users can submit 1–5 star reviews with title and comment
- **Duplicate review prevention** — one review per user per product
- Reviews require **admin approval** before becoming visible (`IsApproved = false` by default)
- Average rating calculation per product
- Review ownership enforcement (only the author can delete their review)

### 📋 Orders
- Order creation **directly from the shopping cart** with atomic stock deduction
- **Transactional order processing** with explicit `BEGIN → COMMIT / ROLLBACK`
- Order status lifecycle: `Pending → Processing → Shipped → Delivered`
- **Invalid status transition prevention** (e.g., can't go from `Delivered` back to `Processing`)
- Order history and search with pagination
- Admin-only status updates

### 💗 Wishlist
- Save products for later
- Duplicate item prevention (silently ignores if already in wishlist)
- Clear entire wishlist
- Explicit error when removing a product not in the wishlist

### 📍 Addresses
- Full CRUD for user addresses
- **Default shipping address management** — setting a new default automatically unsets the previous one
- Address ownership enforcement

### 💳 Payments (Stripe)
- Stripe Checkout integration (Test Mode)
- Dedicated Payments API (separated from Orders)
- Secure checkout session creation
- Frontend-ready design (API returns checkout URL)

---

## 🧩 Cross-Cutting Concerns

| Concern | Implementation |
|---|---|
| **Error Handling** | `Result<T>` / `Result` pattern + centralized `DomainErrors` |
| **Concurrency** | Optimistic concurrency via `RowVersion` on `ProductVariant` and `Order` |
| **Transactions** | Explicit `IUnitOfWork` with `Begin/Commit/Rollback` |
| **Logging** | Structured logging with **Serilog** |
| **Data Access** | Repository + Unit of Work patterns |
| **Mapping** | AutoMapper for Entity ↔ DTO transformations |
| **API Docs** | Swagger / OpenAPI with XML comments |

---

## 🛠️ Technology Stack

| Layer | Technologies |
|---|---|
| **Runtime** | ASP.NET Core Web API (.NET 9) |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Identity** | ASP.NET Core Identity + JWT |
| **Payments** | Stripe API (Test Mode) |
| **Logging** | Serilog |
| **Mapping** | AutoMapper |
| **Testing** | xUnit, Moq, FluentAssertions |
| **Docs** | Swagger / OpenAPI |

---

## ▶️ Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Run the Project

1. Clone the repository
2. Run `dotnet restore`
3. Apply migrations with `dotnet ef database update --project ECommerce.Infrastructure`
4. Start the API with `dotnet run --project ECommerce.API`
5. Open Swagger UI at `https://localhost:7000/swagger`

### Run Unit Tests

Run `dotnet test ECommerce.Tests.Unit` to execute all test suites.

---

## 💳 Stripe Test Mode

Use the following **Stripe test card** to simulate payments:

| Field | Value |
|---|---|
| Card Number | `4242 4242 4242 4242` |
| Expiry Date | Any future date |
| CVC | Any 3 digits |

> ⚠️ Payments are handled in **Test Mode only**.  
> The API returns a checkout URL, and redirection is handled by the client.

---

## 🔐 Authentication

All protected endpoints require a JWT token in the `Authorization: Bearer <token>` header.

Tokens are obtained via the Auth endpoints: `POST /api/auth/login`, `POST /api/auth/register`, and `POST /api/auth/refresh-token`.

---

## 📌 API Highlights

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/auth/login` | Authenticate and get JWT |
| `POST` | `/api/orders` | Create order from cart |
| `GET` | `/api/orders` | User order history |
| `PUT` | `/api/orders/{id}/status` | Admin: update order status |
| `POST` | `/api/payments/checkout` | Start Stripe checkout |
| `GET` | `/api/cart` | Get user cart |
| `POST` | `/api/cart` | Add item to cart |
| `GET` | `/api/wishlist` | Get user wishlist |
| `POST` | `/api/reviews` | Add product review |
| `GET` | `/api/reviews/product/{id}` | Get product reviews |
| `GET` | `/api/products` | Browse products (paginated) |
| `GET` | `/api/addresses` | Get user addresses |

Full documentation available via Swagger.

---
